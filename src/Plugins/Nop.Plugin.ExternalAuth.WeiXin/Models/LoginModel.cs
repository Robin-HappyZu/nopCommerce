using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.WeiXin.Models
{
    public class LoginModel
    {
        public string ExternalIdentifier { get; set; }
        public string KnownProvider { get; set; }
        public string ReturnUrl { get; set; }
    }
}
