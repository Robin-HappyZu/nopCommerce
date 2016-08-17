using System.IO;
using System.Runtime.Serialization.Json;

namespace Nop.Plugin.ExternalAuth.WeiXin.AspNet
{
    internal static class JsonHelper
    {
        public static T Deserialize<T>(Stream stream) where T : class
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }
    }
}
