using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.TenPay.Models
{
    public class NativePayModel
    {
        public string OrderTotal { get; set; }

        public string QrCodeBase64String { get; set; }
    }
}
