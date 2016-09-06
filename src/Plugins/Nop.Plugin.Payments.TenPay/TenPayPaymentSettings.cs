using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.TenPay
{
    public class TenPayPaymentSettings : ISettings
    {
        /// <summary>
        /// 账号标识(AppId)
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 受理商ID(MchId)
        /// </summary>
        public string MchId { get; set; }

        /// <summary>
        /// 密钥(Key)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 开发者(AppSecret)
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        ///  额外手续费
        /// </summary>
        public decimal AdditionalFee { get; set; }
    }
}
