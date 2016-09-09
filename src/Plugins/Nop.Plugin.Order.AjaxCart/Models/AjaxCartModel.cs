using Nop.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nop.Web.Models.ShoppingCart.ShoppingCartModel;

namespace Nop.Plugin.Order.AjaxCart.Models
{
    public class AjaxCartModel
    {
        public IList<ShoppingCartItemModel> Items { get; set; }

        public OrderTotalsModel OrderTotals { get; set; }

        public bool IsEditable { get; set; }

        public bool ShowSku { get; set; }

        public bool ShowProductImages { get; set; }
    }
}
