using System.Web.Mvc;
using Nop.Web.Framework.Controllers;

namespace Happyzu.Plugin.Wechat.Web.Controllers
{
    public class HomeController : BasePluginController
    {
        public ActionResult Index()
        {
            return View("~/Plugins/Happyzu.Plugin.Wechat.Web/Views/Home/Index.cshtml");
        }

        //public ActionResult Configure()
        //{
        //    return View("~/Plugins/Happyzu.Plugin.Wechat.Web/Views/Home/Configure.cshtml");
        //}
    }
}
