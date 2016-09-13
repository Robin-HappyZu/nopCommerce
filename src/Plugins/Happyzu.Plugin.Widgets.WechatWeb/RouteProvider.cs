using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Happyzu.Plugin.Widgets.WechatWeb
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Happyzu.Wechat.Web.Home",
                 "Wechat",
                 new { controller = "WechatWebHome", action = "Index" },
                 new[] { "Happyzu.Plugin.Widgets.WechatWeb.Controllers" }
            );
        }

        public int Priority => 5;
    }
}
