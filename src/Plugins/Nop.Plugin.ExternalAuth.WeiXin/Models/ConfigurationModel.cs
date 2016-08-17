using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework;

namespace Nop.Plugin.ExternalAuth.WeiXin.Models
{
    public class ConfigurationModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.WeiXin.ClientKeyIdentifier")]
        public string ClientKeyIdentifier { get; set; }
        public bool ClientKeyIdentifier_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.WeiXin.ClientSecret")]
        public string ClientSecret { get; set; }
        public bool ClientSecret_OverrideForStore { get; set; }
    }
}
