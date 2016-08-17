using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.QQConnect.AspNet
{
    public static class Requires
    {
        [DebuggerStepThrough]
        internal static T NotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            return value;
        }

        [DebuggerStepThrough]
        internal static string NotNullOrEmpty(string value, string parameterName)
        {
            NotNull(value, parameterName);
            True(value.Length > 0, parameterName, "EmptyStringNotAllowed");
            return value;
        }

        [DebuggerStepThrough]
        internal static void NotNullOrEmpty<T>(IEnumerable<T> value, string parameterName)
        {
            var enumerable = value as IList<T> ?? value.ToList();
            NotNull(enumerable, parameterName);
            True(enumerable.Any(), parameterName, "InvalidArgument");
        }

        [DebuggerStepThrough]
        internal static void NullOrWithNoNullElements<T>(IEnumerable<T> sequence, string parameterName) where T : class
        {
            if (sequence == null) return;
            if (sequence.Any((e => e == null)))
            {
                throw new ArgumentException("SequenceContainsNullElement", parameterName);
            }
        }

        [DebuggerStepThrough]
        internal static void InRange(bool condition, string parameterName, string message = null)
        {
            if (!condition)
            {
                throw new ArgumentOutOfRangeException(parameterName, message);
            }
        }

        [DebuggerStepThrough]
        internal static void True(bool condition, string parameterName = null, string message = null)
        {
            if (!condition)
            {
                throw new ArgumentException(message ?? "InvalidArgument", parameterName);
            }
        }

        [DebuggerStepThrough]
        internal static void True(bool condition, string parameterName, string unformattedMessage, params object[] args)
        {
            if (!condition)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, unformattedMessage, args),
                    parameterName);
            }
        }

        [DebuggerStepThrough]
        internal static void ValidState(bool condition)
        {
            if (!condition)
            {
                throw new InvalidOperationException();
            }
        }

        [DebuggerStepThrough]
        internal static void ValidState(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        [DebuggerStepThrough]
        internal static void ValidState(bool condition, string unformattedMessage, params object[] args)
        {
            if (!condition)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, unformattedMessage, args));
            }
        }

        //[DebuggerStepThrough]
        //internal static void NotNullSubtype<T>(Type type, string parameterName)
        //{
        //    NotNull(type, parameterName);
        //    True(typeof (T).IsAssignableFrom(type), parameterName, MessagingStrings.UnexpectedType, new object[]
        //    {
        //        typeof (T).FullName,
        //        type.FullName
        //    });
        //}

        [DebuggerStepThrough]
        internal static void Format(bool condition, string message)
        {
            if (!condition)
            {
                throw new FormatException(message);
            }
        }

        [DebuggerStepThrough]
        internal static void Support(bool condition, string message)
        {
            if (!condition)
            {
                throw new NotSupportedException(message);
            }
        }

        [DebuggerStepThrough]
        internal static void Fail(string parameterName, string message)
        {
            throw new ArgumentException(message, parameterName);
        }
    }
}
