using Nop.Plugin.ExternalAuth.QQConnect.AspNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.QQConnect.Messaging
{
    public static class MessagingUtilities
    {
        public static void AppendQueryArgs(this UriBuilder builder, IEnumerable<KeyValuePair<string, string>> args)
        {
            Requires.NotNull(builder, "builder");
            if (args == null) return;

            var enumerable = args.ToList();
            if (!enumerable.Any()) return;

            var stringBuilder = new StringBuilder(50 + enumerable.Count() * 10);
            if (!string.IsNullOrEmpty(builder.Query))
            {
                stringBuilder.Append(builder.Query.Substring(1));
                stringBuilder.Append('&');
            }
            stringBuilder.Append(CreateQueryString(enumerable));
            builder.Query = stringBuilder.ToString();
        }

        public static string CreateQueryString(IEnumerable<KeyValuePair<string, string>> args)
        {
            var enumerable = args.ToList();
            Requires.NotNull(enumerable, "args");
            if (!enumerable.Any()) return string.Empty;

            var stringBuilder = new StringBuilder(enumerable.Count() * 10);
            foreach (var current in enumerable)
            {
                ErrorUtilities.VerifyArgument(!string.IsNullOrEmpty(current.Key), "UnexpectedNullOrEmpty", new object[0]);
                ErrorUtilities.VerifyArgument(current.Value != null, "UnexpectedNullValue", new object[]
                {
                    current.Key
                });
                stringBuilder.Append(EscapeUriDataStringRfc3986(current.Key));
                stringBuilder.Append('=');
                stringBuilder.Append(EscapeUriDataStringRfc3986(current.Value));
                stringBuilder.Append('&');
            }
            stringBuilder.Length--;
            return stringBuilder.ToString();
        }

        public static string EscapeUriDataStringRfc3986(string value)
        {
            Requires.NotNull(value, "value");

            var stringBuilder = new StringBuilder(Uri.EscapeDataString(value));
            foreach (var item in UriRfc3986CharsToEscape)
            {
                stringBuilder.Replace(item, Uri.HexEscape(item[0]));
            }
            return stringBuilder.ToString();
        }

        public static readonly string[] UriRfc3986CharsToEscape =
        {
            "!", 
            "*", 
            "'", 
            "(", 
            ")"
        };
    }
}
