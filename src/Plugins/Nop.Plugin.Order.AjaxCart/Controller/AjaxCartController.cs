using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Order.AjaxCart.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.ShoppingCart;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Plugin.Order.AjaxCart.Controller
{
    public partial class AjaxCartController : ShoppingCartController
    {

        #region Fields
        
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Constructors

        public AjaxCartController(IProductService productService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IProductAttributeService productAttributeService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            ITaxService taxService, ICurrencyService currencyService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IOrderProcessingService orderProcessingService,
            IDiscountService discountService,
            ICustomerService customerService,
            IGiftCardService giftCardService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShippingService shippingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICheckoutAttributeService checkoutAttributeService,
            IPaymentService paymentService,
            IWorkflowMessageService workflowMessageService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            ICacheManager cacheManager,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            HttpContextBase httpContext,
            MediaSettings mediaSettings,
            ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings,
            OrderSettings orderSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            CaptchaSettings captchaSettings,
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            CustomerSettings customerSettings) : base(productService,
            storeContext,
            workContext,
            shoppingCartService,
            pictureService,
            localizationService,
            productAttributeService,
            productAttributeFormatter,
            productAttributeParser,
            taxService, currencyService,
            priceCalculationService,
            priceFormatter,
            checkoutAttributeParser,
            checkoutAttributeFormatter,
            orderProcessingService,
            discountService,
            customerService,
            giftCardService,
            countryService,
            stateProvinceService,
            shippingService,
            orderTotalCalculationService,
            checkoutAttributeService,
            paymentService,
            workflowMessageService,
            permissionService,
            downloadService,
            cacheManager,
            webHelper,
            customerActivityService,
            genericAttributeService,
            addressAttributeFormatter,
            httpContext,
            mediaSettings,
            shoppingCartSettings,
            catalogSettings,
            orderSettings,
            shippingSettings,
            taxSettings,
            captchaSettings,
            addressSettings,
            rewardPointsSettings,
            customerSettings)
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _permissionService = permissionService;
        }

        #endregion

         
        public ActionResult PublicInfo(string widgetZone,object additionalData=null)
        {
            return View();
        }

        /// <summary>
        /// Ajax Load Cart
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadCart()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //ShoppingCart
            var shoppingCart = new ShoppingCartModel();
            PrepareShoppingCartModel(shoppingCart, cart);

            //OrderTotals
            var orderTotals = PrepareOrderTotalsModel(cart, false);
            
            var model = new AjaxCartModel() {
                OrderTotals = orderTotals,
                IsEditable = shoppingCart.IsEditable,
                Items = shoppingCart.Items,
                ShowProductImages = shoppingCart.ShowProductImages,
                ShowSku = shoppingCart.ShowSku
            };
            return Json(model,JsonRequestBehavior.AllowGet);
        }
    }
}
