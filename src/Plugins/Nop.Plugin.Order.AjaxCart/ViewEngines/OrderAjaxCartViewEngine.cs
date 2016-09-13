using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Themes;

namespace Nop.Plugin.Order.AjaxCart.ViewEngines
{
    public class OrderAjaxCartViewEngine:ThemeableRazorViewEngine
    {
        public OrderAjaxCartViewEngine()
        {
            var themeContext = EngineContext.Current.Resolve<IThemeContext>();

            PartialViewLocationFormats = new[]
            {
                "~/Themes/" + themeContext.WorkingThemeName + "/Views/Plugins/Order.AjaxCart/{1}/{0}.cshtml",
                "~/Plugins/Order.AjaxCart/Themes/" + themeContext.WorkingThemeName + "/Views/{1}/{0}.cshtml",
                "~/Plugins/Order.AjaxCart/Views/{1}/{0}.cshtml"
            };
            ViewLocationFormats = new[]
            {
                "~/Themes/" + themeContext.WorkingThemeName + "/Views/Plugins/Order.AjaxCart/{1}/{0}.cshtml",
                "~/Plugins/Order.AjaxCart/Themes/" + themeContext.WorkingThemeName + "/Views/{1}/{0}.cshtml",
                "~/Plugins/Order.AjaxCart/Views/{1}/{0}.cshtml"
            };
        }
    }
}
