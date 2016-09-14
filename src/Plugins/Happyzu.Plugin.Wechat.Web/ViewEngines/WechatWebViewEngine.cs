using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Themes;

namespace Happyzu.Plugin.Wechat.Web.ViewEngines
{
    public class WechatWebViewEngine : ThemeableRazorViewEngine
    {
        public WechatWebViewEngine()
        {
            var themeContext = EngineContext.Current.Resolve<IThemeContext>();

            PartialViewLocationFormats = new[]
            {
                "~/Themes/" + themeContext.WorkingThemeName + "/Views/Plugins/Happyzu.Plugin.Wechat.Web/{1}/{0}.cshtml",
                "~/Plugins/Happyzu.Plugin.Wechat.Web/Themes/" + themeContext.WorkingThemeName + "/Views/{1}/{0}.cshtml",
                "~/Plugins/Happyzu.Plugin.Wechat.Web/Views/{1}/{0}.cshtml"
            };
            ViewLocationFormats = new[]
            {
                "~/Themes/" + themeContext.WorkingThemeName + "/Views/Plugins/Happyzu.Plugin.Wechat.Web/{1}/{0}.cshtml",
                "~/Plugins/Happyzu.Plugin.Wechat.Web/Themes/" + themeContext.WorkingThemeName + "/Views/{1}/{0}.cshtml",
                "~/Plugins/Happyzu.Plugin.Wechat.Web/Views/{1}/{0}.cshtml"
            };
        }
    }
}