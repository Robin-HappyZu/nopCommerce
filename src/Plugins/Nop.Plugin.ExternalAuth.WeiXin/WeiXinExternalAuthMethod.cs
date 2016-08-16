using System;
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.WeiXin
{
    /// <summary>
    /// Weixin externalAuth processor
    /// </summary>
    public class WeiXinExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod
    {
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            throw new NotImplementedException();
        }

        public void GetPublicInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 安装插件
        /// </summary>
        public override void Install()
        {
            base.Install();
        }

        /// <summary>
        /// 卸载插件
        /// </summary>
        public override void Uninstall()
        {
            base.Uninstall();
        }
    }
}
