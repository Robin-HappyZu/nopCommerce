using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using DotNetOpenAuth.AspNet;
using Nop.Plugin.ExternalAuth.WeiXin.Core;

namespace Nop.Plugin.ExternalAuth.WeiXin.AspNet
{
    public class WeiXinClient : OAuth2Client
    {
        private const string AuthorizationEndpoint = "";
        private const string TokenEndpoint = "";
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly string _returnUrl;

        public WeiXinClient(string appId, string appSecret, string returnUrl = null) : base(Provider.SystemName)
        {
            _appId = appId;
            _appSecret = appSecret;
            _returnUrl = string.IsNullOrEmpty(returnUrl) ? null : new Uri(returnUrl).AbsoluteUri.Normalize();
        }

        public string ProviderName { get; }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            var uriBuilder = new UriBuilder(AuthorizationEndpoint);
            var uri = _returnUrl ?? returnUrl.AbsoluteUri.Normalize();
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
                    Guid.NewGuid().ToString("N")
                },
                {
                    "scope",
                    "get_user_info"
                }
            });
            return uriBuilder.Uri;
        }

        protected override IDictionary<string, string> GetOpenIdData(string accessToken)
        {
            WeiXinOpenIdData data;

            var uriBuilder = new UriBuilder("https://graph.qq.com/oauth2.0/me");
            uriBuilder.AppendQueryArgs(new Dictionary<string, string>
            {
                {
                    "access_token",
                    accessToken
                }
            });

            using (var responseOpenID = WebRequest.Create(uriBuilder.Uri).GetResponse())
            {
                using (var streamOpenID = responseOpenID.GetResponseStream())
                {
                    var result = new StreamReader(streamOpenID).ReadToEnd();
                    result = result.Replace("callback", "").Replace("(", "").Replace(")", "").Replace(";", "");

                    //调试专用
                    //Nop.Core.Infrastructure.EngineContext.Current.Resolve<Nop.Services.Logging.ILogger>().InsertLog(Nop.Core.Domain.Logging.LogLevel.Information, result, result, null);

                    var array = Encoding.UTF8.GetBytes(result);
                    var memoryStream = new MemoryStream(array);

                    data = JsonHelper.Deserialize<WeiXinOpenIdData>(memoryStream);
                }

                var dictionary = new Dictionary<string, string>();
                //dictionary.AddItemIfNotEmpty("client_id", data.Client_Id);
                dictionary.AddItemIfNotEmpty("openid", data.OpenId);
                return dictionary;
            }
        }

        protected override IDictionary<string, string> GetUserData(IDictionary<string, string> openIdData,
            string accessToken)
        {
            WeiXinGraphData data;

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
                }
            });

            using (var response = WebRequest.Create(uriBuilder.Uri).GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    var result = new StreamReader(stream).ReadToEnd();
                    result = result.Replace(@"\", "");

                    //调试专用
                    //Nop.Core.Infrastructure.EngineContext.Current.Resolve<Nop.Services.Logging.ILogger>().InsertLog(Nop.Core.Domain.Logging.LogLevel.Information, result + "", "", null);

                    var array = Encoding.UTF8.GetBytes(result);
                    var memoryStream = new MemoryStream(array);

                    data = JsonHelper.Deserialize<WeiXinGraphData>(memoryStream);
                }
            }

            var dictionary = new Dictionary<string, string>();
            dictionary.AddItemIfNotEmpty("nickname", data.NickName);
            dictionary.AddItemIfNotEmpty("gender", data.Gender);
            return dictionary;
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            var uriBuilder = new UriBuilder(TokenEndpoint);
            var uri = _returnUrl ?? returnUrl.AbsoluteUri.Normalize();
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
                }
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
    }
}