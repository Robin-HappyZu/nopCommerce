using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.ExternalAuth.WeiXin
{
    /// <summary>
    ///     Weixin externalAuth processor
    /// </summary>
    public class WeiXinExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public WeiXinExternalAuthMethod(ISettingService settingService)
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
            controllerName = "ExternalAuthWeiXin";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.ExternalAuth.WeiXin.Controllers"},
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
            controllerName = "ExternalAuthWeiXin";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.ExternalAuth.WeiXin.Controllers"},
                {"area", null}
            };
        }

        /// <summary>
        ///     安装插件
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new WeiXinExternalAuthSettings
            {
                ClientKeyIdentifier = "",
                ClientSecret = ""
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeiXin.Login", "微信登陆");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeiXin.ClientKeyIdentifier", "App ID/API Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeiXin.ClientKeyIdentifier.Hint",
                "输入你的 app ID/API key");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeiXin.ClientSecret", "App Secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeiXin.ClientSecret.Hint", "输入 app secret here");
            base.Install();
        }

        /// <summary>
        ///     卸载插件
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<WeiXinExternalAuthSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeiXin.Login");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeiXin.ClientKeyIdentifier");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeiXin.ClientKeyIdentifier.Hint");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeiXin.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeiXin.ClientSecret.Hint");
            base.Uninstall();
        }

        #endregion
    }
}