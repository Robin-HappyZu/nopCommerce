using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Happyzu.Plugin.Wechat.Web.Models.Layout;
using Nop.Web.Framework.Controllers;

namespace Happyzu.Plugin.Wechat.Web.Controllers
{
    public class LayoutController : BasePluginController
    {
        [ChildActionOnly]
        public ActionResult FootMenu(string activeMenu = "")
        {
            if (string.IsNullOrWhiteSpace(activeMenu))
            {
                activeMenu = "Home";
            }

            var footMenuVM = new FootMenuViewModel()
            {
                ActiveMenuItemName = activeMenu,
                ShoppingCartItemCount = 10
            };
            return PartialView("FootMenu", footMenuVM);
        }

        [ChildActionOnly]
        public ActionResult HeaderBar(HeaderViewModel vm)
        {
            return PartialView("HeaderBar", vm);
        }
    }
}
