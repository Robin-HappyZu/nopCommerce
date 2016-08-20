using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.WeixinConnect
{
    public class WeixinConnectExternalAuthSettings : ISettings
    {
        public string ClientKeyIdentifier { get; set; }
        public string ClientSecret { get; set; }
    }
}
