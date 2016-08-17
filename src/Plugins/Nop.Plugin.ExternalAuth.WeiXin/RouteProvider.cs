﻿using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.ExternalAuth.WeiXin
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.ExternalAuth.WeiXin.Login",
                 "Plugins/ExternalAuthWeiXin/Login",
                 new { controller = "ExternalAuthWeiXin", action = "Login" },
                 new[] { "Nop.Plugin.ExternalAuth.WeiXin.Controllers" }
            );

            routes.MapRoute("Plugin.ExternalAuth.WeiXin.LoginCallback",
                 "Plugins/ExternalAuthWeiXin/LoginCallback",
                 new { controller = "ExternalAuthWeiXin", action = "LoginCallback" },
                 new[] { "Nop.Plugin.ExternalAuth.WeiXin.Controllers" }
            );
        }

        public int Priority => 0;
    }
}
