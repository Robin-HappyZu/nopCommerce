using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.ExternalAuth.QQConnect
{
    public class QQConnectExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public QQConnectExternalAuthMethod(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        #endregion

        #region Methods

        public void GetConfigurationRoute(out string actionName, out string controllerName, out System.Web.Routing.RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ExternalAuthQQConnect";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.ExternalAuth.QQConnect.Controllers" }, { "area", null } };
        }

        public void GetPublicInfoRoute(out string actionName, out string controllerName, out System.Web.Routing.RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "ExternalAuthQQConnect";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.ExternalAuth.QQConnect.Controllers" }, { "area", null } };
        }

        public override void Install()
        {
            //settings
            var settings = new QQConnectExternalAuthSettings()
            {
                AppId = string.Empty,
                AppKey = string.Empty
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQConnect.AppId", "App ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQConnect.AppId.Hint", "Enter your app ID You can find it on your QQ Connect application page.");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQConnect.AppKey", "API Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQConnect.AppKey.Hint", "Enter your app KEY You can find it on your QQ Connect application page.");

            base.Install();

        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<QQConnectExternalAuthSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.QQConnect.AppId");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.QQConnect.AppId.Hint");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.QQConnect.AppKey");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.QQConnect.AppKey.Hint");

            base.Uninstall();
        }

        #endregion
    }
}
