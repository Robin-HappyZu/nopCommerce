using Nop.Core.Plugins;
using Nop.Services.Common;
using System.Web.Routing;

namespace Happyzu.Plugin.Wechat.Web
{
    public class WechatWebPlugin : BasePlugin, IMiscPlugin
    {
        /// <summary>
        /// 插件安装
        /// </summary>
        public override void Install()
        {
            base.Install();
        }

        /// <summary>
        /// 插件卸载
        /// </summary>
        public override void Uninstall()
        {
            base.Uninstall();
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "Admin";
            routeValues = new RouteValueDictionary { { "Namespaces", "Happyzu.Plugin.Wechat.Web.Controllers" }, { "area", null } };
        }
    }
}
