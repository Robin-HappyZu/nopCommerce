using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.QQConnect.AspNet.Client
{
    internal static class JsonHelper
    {
        public static T Deserialize<T>(Stream stream) where T : class
        {
            Requires.NotNull<Stream>(stream, "stream");
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }
    }
}
