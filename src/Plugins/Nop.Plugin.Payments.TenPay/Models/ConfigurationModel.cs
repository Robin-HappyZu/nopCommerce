using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.TenPay.Models
{
    public class ConfigurationModel: BaseNopModel
    {
        /// <summary>
        /// 账号标识(AppId)
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.TenPay.AppId")]
        public string AppId { get; set; }

        /// <summary>
        /// 受理商ID(MchId)
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.TenPay.MchId")]
        public string MchId { get; set; }

        /// <summary>
        /// 密钥(Key)
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.TenPay.Key")]
        public string Key { get; set; }

        /// <summary>
        /// 开发者(AppSecret)
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.TenPay.AppSecret")]
        public string AppSecret { get; set; }

        /// <summary>
        ///  额外手续费
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.TenPay.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
    }
}
