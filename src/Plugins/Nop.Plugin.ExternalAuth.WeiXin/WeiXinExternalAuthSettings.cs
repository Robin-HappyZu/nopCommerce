using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.WeiXin
{
    public class WeiXinExternalAuthSettings : ISettings
    {
        public string ClientKeyIdentifier { get; set; }
        public string ClientSecret { get; set; }
    }
}
