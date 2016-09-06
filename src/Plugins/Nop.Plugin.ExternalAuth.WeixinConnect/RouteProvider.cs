using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.ExternalAuth.WeixinConnect
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.ExternalAuth.WeixinConnect.Login",
                 "Plugins/ExternalAuthWeixinConnect/Login",
                 new { controller = "ExternalAuthWeixinConnect", action = "Login" },
                 new[] { "Nop.Plugin.ExternalAuth.WeixinConnect.Controllers" }
            );

            routes.MapRoute("Plugin.ExternalAuth.WeixinConnect.LoginCallback",
                 "Plugins/ExternalAuthWeixinConnect/LoginCallback",
                 new { controller = "ExternalAuthWeixinConnect", action = "LoginCallback" },
                 new[] { "Nop.Plugin.ExternalAuth.WeixinConnect.Controllers" }
            );
        }

        public int Priority => 0;
    }
}
