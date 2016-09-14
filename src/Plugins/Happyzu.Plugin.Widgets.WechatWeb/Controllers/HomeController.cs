using System.Web.Mvc;
using Nop.Web.Framework.Controllers;

namespace Happyzu.Plugin.Widgets.WechatWeb.Controllers
{
    public class HomeController : BasePluginController
    {
        public ActionResult Index()
        {
            return View("~/Plugins/Happyzu.Plugin.Widgets.WechatWeb/Views/Home/Index.cshtml");
        }

        //public ActionResult Configure()
        //{
        //    return View("~/Plugins/Happyzu.Plugin.Widgets.WechatWeb/Views/Home/Configure.cshtml");
        //}
    }
}
