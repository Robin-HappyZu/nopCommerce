using System;
using System.Runtime.Serialization;

namespace Nop.Plugin.ExternalAuth.QQConnect.AspNet.Client
{
    [DataContract]
    public class QQConnectOpenIdData
    {

        [DataMember(Name = "client_id", IsRequired = true)]
        public string Client_Id { get; set; }
        [DataMember(Name = "openid", IsRequired = true)]
        public string OpenId { get; set; }
    }
}
