using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json;
using Nop.Plugin.ExternalAuth.QQConnect.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.QQConnect.AspNet.Client
{
    public sealed class QQConnectClient : OAuth2Client
    {
        private const string AuthorizationEndpoint = "https://graph.qq.com/oauth2.0/authorize";
        private const string TokenEndpoint = "https://graph.qq.com/oauth2.0/token";

        private readonly string _appId;
        private readonly string _appSecret;
        private readonly string _returnUrl;

        public QQConnectClient(string appId, string appSecret, string returnUrl = null)
            : base("QQConnect")
        {
            Requires.NotNullOrEmpty(appId, "appId");
            Requires.NotNullOrEmpty(appSecret, "appSecret");

            this._appId = appId;
            this._appSecret = appSecret;
            _returnUrl = (returnUrl == null) ? null : new Uri(returnUrl).AbsoluteUri.NormalizeHexEncoding();
        }
        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            var uriBuilder = new UriBuilder(AuthorizationEndpoint);
            var uri = _returnUrl ?? returnUrl.AbsoluteUri.NormalizeHexEncoding();
            uriBuilder.AppendQueryArgs(new Dictionary<string, string>
            {
                {
                    "client_id",
                    _appId
                },
                {
                    "response_type",
                    "code"
                },
                {
                    "redirect_uri",
                    uri
                },
                {
                    "state",
                     Guid.NewGuid().ToString().Replace("-", "")
                },
                 {
                    "scope",
                     "get_user_info"
                }
            });
            return uriBuilder.Uri;
        }
        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            var uriBuilder = new UriBuilder(TokenEndpoint);
            var uri = _returnUrl ?? returnUrl.AbsoluteUri.NormalizeHexEncoding();
            uriBuilder.AppendQueryArgs(new Dictionary<string, string>
            {
                {
                    "grant_type",
                    "authorization_code"
                },

                {
                    "code",
                    authorizationCode
                },
                
                {
                    "client_id",
                    _appId
                },

                {
                    "client_secret",
                    _appSecret
                },
                
                {
                    "redirect_uri",
                    uri
                },
            });
            string result;
            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadString(uriBuilder.Uri);

                //调试专用
                //Nop.Core.Infrastructure.EngineContext.Current.Resolve<Nop.Services.Logging.ILogger>().InsertLog(Nop.Core.Domain.Logging.LogLevel.Information, WebHelper.GetParmList(json)["access_token"] + "", json + "", null);
                
                if (string.IsNullOrEmpty(json))
                {
                    result = null;
                }
                else
                {
                    result = WebHelper.GetParmList(json)["access_token"];
                }
            }
            return result;
        }
        protected override IDictionary<string, string> GetOpenIdData(string accessToken)
        {
            QQConnectOpenIdData data;

            var uriBuilder = new UriBuilder("https://graph.qq.com/oauth2.0/me");
            uriBuilder.AppendQueryArgs(new Dictionary<string, string>
            {
                {
                    "access_token",
                    accessToken
                }
            });

            using (WebResponse responseOpenID = WebRequest.Create(uriBuilder.Uri).GetResponse())
            {
                using (Stream streamOpenID = responseOpenID.GetResponseStream())
                {
                    string result = new StreamReader(streamOpenID).ReadToEnd();
                    result = result.Replace("callback", "").Replace("(", "").Replace(")", "").Replace(";", "");

                    //调试专用
                    //Nop.Core.Infrastructure.EngineContext.Current.Resolve<Nop.Services.Logging.ILogger>().InsertLog(Nop.Core.Domain.Logging.LogLevel.Information, result, result, null);

                    byte[] array = Encoding.UTF8.GetBytes(result);
                    MemoryStream memoryStream = new MemoryStream(array);

                    data = JsonHelper.Deserialize<QQConnectOpenIdData>(memoryStream);
                }
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.AddItemIfNotEmpty("client_id", data.Client_Id);
                dictionary.AddItemIfNotEmpty("openid", data.OpenId);
                return dictionary;
            }
        }
        protected override IDictionary<string, string> GetUserData(IDictionary<string, string> openIdData, string accessToken)
        {
            QQConnectGraphData data;

            var uriBuilder = new UriBuilder("https://graph.qq.com/user/get_user_info");
            uriBuilder.AppendQueryArgs(new Dictionary<string, string>
            {
                {
                    "access_token",
                    accessToken
                },
                {
                    "openid",
                    openIdData["openid"]
                },
                {
                    "oauth_consumer_key",
                    openIdData["client_id"]
                },
            });

            using (WebResponse response = WebRequest.Create(uriBuilder.Uri).GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    string result = new StreamReader(stream).ReadToEnd();
                    result = result.Replace(@"\", "");

                    //调试专用
                    //Nop.Core.Infrastructure.EngineContext.Current.Resolve<Nop.Services.Logging.ILogger>().InsertLog(Nop.Core.Domain.Logging.LogLevel.Information, result + "", "", null);

                    byte[] array = Encoding.UTF8.GetBytes(result);
                    MemoryStream memoryStream = new MemoryStream(array);

                    data = JsonHelper.Deserialize<QQConnectGraphData>(memoryStream);
                }
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.AddItemIfNotEmpty("nickname", data.Nickname);
            dictionary.AddItemIfNotEmpty("figureurl", data.Figureurl);
            dictionary.AddItemIfNotEmpty("figureurl_1", data.Figureurl_1);
            dictionary.AddItemIfNotEmpty("figureurl_2", data.Figureurl_2);
            dictionary.AddItemIfNotEmpty("figureurl_qq_1", data.Figureurl_qq_1);
            dictionary.AddItemIfNotEmpty("figureurl_qq_2", data.Figureurl_qq_2);
            dictionary.AddItemIfNotEmpty("gender", data.Gender);
            dictionary.AddItemIfNotEmpty("is_yellow_vip", data.Is_Yellow_Vip);
            dictionary.AddItemIfNotEmpty("vip", data.Vip);
            dictionary.AddItemIfNotEmpty("yellow_vip_level", data.Yellow_Vip_Level);
            dictionary.AddItemIfNotEmpty("level", data.Level);
            dictionary.AddItemIfNotEmpty("is_yellow_year_vip", data.Is_Yellow_Year_Vip);
            return dictionary;
        }
    }
}
