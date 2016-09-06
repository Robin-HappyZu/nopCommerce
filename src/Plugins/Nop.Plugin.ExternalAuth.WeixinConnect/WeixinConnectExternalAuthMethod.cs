using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.ExternalAuth.WeixinConnect
{
    /// <summary>
    ///     Weixin externalAuth processor
    /// </summary>
    public class WeixinConnectExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public WeixinConnectExternalAuthMethod(ISettingService settingService)
        {
            _settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     插件配置路由
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        public void GetConfigurationRoute(out string actionName, out string controllerName,
            out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ExternalAuthWeixinConnect";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.ExternalAuth.WeixinConnect.Controllers"},
                {"area", null}
            };
        }

        /// <summary>
        ///     前端插件显示
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        public void GetPublicInfoRoute(out string actionName, out string controllerName,
            out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "ExternalAuthWeixinConnect";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.ExternalAuth.WeixinConnect.Controllers"},
                {"area", null}
            };
        }

        /// <summary>
        /// 安装插件
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new WeixinConnectExternalAuthSettings
            {
                ClientKeyIdentifier = "",
                ClientSecret = ""
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeixinConnect.Login", "微信网站扫码登陆");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeixinConnect.ClientKeyIdentifier", "App ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeixinConnect.ClientKeyIdentifier.Hint",
                "输入你的 app ID/API key");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeixinConnect.ClientSecret", "App Secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeixinConnect.ClientSecret.Hint", "输入 app secret here");
            base.Install();
        }

        /// <summary>
        /// 卸载插件
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<WeixinConnectExternalAuthSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeixinConnect.Login");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeixinConnect.ClientKeyIdentifier");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeixinConnect.ClientKeyIdentifier.Hint");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeixinConnect.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeixinConnect.ClientSecret.Hint");
            base.Uninstall();
        }

        #endregion
    }
}