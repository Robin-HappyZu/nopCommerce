using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.TenPay
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //Notify
            routes.MapRoute("Plugin.Payments.TenPay.Notify",
                 "Plugins/PaymentTenPay/Notify",
                 new { controller = "PaymentTenPay", action = "Notify" },
                 new[] { "Nop.Plugin.Payments.TenPay.Controllers" }
            );

            //JsPay
            routes.MapRoute("Plugin.Payments.TenPay.JsPay",
                 "Plugins/PaymentTenPay/JsPay",
                 new { controller = "PaymentTenPay", action = "JsApi" },
                 new[] { "Nop.Plugin.Payments.TenPay.Controllers" }
            );

            //Native 扫码支付
            routes.MapRoute("Plugin.Payments.TenPay.Native",
               "Plugins/PaymentTenPay/Native",
               new { controller = "PaymentTenPay", action = "Native" },
                 new[] { "Nop.Plugin.Payments.TenPay.Controllers" }
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
