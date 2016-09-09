using Nop.Core.Plugins;
using Nop.Services.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Nop.Plugin.Order.AjaxCart
{
    public class AjaxCartProcessor : BasePlugin,IWidgetPlugin
    {
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "AjaxCart";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Order.AjaxCart.Controllers" }, { "area", null } };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "AjaxCart";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Order.AjaxCart.Controllers" }, { "area", null }, { "widgetZone", widgetZone } };
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string> { "order_summary_content_after"};
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            base.Install();
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
    }
}
