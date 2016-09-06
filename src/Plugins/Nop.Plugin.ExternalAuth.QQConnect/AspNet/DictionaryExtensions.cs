using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nop.Plugin.ExternalAuth.QQConnect.AspNet
{
    internal static class DictionaryExtensions
    {
        public static void AddDataIfNotEmpty(this Dictionary<string, string> dictionary, XDocument document, string elementName)
        {
            XElement element = document.Root.Element(elementName);
            if (element != null)
            {
                dictionary.AddItemIfNotEmpty(elementName, element.Value);
            }
        }
        public static void AddItemIfNotEmpty(this IDictionary<string, string> dictionary, string key, string value)
        {

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (!string.IsNullOrEmpty(value))
            {
                dictionary[key] = value;
            }
        }
    }
}
