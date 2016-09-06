using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.TenPay.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.Serialization;
using ZXing;
using ZXing.Common;

namespace Nop.Plugin.Payments.TenPay.Controllers
{
    public class PaymentTenPayController : BasePaymentController
    {

        #region Fields

        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        private readonly TenPayPaymentSettings _tenPayPaymentSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly IWebHelper _webHelper;
        private readonly IStoreContext _storeContext;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PaymentTenPayController(
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            ILogger logger,
            ILocalizationService localizationService,
            TenPayPaymentSettings tenPayPaymentSettings,
            PaymentSettings paymentSettings,
            IWebHelper webHelper,
            IStoreContext storeContext,
            IPriceFormatter priceFormatter,
            ICurrencyService currencyService,
            IWorkContext workContext)
        {
            _settingService = settingService;
            _paymentService = paymentService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _logger = logger;
            _localizationService = localizationService;
            _tenPayPaymentSettings = tenPayPaymentSettings;
            _paymentSettings = paymentSettings;
            _webHelper = webHelper;
            _storeContext = storeContext;
            _priceFormatter = priceFormatter;
            _currencyService = currencyService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel
            {
                AppId = _tenPayPaymentSettings.AppId,
                MchId = _tenPayPaymentSettings.MchId,
                Key = _tenPayPaymentSettings.Key,
                AppSecret = _tenPayPaymentSettings.AppSecret,
                AdditionalFee = _tenPayPaymentSettings.AdditionalFee
            };

            return View("~/Plugins/Payments.TenPay/Views/PaymentTenPay/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _tenPayPaymentSettings.AppId = model.AppId;
            _tenPayPaymentSettings.MchId = model.MchId;
            _tenPayPaymentSettings.Key = model.Key;
            _tenPayPaymentSettings.AppSecret = model.AppSecret;
            _tenPayPaymentSettings.AdditionalFee = model.AdditionalFee;
            _settingService.SaveSetting(_tenPayPaymentSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            return View("~/Plugins/Payments.TenPay/Views/PaymentTenPay/PaymentInfo.cshtml");
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();

            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();

            return paymentInfo;
        }

        /// <summary>
        /// 原生支付 模式二
        /// 根据统一订单返回的code_url生成支付二维码。该模式链接较短，生成的二维码打印到结账小票上的识别率较高。
        /// 注意：code_url有效期为2小时，过期后扫码不能再发起支付
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Native(FormCollection form)
        {
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.TenPay") as TenPayPaymentProcessor;

            if (processor == null
                || !processor.IsPaymentMethodActive(_paymentSettings)
                || !processor.PluginDescriptor.Installed)
                throw new NopException("TenPay module cannot be loaded");

            //获取请求参数
            string orderGuid = Request.Form["OrderGuid"] == null ? string.Empty : Request.Form["OrderGuid"].ToString();
            if (string.IsNullOrWhiteSpace(orderGuid))
                throw new Exception("OrderGuid is not set");
            var order = _orderService.GetOrderByGuid(Guid.Parse(orderGuid));

            //创建支付应答对象
            RequestHandler packageReqHandler = new RequestHandler(null);

            var notifyUrl = _webHelper.GetStoreLocation(false) + "Plugins/PaymentTenPay/Notify";
            var nonceStr = TenPayV3Util.GetNoncestr();

            //创建请求统一订单接口参数
            packageReqHandler.SetParameter("appid", _tenPayPaymentSettings.AppId);
            packageReqHandler.SetParameter("mch_id", _tenPayPaymentSettings.MchId);
            packageReqHandler.SetParameter("nonce_str", nonceStr);
            packageReqHandler.SetParameter("body", "订单来自 " + _storeContext.CurrentStore.Name);
            packageReqHandler.SetParameter("out_trade_no", order.Id.ToString());
            packageReqHandler.SetParameter("total_fee", Convert.ToInt32(order.OrderTotal * 100).ToString());
            packageReqHandler.SetParameter("spbill_create_ip", Request.UserHostAddress);
            packageReqHandler.SetParameter("notify_url", notifyUrl);
            packageReqHandler.SetParameter("trade_type", TenPayV3Type.NATIVE.ToString());

            string sign = packageReqHandler.CreateMd5Sign("key", _tenPayPaymentSettings.Key);
            packageReqHandler.SetParameter("sign", sign);

            string data = packageReqHandler.ParseXML();

            //调用统一订单接口
            var result = TenPayV3.Unifiedorder(data);
            var returnValue = XDocument.Parse(result);
            var returnCode = returnValue.Element("xml").Element("return_code") == null ? string.Empty : returnValue.Element("xml").Element("return_code").Value;
            var resultCode = returnValue.Element("xml").Element("result_code") == null ? string.Empty : returnValue.Element("xml").Element("result_code").Value;
            var codeUrl = returnValue.Element("xml").Element("code_url") == null ? string.Empty : returnValue.Element("xml").Element("code_url").Value;
            if (!returnCode.Equals("SUCCESS") || !resultCode.Equals("SUCCESS") || String.IsNullOrWhiteSpace(codeUrl))
            {
                _logger.Error(string.Format("支付错误,微信返回信息：{0}", result));
                throw new NopException("支付错误");
            }

            //根据二维码链接生成二维码图片           
            BitMatrix bitMatrix;
            bitMatrix = new MultiFormatWriter().encode(codeUrl, BarcodeFormat.QR_CODE, 600, 600);
            BarcodeWriter bw = new BarcodeWriter();
            var ms = new MemoryStream();
            var bitmap = bw.Write(bitMatrix);
            bitmap.Save(ms, ImageFormat.Png);
            string codeImage = Convert.ToBase64String(ms.GetBuffer());
            ms.Close();

            //返回支付结果
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            var orderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
            var model = new NativePayModel
            {
                OrderTotal = orderTotal,
                QrCodeBase64String = codeImage
            };
            return View("~/Plugins/Payments.TenPay/Views/PaymentTenPay/Native.cshtml", model);
        }

        /// <summary>
        /// 支付通知
        /// </summary>
        /// <returns></returns>
        public ActionResult Notify()
        {
            //检查微信支付模块是否可用
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.TenPay") as TenPayPaymentProcessor;

            if (processor == null
                || !processor.IsPaymentMethodActive(_paymentSettings)
                || !processor.PluginDescriptor.Installed)
                throw new NopException("TenPay module cannot be loaded");

            //获取参数
            ResponseHandler resHandler = new ResponseHandler(null);
            string returnCode = resHandler.GetParameter("return_code");
            string returnMsg = resHandler.GetParameter("return_msg");
            resHandler.SetKey(_tenPayPaymentSettings.Key);

            string xml = @"<xml>
                            < return_code >< ![CDATA[{0}]]></ return_code >
                            < return_msg >< ![CDATA[{1}]]></ return_msg >
                           </ xml > ";
            //验证请求是否从微信发过来（安全）
            if (!resHandler.IsTenpaySign())
            {
                _logger.Error(String.Format("支付通知错误，ReturnCode:{0},ReturnMsg:{1}", returnCode, returnMsg));

                xml = String.Format(xml, "FAIL", "参数格式校验错误");
                return Content(xml, "text/xml");
            }

            //验证支付状态
            if (!returnCode.Equals("SUCCESS"))
            {
                _logger.Error(String.Format("支付通知错误，ReturnCode:{0},ReturnMsg:{1}", returnCode, returnMsg));

                xml = String.Format(xml, "FAIL", "支付状态错误");
                return Content(xml, "text/xml");
            }
            string resultCode = resHandler.GetParameter("result_code");
            if (!resultCode.Equals("SUCCESS"))
            {
                _logger.Error(String.Format("支付通知错误，ReturnCode:{0},ReturnMsg:{1},ResultCode:{2}", returnCode, returnMsg, resultCode));

                xml = String.Format(xml, "FAIL", "支付状态错误");
                return Content(xml, "text/xml");
            }

            //验证订单
            int orderId = Convert.ToInt32(resHandler.GetParameter("out_trade_no"));
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
            {
                xml = String.Format(xml, "FAIL", "订单查询失败");
                return Content(xml, "text/xml");
            }

            //修改订单状态
            if (_orderProcessingService.CanMarkOrderAsPaid(order))
            {
                string transactionId = resHandler.GetParameter("transaction_id");
                order.OrderNotes.Add(new OrderNote()
                {
                    Note = "微信支付订单号：" + transactionId,
                    DisplayToCustomer = true, //向客户展示微信支付的订单号
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderProcessingService.MarkOrderAsPaid(order);
            }
            xml = String.Format(xml, "SUCCESS", "OK");
            return Content(xml, "text/xml");
        }

        #region 其它支付模式(暂未实现)
        //public ActionResult JsApi(string code, string state)
        //{
        //    if (string.IsNullOrEmpty(code))
        //    {
        //        return Content("您拒绝了授权！");
        //    }

        //    if (!state.Contains("|"))
        //    {
        //        //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下
        //        //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
        //        return Content("验证失败！请从正规途径进入！1001");
        //    }

        //    //获取产品信息
        //    var stateData = state.Split('|');
        //    int productId = 0;
        //    ProductModel product = null;
        //    if (int.TryParse(stateData[0], out productId))
        //    {
        //        int hc = 0;
        //        if (int.TryParse(stateData[1], out hc))
        //        {
        //            var products = ProductModel.GetFakeProductList();
        //            product = products.FirstOrDefault(z => z.Id == productId);
        //            if (product == null || product.GetHashCode() != hc)
        //            {
        //                return Content("商品信息不存在，或非法进入！1002");
        //            }
        //            ViewData["product"] = product;
        //        }
        //    }

        //    //通过，用code换取access_token
        //    var openIdResult = OAuthApi.GetAccessToken(TenPayV3Info.AppId, TenPayV3Info.AppSecret, code);
        //    if (openIdResult.errcode != ReturnCode.请求成功)
        //    {
        //        return Content("错误：" + openIdResult.errmsg);
        //    }

        //    string timeStamp = "";
        //    string nonceStr = "";
        //    string paySign = "";

        //    string sp_billno = Request["order_no"];
        //    //当前时间 yyyyMMdd
        //    string date = DateTime.Now.ToString("yyyyMMdd");

        //    if (null == sp_billno)
        //    {
        //        //生成订单10位序列号，此处用时间和随机数生成，商户根据自己调整，保证唯一
        //        sp_billno = DateTime.Now.ToString("HHmmss") + TenPayV3Util.BuildRandomStr(28);
        //    }
        //    else
        //    {
        //        sp_billno = Request["order_no"].ToString();
        //    }

        //    //创建支付应答对象
        //    RequestHandler packageReqHandler = new RequestHandler(null);
        //    //初始化
        //    packageReqHandler.Init();

        //    timeStamp = TenPayV3Util.GetTimestamp();
        //    nonceStr = TenPayV3Util.GetNoncestr();

        //    //设置package订单参数
        //    packageReqHandler.SetParameter("appid", TenPayV3Info.AppId);		  //公众账号ID
        //    packageReqHandler.SetParameter("mch_id", TenPayV3Info.MchId);		  //商户号
        //    packageReqHandler.SetParameter("nonce_str", nonceStr);                    //随机字符串
        //    packageReqHandler.SetParameter("body", product == null ? "test" : product.Name);    //商品信息
        //    packageReqHandler.SetParameter("out_trade_no", sp_billno);		//商家订单号
        //    packageReqHandler.SetParameter("total_fee", product == null ? "100" : (product.Price * 100).ToString());			        //商品金额,以分为单位(money * 100).ToString()
        //    packageReqHandler.SetParameter("spbill_create_ip", Request.UserHostAddress);   //用户的公网ip，不是商户服务器IP
        //    packageReqHandler.SetParameter("notify_url", TenPayV3Info.TenPayV3Notify);		    //接收财付通通知的URL
        //    packageReqHandler.SetParameter("trade_type", TenPayV3Type.JSAPI.ToString());	                    //交易类型
        //    packageReqHandler.SetParameter("openid", openIdResult.openid);	                    //用户的openId

        //    string sign = packageReqHandler.CreateMd5Sign("key", TenPayV3Info.Key);
        //    packageReqHandler.SetParameter("sign", sign);	                    //签名

        //    string data = packageReqHandler.ParseXML();

        //    var result = TenPayV3.Unifiedorder(data);
        //    var res = XDocument.Parse(result);
        //    string prepayId = res.Element("xml").Element("prepay_id").Value;

        //    //设置支付参数
        //    RequestHandler paySignReqHandler = new RequestHandler(null);
        //    paySignReqHandler.SetParameter("appId", TenPayV3Info.AppId);
        //    paySignReqHandler.SetParameter("timeStamp", timeStamp);
        //    paySignReqHandler.SetParameter("nonceStr", nonceStr);
        //    paySignReqHandler.SetParameter("package", string.Format("prepay_id={0}", prepayId));
        //    paySignReqHandler.SetParameter("signType", "MD5");
        //    paySign = paySignReqHandler.CreateMd5Sign("key", TenPayV3Info.Key);

        //    ViewData["appId"] = TenPayV3Info.AppId;
        //    ViewData["timeStamp"] = timeStamp;
        //    ViewData["nonceStr"] = nonceStr;
        //    ViewData["package"] = string.Format("prepay_id={0}", prepayId);
        //    ViewData["paySign"] = paySign;

        //    return View();
        //}

        ///// <summary>
        ///// 原生支付 模式一
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Native()
        //{
        //    RequestHandler nativeHandler = new RequestHandler(null);
        //    string timeStamp = TenPayV3Util.GetTimestamp();
        //    string nonceStr = TenPayV3Util.GetNoncestr();

        //    //商品Id，用户自行定义
        //    string productId = DateTime.Now.ToString("yyyyMMddHHmmss");

        //    nativeHandler.SetParameter("appid", TenPayV3Info.AppId);
        //    nativeHandler.SetParameter("mch_id", TenPayV3Info.MchId);
        //    nativeHandler.SetParameter("time_stamp", timeStamp);
        //    nativeHandler.SetParameter("nonce_str", nonceStr);
        //    nativeHandler.SetParameter("product_id", productId);
        //    string sign = nativeHandler.CreateMd5Sign("key", TenPayV3Info.Key);

        //    var url = TenPayV3.NativePay(TenPayV3Info.AppId, timeStamp, TenPayV3Info.MchId, nonceStr, productId, sign);

        //    BitMatrix bitMatrix;
        //    bitMatrix = new MultiFormatWriter().encode(url, BarcodeFormat.QR_CODE, 600, 600);
        //    BarcodeWriter bw = new BarcodeWriter();

        //    var ms = new MemoryStream();
        //    var bitmap = bw.Write(bitMatrix);
        //    bitmap.Save(ms, ImageFormat.Png);
        //    //return File(ms, "image/png");
        //    ms.WriteTo(Response.OutputStream);
        //    Response.ContentType = "image/png";
        //    return null;
        //}

        //public ActionResult NativeNotifyUrl()
        //{
        //    ResponseHandler resHandler = new ResponseHandler(null);

        //    //返回给微信的请求
        //    RequestHandler res = new RequestHandler(null);

        //    string openId = resHandler.GetParameter("openid");
        //    string productId = resHandler.GetParameter("product_id");

        //    if (openId == null || productId == null)
        //    {
        //        res.SetParameter("return_code", "FAIL");
        //        res.SetParameter("return_msg", "回调数据异常");
        //    }

        //    //创建支付应答对象
        //    RequestHandler packageReqHandler = new RequestHandler(null);

        //    var sp_billno = DateTime.Now.ToString("HHmmss") + TenPayV3Util.BuildRandomStr(28);
        //    var nonceStr = TenPayV3Util.GetNoncestr();

        //    //创建请求统一订单接口参数
        //    packageReqHandler.SetParameter("appid", TenPayV3Info.AppId);
        //    packageReqHandler.SetParameter("mch_id", TenPayV3Info.MchId);
        //    packageReqHandler.SetParameter("nonce_str", nonceStr);
        //    packageReqHandler.SetParameter("body", "test");
        //    packageReqHandler.SetParameter("out_trade_no", sp_billno);
        //    packageReqHandler.SetParameter("total_fee", "1");
        //    packageReqHandler.SetParameter("spbill_create_ip", Request.UserHostAddress);
        //    packageReqHandler.SetParameter("notify_url", TenPayV3Info.TenPayV3Notify);
        //    packageReqHandler.SetParameter("trade_type", TenPayV3Type.NATIVE.ToString());
        //    packageReqHandler.SetParameter("openid", openId);
        //    packageReqHandler.SetParameter("product_id", productId);

        //    string sign = packageReqHandler.CreateMd5Sign("key", TenPayV3Info.Key);
        //    packageReqHandler.SetParameter("sign", sign);

        //    string data = packageReqHandler.ParseXML();

        //    try
        //    {
        //        //调用统一订单接口
        //        var result = TenPayV3.Unifiedorder(data);
        //        var unifiedorderRes = XDocument.Parse(result);
        //        string prepayId = unifiedorderRes.Element("xml").Element("prepay_id").Value;

        //        //创建应答信息返回给微信
        //        res.SetParameter("return_code", "SUCCESS");
        //        res.SetParameter("return_msg", "OK");
        //        res.SetParameter("appid", TenPayV3Info.AppId);
        //        res.SetParameter("mch_id", TenPayV3Info.MchId);
        //        res.SetParameter("nonce_str", nonceStr);
        //        res.SetParameter("prepay_id", prepayId);
        //        res.SetParameter("result_code", "SUCCESS");
        //        res.SetParameter("err_code_des", "OK");

        //        string nativeReqSign = res.CreateMd5Sign("key", TenPayV3Info.Key);
        //        res.SetParameter("sign", nativeReqSign);
        //    }
        //    catch (Exception)
        //    {
        //        res.SetParameter("return_code", "FAIL");
        //        res.SetParameter("return_msg", "统一下单失败");
        //    }

        //    return Content(res.ParseXML());
        //}

        ///// <summary>
        ///// 刷卡支付
        ///// </summary>
        ///// <param name="authCode">扫码设备获取到的微信用户刷卡授权码</param>
        ///// <returns></returns>
        //public ActionResult MicroPay(string authCode)
        //{
        //    RequestHandler payHandler = new RequestHandler(null);

        //    var sp_billno = DateTime.Now.ToString("HHmmss") + TenPayV3Util.BuildRandomStr(28);
        //    var nonceStr = TenPayV3Util.GetNoncestr();

        //    payHandler.SetParameter("auth_code", authCode);//授权码
        //    payHandler.SetParameter("body", "test");//商品描述
        //    payHandler.SetParameter("total_fee", "1");//总金额
        //    payHandler.SetParameter("out_trade_no", sp_billno);//产生随机的商户订单号
        //    payHandler.SetParameter("spbill_create_ip", Request.UserHostAddress);//终端ip
        //    payHandler.SetParameter("appid", TenPayV3Info.AppId);//公众账号ID
        //    payHandler.SetParameter("mch_id", TenPayV3Info.MchId);//商户号
        //    payHandler.SetParameter("nonce_str", nonceStr);//随机字符串

        //    string sign = payHandler.CreateMd5Sign("key", TenPayV3Info.Key);
        //    payHandler.SetParameter("sign", sign);//签名

        //    var result = TenPayV3.MicroPay(payHandler.ParseXML());

        //    //此处只是完成最简单的支付功能，实际情况还需要考虑各种出错的情况，并处理错误，最后返回结果通知用户。

        //    return Content(result);
        //}
        #endregion

        #endregion
    }
}
