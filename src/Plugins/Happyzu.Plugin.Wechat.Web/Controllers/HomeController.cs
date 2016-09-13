using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Happyzu.Plugin.Wechat.Web.Controllers
{
    public class HomeController : BasePluginController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
