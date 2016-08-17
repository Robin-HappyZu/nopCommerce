using Nop.Plugin.ExternalAuth.QQConnect.AspNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.QQConnect.Messaging
{
    public static class ErrorUtilities
    {
        internal static void VerifyArgument(bool condition, string message, params object[] args)
        {
            Requires.NotNull(args, "args");
            if (!condition)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, message, args));
            }
        }
    }
}
