using Nop.Core.Plugins;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using System.Web.Routing;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.TenPay.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Core;

namespace Nop.Plugin.Payments.TenPay
{
    public class TenPayPaymentProcessor : BasePlugin, IPaymentMethod
    {

        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly TenPayPaymentSettings _tenPayPaymentSettings;

        #endregion

        #region Ctor

        public TenPayPaymentProcessor(
            IWebHelper webHelper,
            ISettingService settingService,
            TenPayPaymentSettings tenPayPaymentSettings)
        {
            _webHelper = webHelper;
            _settingService = settingService;
            _tenPayPaymentSettings = tenPayPaymentSettings;
        }

        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return false; }
        }

        #endregion

        #region Methods
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var orderGuid = postProcessPaymentRequest.Order.OrderGuid.ToString();

            var post = new RemotePost
            {
                FormName = "tenpaysubmit",
                Url = _webHelper.GetStoreLocation(false) + "Plugins/PaymentTenPay/Native",
                Method = "POST"
            };
            post.Add("OrderGuid", orderGuid);

            post.Post();
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Pending };

            return result;
        }

        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //AliPay is the redirection payment method
            //It also validates whether order is also paid (after redirection) so customers will not be able to pay twice

            //payment status should be Pending
            if (order.PaymentStatus != PaymentStatus.Pending)
                return false;

            //let's ensure that at least 1 minute passed after order is placed
            return !((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes < 1);
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentTenPay";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.TenPay.Controllers" }, { "area", null } };
        }

        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentTenPay";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.TenPay.Controllers" }, { "area", null } };
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return _tenPayPaymentSettings.AdditionalFee;
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();

            result.AddError("Recurring payment not supported");

            return result;
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();

            result.AddError("Recurring payment not supported");

            return result;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();

            result.AddError("Capture method not supported");

            return result;
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();

            result.AddError("Refund method not supported");

            return result;
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();

            result.AddError("Void method not supported");

            return result;
        }
        
        public Type GetControllerType()
        {
            return typeof(PaymentTenPayController);
        }

        public override void Install()
        {
            //settings
            var settings = new TenPayPaymentSettings()
            {
                AppId = "",
                AppSecret = "",
                Key = "",
                MchId = "",
                AdditionalFee = 0,
            };

            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TenPay.AppId", "账号标识(AppId):");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TenPay.MchId", "受理商ID(MchId):");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TenPay.Key", "密钥(Key):");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TenPay.AppSecret", "开发者(AppSecret):");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TenPay.AdditionalFee", "额外费用:");

            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.TenPay.AppId");
            this.DeletePluginLocaleResource("Plugins.Payments.TenPay.AppSecret");
            this.DeletePluginLocaleResource("Plugins.Payments.TenPay.Key");
            this.DeletePluginLocaleResource("Plugins.Payments.TenPay.MchId");
            this.DeletePluginLocaleResource("Plugins.Payments.TenPay.AdditionalFee");

            base.Uninstall();
        }

        #endregion
    }
}
