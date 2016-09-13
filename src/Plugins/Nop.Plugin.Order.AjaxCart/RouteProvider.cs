﻿using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Plugin.Order.AjaxCart.ViewEngines;

namespace Nop.Plugin.Order.AjaxCart
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            System.Web.Mvc.ViewEngines.Engines.Add(new OrderAjaxCartViewEngine());

            routes.MapRoute("Plugin.Order.AjaxCart.LoadCart",
                 "Plugins/Order/LoadCart",
                 new { controller = "AjaxCart", action = "LoadCart" },
                 new[] { "Nop.Plugin.Order.AjaxCart.Controllers" }
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
