using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.ExternalAuth.QQConnect
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.ExternalAuth.QQConnect.Login",
                "Plugins/ExternalAuthQQConnect/Login",
                new { controller = "ExternalAuthQQConnect", action = "Login" },
                new[] { "Nop.Plugin.ExternalAuth.QQConnect.Controllers" }
           );

            routes.MapRoute("Plugin.ExternalAuth.QQConnect.LoginCallback",
                 "Plugins/ExternalAuthQQConnect/LoginCallback",
                 new { controller = "ExternalAuthQQConnect", action = "LoginCallback" },
                 new[] { "Nop.Plugin.ExternalAuth.QQConnect.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
