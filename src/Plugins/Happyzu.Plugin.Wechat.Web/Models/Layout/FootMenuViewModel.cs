using System.Collections.Generic;

namespace Happyzu.Plugin.Wechat.Web.Models.Layout
{
    public class FootMenuViewModel
    {
        public UserMenu MainMenu { get; set; }

        public string ActiveMenuItemName { get; set; }

        public int ShoppingCartItemCount { get; set; }
    }

    public class UserMenu
    {
        public UserMenu()
        {
            Items=new List<UserMenuItem>();
        }

        public object CustomData { get; set; }

        public string DisplayName { get; set; }

        public IList<UserMenuItem> Items { get; set; }

        public string Name { get; set; }
    }

    public class UserMenuItem
    {
        public UserMenuItem()
        {
            Items=new List<UserMenuItem>();
        }

        public object CustomData { get; set; }

        public string DisplayName { get; }

        public string Icon { get; set; }

        public IList<UserMenuItem> Items { get; }

        public string Name { get; set; }

        public int Order { get; set; }

        public string Url { get; set; }
    }
}