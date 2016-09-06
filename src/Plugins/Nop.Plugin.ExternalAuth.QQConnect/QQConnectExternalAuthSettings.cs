using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.QQConnect
{
    public class QQConnectExternalAuthSettings:ISettings
    {
        public string AppId { get; set; }

        public string AppKey { get; set; }
    }
}
