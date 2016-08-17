using System;
using System.Collections.Specialized;

namespace Nop.Plugin.ExternalAuth.WeiXin
{
    internal class WebHelper
    {
        /// <summary>
        /// 获得参数列表
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static NameValueCollection GetParmList(string data)
        {
            var parmList = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(data))
            {
                var length = data.Length;
                for (var i = 0; i < length; i++)
                {
                    var startIndex = i;
                    var endIndex = -1;
                    while (i < length)
                    {
                        var c = data[i];
                        if (c == '=')
                        {
                            if (endIndex < 0)
                                endIndex = i;
                        }
                        else if (c == '&')
                        {
                            break;
                        }
                        i++;
                    }
                    string key;
                    string value;
                    if (endIndex >= 0)
                    {
                        key = data.Substring(startIndex, endIndex - startIndex);
                        value = data.Substring(endIndex + 1, (i - endIndex) - 1);
                    }
                    else
                    {
                        key = data.Substring(startIndex, i - startIndex);
                        value = string.Empty;
                    }
                    parmList[key] = value;
                    if ((i == (length - 1)) && (data[i] == '&'))
                        parmList[key] = string.Empty;
                }
            }
            return parmList;
        }
    }
}