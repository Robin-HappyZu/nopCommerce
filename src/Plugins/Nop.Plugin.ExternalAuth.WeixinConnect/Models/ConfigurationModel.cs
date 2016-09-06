using Nop.Web.Framework;

namespace Nop.Plugin.ExternalAuth.WeixinConnect.Models
{
    public class ConfigurationModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.WeixinConnect.ClientKeyIdentifier")]
        public string ClientKeyIdentifier { get; set; }
        public bool ClientKeyIdentifier_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.WeixinConnect.ClientSecret")]
        public string ClientSecret { get; set; }
        public bool ClientSecret_OverrideForStore { get; set; }
    }
}
