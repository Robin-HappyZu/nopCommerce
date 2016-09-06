using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using Nop.Plugin.ExternalAuth.QQConnect.Core;
using Nop.Plugin.ExternalAuth.QQConnect.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.ExternalAuth.QQConnect.Controllers
{
    public class ExternalAuthQQConnectController : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IPluginFinder _pluginFinder;
        private readonly IOAuthProviderQQConnectAuthorizer _oAuthProviderQQConnectAuthorizer;
        private readonly IOpenAuthenticationService _openAuthenticationService;


        public ExternalAuthQQConnectController(ISettingService settingService,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IPermissionService permissionService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWorkContext workContext,
            IPluginFinder pluginFinder,
            IOAuthProviderQQConnectAuthorizer oAuthProviderQQConnectAuthorizer,
            IOpenAuthenticationService openAuthenticationService)
        {
            this._settingService = settingService;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._permissionService = permissionService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._workContext = workContext;
            this._pluginFinder = pluginFinder;
            this._oAuthProviderQQConnectAuthorizer = oAuthProviderQQConnectAuthorizer;
            this._openAuthenticationService = openAuthenticationService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return Content("Access denied");

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var connectExternalAuthSettings = _settingService.LoadSetting<QQConnectExternalAuthSettings>(storeScope);

            var model = new ConfigurationModel();
            model.AppId = connectExternalAuthSettings.AppId;
            model.AppKey = connectExternalAuthSettings.AppKey;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AppId_OverrideForStore = _settingService.SettingExists(connectExternalAuthSettings, x => x.AppId, storeScope);
                model.AppKey_OverrideForStore = _settingService.SettingExists(connectExternalAuthSettings, x => x.AppKey, storeScope);
            }

            return View("~/Plugins/ExternalAuth.QQConnect/Views/ExternalAuthQQConnect/Configure.cshtml", model);
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
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var connectExternalAuthSettings = _settingService.LoadSetting<QQConnectExternalAuthSettings>(storeScope);

            //save settings
            connectExternalAuthSettings.AppId = model.AppId;
            connectExternalAuthSettings.AppKey = model.AppKey;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.AppId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(connectExternalAuthSettings, x => x.AppId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(connectExternalAuthSettings, x => x.AppId, storeScope);

            if (model.AppKey_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(connectExternalAuthSettings, x => x.AppKey, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(connectExternalAuthSettings, x => x.AppKey, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            return Configure();
        }


        [ChildActionOnly]
        public ActionResult PublicInfo()
        {
            return View("~/Plugins/ExternalAuth.QQConnect/Views/ExternalAuthQQConnect/PublicInfo.cshtml");
        }

        [NonAction]
        private ActionResult LoginInternal(string returnUrl, bool verifyResponse)
        {
            var processor = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName("ExternalAuth.QQConnect");
            if (processor == null ||
                !processor.IsMethodActive(_externalAuthenticationSettings) ||
                !processor.PluginDescriptor.Installed ||
                !_pluginFinder.AuthenticateStore(processor.PluginDescriptor, _storeContext.CurrentStore.Id))
                throw new NopException("QQConnect module cannot be loaded");
            
            var viewModel = new LoginModel();
            TryUpdateModel(viewModel);

            var result = _oAuthProviderQQConnectAuthorizer.Authorize(returnUrl, verifyResponse);
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
            return HttpContext.Request.IsAuthenticated ? new RedirectResult(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "~/") : new RedirectResult(Url.LogOn(returnUrl));
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
