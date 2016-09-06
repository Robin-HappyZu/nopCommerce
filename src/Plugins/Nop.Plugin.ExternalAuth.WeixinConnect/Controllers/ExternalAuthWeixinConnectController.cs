using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using Nop.Plugin.ExternalAuth.WeixinConnect.Core;
using Nop.Plugin.ExternalAuth.WeixinConnect.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.ExternalAuth.WeixinConnect.Controllers
{
    public class ExternalAuthWeixinConnectController : BasePluginController
    {
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IOAuthProviderWeixinConnectAuthorizer _oAuthProviderWeixinConnectAuthorizer;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly IPermissionService _permissionService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        public ExternalAuthWeixinConnectController(ISettingService settingService,
            IOAuthProviderWeixinConnectAuthorizer oAuthProviderWeixinConnectAuthorizer,
            IOpenAuthenticationService openAuthenticationService,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IPermissionService permissionService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWorkContext workContext,
            IPluginFinder pluginFinder,
            ILocalizationService localizationService)
        {
            _settingService = settingService;
            _oAuthProviderWeixinConnectAuthorizer = oAuthProviderWeixinConnectAuthorizer;
            _openAuthenticationService = openAuthenticationService;
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _permissionService = permissionService;
            _storeContext = storeContext;
            _storeService = storeService;
            _workContext = workContext;
            _pluginFinder = pluginFinder;
            _localizationService = localizationService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return Content("Access denied");

            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var weiXinExternalAuthSettings = _settingService.LoadSetting<WeixinConnectExternalAuthSettings>(storeScope);

            var model = new ConfigurationModel();
            model.ClientKeyIdentifier = weiXinExternalAuthSettings.ClientKeyIdentifier;
            model.ClientSecret = weiXinExternalAuthSettings.ClientSecret;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.ClientKeyIdentifier_OverrideForStore = _settingService.SettingExists(weiXinExternalAuthSettings,
                    x => x.ClientKeyIdentifier, storeScope);
                model.ClientSecret_OverrideForStore = _settingService.SettingExists(weiXinExternalAuthSettings,
                    x => x.ClientSecret, storeScope);
            }

            return View("~/Plugins/ExternalAuth.WeixinConnect/Views/ExternalAuthWeixinConnect/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return Content("Access denied");

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var weiXinExternalAuthSettings = _settingService.LoadSetting<WeixinConnectExternalAuthSettings>(storeScope);

            //save settings
            weiXinExternalAuthSettings.ClientKeyIdentifier = model.ClientKeyIdentifier;
            weiXinExternalAuthSettings.ClientSecret = model.ClientSecret;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(weiXinExternalAuthSettings, x => x.ClientKeyIdentifier,
                model.ClientKeyIdentifier_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(weiXinExternalAuthSettings, x => x.ClientSecret,
                model.ClientSecret_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo()
        {
            return View("~/Plugins/ExternalAuth.WeixinConnect/Views/ExternalAuthWeixinConnect/PublicInfo.cshtml");
        }

        [NonAction]
        private ActionResult LoginInternal(string returnUrl, bool verifyResponse)
        {
            var processor = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(Provider.SystemName);
            if (processor == null ||
                !processor.IsMethodActive(_externalAuthenticationSettings) ||
                !processor.PluginDescriptor.Installed ||
                !_pluginFinder.AuthenticateStore(processor.PluginDescriptor, _storeContext.CurrentStore.Id))
                throw new NopException("WeixinConnect Auth module cannot be loaded");

            var viewModel = new LoginModel();
            TryUpdateModel(viewModel);

            var result = _oAuthProviderWeixinConnectAuthorizer.Authorize(returnUrl, verifyResponse);
            switch (result.AuthenticationStatus)
            {
                case OpenAuthenticationStatus.Error:
                    {
                        if (!result.Success)
                            foreach (var error in result.Errors)
                                ExternalAuthorizerHelper.AddErrorsToDisplay(error);

                        return new RedirectResult(Url.LogOn(returnUrl));
                    }
                case OpenAuthenticationStatus.AssociateOnLogon:
                    {
                        return new RedirectResult(Url.LogOn(returnUrl));
                    }
                case OpenAuthenticationStatus.AutoRegisteredEmailValidation:
                    {
                        //result
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                    }
                case OpenAuthenticationStatus.AutoRegisteredAdminApproval:
                    {
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                    }
                case OpenAuthenticationStatus.AutoRegisteredStandard:
                    {
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                    }
                default:
                    break;
            }

            if (result.Result != null) return result.Result;
            return HttpContext.Request.IsAuthenticated
                ? new RedirectResult(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "~/")
                : new RedirectResult(Url.LogOn(returnUrl));
        }

        public ActionResult Login(string returnUrl)
        {
            return LoginInternal(returnUrl, false);
        }

        public ActionResult LoginCallback(string returnUrl)
        {
            return LoginInternal(returnUrl, true);
        }
    }
}
