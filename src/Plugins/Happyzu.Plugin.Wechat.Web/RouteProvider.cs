using System;
using System.Web.Mvc;
using System.Web.Routing;
using Happyzu.Plugin.Wechat.Web.ViewEngines;
using Nop.Web.Framework.Mvc.Routes;

namespace Happyzu.Plugin.Wechat.Web
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            System.Web.Mvc.ViewEngines.Engines.Add(new WechatWebViewEngine());

            routes.MapRoute("Happyzu.Wechat.Web.Home",
                 "Wechat",
                 new { controller = "Home", action = "Index" },
                 new[] { "Happyzu.Plugin.Wechat.Web.Controllers" }
            );
        }

        public int Priority => Int32.MaxValue;
    }
}
