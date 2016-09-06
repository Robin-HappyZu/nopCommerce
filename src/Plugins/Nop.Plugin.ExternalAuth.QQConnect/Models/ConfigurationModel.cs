using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.QQConnect.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.QQConnect.AppId")]
        public string AppId { get; set; }
        public bool AppId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.QQConnect.AppKey")]
        public string AppKey { get; set; }
        public bool AppKey_OverrideForStore { get; set; }
    }
}
