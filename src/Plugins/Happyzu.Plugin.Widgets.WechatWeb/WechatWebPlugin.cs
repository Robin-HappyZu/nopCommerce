using System;
using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Cms;

namespace Happyzu.Plugin.Widgets.WechatWeb
{
    public class WechatWebPlugin : BasePlugin, IWidgetPlugin
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

        public IList<string> GetWidgetZones()
        {
            return new List<string>() {""};
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WechatWebHome";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Happyzu.Plugin.Widgets.WechatWeb.Controllers" },
                { "area", null }
            };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName,
            out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WechatWebHome";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Happyzu.Plugin.Widgets.WechatWeb.Controllers" },
                { "area", null }
            };
        }
    }
}
