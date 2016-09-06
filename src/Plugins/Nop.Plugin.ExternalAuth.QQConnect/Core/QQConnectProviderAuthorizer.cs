using DotNetOpenAuth.AspNet;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Plugin.ExternalAuth.QQConnect.AspNet.Client;
using Nop.Services.Authentication.External;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Nop.Plugin.ExternalAuth.QQConnect.Core
{
    public class QQConnectProviderAuthorizer : IOAuthProviderQQConnectAuthorizer
    {
        #region Fields

        private readonly IExternalAuthorizer _authorizer;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly HttpContextBase _httpContext;
        private readonly IWebHelper _webHelper;
        private readonly QQConnectExternalAuthSettings _conntectExternalAuthSettings;
        private QQConnectClient _qqConnectApplication;
        private readonly ILogger _logService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public QQConnectProviderAuthorizer(IExternalAuthorizer authorizer,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            HttpContextBase httpContext,
            IWebHelper webHelper,
            QQConnectExternalAuthSettings conntectExternalAuthSettings,
            ILogger logService,
            IWorkContext workContext
            )
        {
            this._authorizer = authorizer;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._httpContext = httpContext;
            this._webHelper = webHelper;
            this._conntectExternalAuthSettings = conntectExternalAuthSettings;
            this._logService = logService;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        private QQConnectClient QQConnectApplication
        {
            get { return _qqConnectApplication ?? (_qqConnectApplication = new QQConnectClient(_conntectExternalAuthSettings.AppId, _conntectExternalAuthSettings.AppKey)); }
        }

        private AuthorizeState VerifyAuthentication(string returnUrl)
        {
            var authResult = this.QQConnectApplication.VerifyAuthentication(_httpContext, GenerateLocalCallbackUri());

            if (authResult.IsSuccessful)
            {
                if (!authResult.ExtraData.ContainsKey("id"))
                    throw new Exception("Authentication result does not contain id data");

                if (!authResult.ExtraData.ContainsKey("accesstoken"))
                    throw new Exception("Authentication result does not contain accesstoken data");

                var parameters = new OAuthAuthenticationParameters(Provider.SystemName)
                {
                    ExternalIdentifier = authResult.ProviderUserId,
                    OAuthToken = authResult.ExtraData["accesstoken"],
                    OAuthAccessToken = authResult.ProviderUserId,
                };

                ParseClaims(authResult, parameters);

                var result = _authorizer.Authorize(parameters);
                return new AuthorizeState(returnUrl, result);
            }

            var state = new AuthorizeState(returnUrl, OpenAuthenticationStatus.Error);
            var error = authResult.Error != null ? authResult.Error.Message : "Unknown error";
            state.AddError(error);
            return state;
        }

        private void ParseClaims(AuthenticationResult authenticationResult, OAuthAuthenticationParameters parameters)
        {
            //通过NameClaims对象存储QQ互联返回的用户昵称
            var claims = new UserClaims();
            claims.Name = new NameClaims();
            claims.Contact = new ContactClaims();
            if (authenticationResult.ExtraData.ContainsKey("id"))
            {
                claims.Contact.Email = "QQ" + authenticationResult.ExtraData["id"] + "@" + _webHelper.GetStoreLocation().ToLower().Replace("www.", "").Replace("http://", "").Replace("/", "");
            }

            if (authenticationResult.ExtraData.ContainsKey("nickname"))
            {
                claims.Name.Nickname = authenticationResult.ExtraData["nickname"];
            }
        
            parameters.AddClaim(claims);
        }

        private AuthorizeState RequestAuthentication(string returnUrl)
        {
            var authUrl = GenerateServiceLoginUrl().AbsoluteUri;
            return new AuthorizeState("", OpenAuthenticationStatus.RequiresRedirect) { Result = new RedirectResult(authUrl) };
        }

        private Uri GenerateLocalCallbackUri()
        {
            string url = string.Format("{0}plugins/externalAuthQQConnect/logincallback/", _webHelper.GetStoreLocation());
            return new Uri(url);
        }

        private Uri GenerateServiceLoginUrl()
        {
            var builder = new UriBuilder("https://graph.qq.com/oauth2.0/authorize");
            string salt = Guid.NewGuid().ToString().Replace("-", "");
            _httpContext.Session[_workContext.CurrentCustomer.Id.ToString()] = salt;
            var args = new Dictionary<string, string>();
            args.Add("client_id", _conntectExternalAuthSettings.AppId);
            args.Add("response_type", "code");
            args.Add("redirect_uri", GenerateLocalCallbackUri().AbsoluteUri.ToLower());
            args.Add("state", salt);
            args.Add("scope", "get_user_info");
            AppendQueryArgs(builder, args);

            return builder.Uri;
        }

        private void AppendQueryArgs(UriBuilder builder, IEnumerable<KeyValuePair<string, string>> args)
        {
            if ((args != null) && (args.Count<KeyValuePair<string, string>>() > 0))
            {
                StringBuilder builder2 = new StringBuilder(50 + (args.Count<KeyValuePair<string, string>>() * 10));
                if (!string.IsNullOrEmpty(builder.Query))
                {
                    builder2.Append(builder.Query.Substring(1));
                    builder2.Append('&');
                }
                builder2.Append(CreateQueryString(args));
                builder.Query = builder2.ToString();
            }
        }
        private string CreateQueryString(IEnumerable<KeyValuePair<string, string>> args)
        {
            if (!args.Any<KeyValuePair<string, string>>())
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder(args.Count<KeyValuePair<string, string>>() * 10);
            foreach (KeyValuePair<string, string> pair in args)
            {
                builder.Append(EscapeUriDataStringRfc3986(pair.Key));
                builder.Append('=');
                builder.Append(EscapeUriDataStringRfc3986(pair.Value));
                builder.Append('&');
            }
            builder.Length--;
            return builder.ToString();
        }

        private readonly string[] UriRfc3986CharsToEscape = new string[] { "!", "*", "'", "(", ")" };
        private string EscapeUriDataStringRfc3986(string value)
        {
            StringBuilder builder = new StringBuilder(Uri.EscapeDataString(value));
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                builder.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }
            return builder.ToString();
        }

        #endregion

        public AuthorizeState Authorize(string returnUrl, bool? verifyResponse = null)
        {
            if (!verifyResponse.HasValue)
                throw new ArgumentException("QQ connect plugin cannot automatically determine verifyResponse property");

            if (verifyResponse.Value)
            {
                return VerifyAuthentication(returnUrl);
            }
            else
            {
                return RequestAuthentication(returnUrl);
            }
        }
    }
}
