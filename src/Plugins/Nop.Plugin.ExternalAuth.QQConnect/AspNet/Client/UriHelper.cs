using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.QQConnect.AspNet.Client
{
    public static class UriHelper
    {
        public static string NormalizeHexEncoding(this string url)
        {
            var array = url.ToCharArray();
            for (var i = 0; i < array.Length - 2; i++)
            {
                if (array[i] != '%') continue;

                array[i + 1] = Char.ToUpperInvariant(array[i + 1]);
                array[i + 2] = Char.ToUpperInvariant(array[i + 2]);
                i += 2;
            }
            return new string(array);
        }
        public static string GetProviderNameFromQueryString(NameValueCollection queryString)
        {
            var result = queryString["__provider__"];
            //commented out stuff
            return result;
        }
    }
}
