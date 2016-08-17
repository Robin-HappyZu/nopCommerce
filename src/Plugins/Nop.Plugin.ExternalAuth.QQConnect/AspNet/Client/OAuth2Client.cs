using DotNetOpenAuth.AspNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nop.Plugin.ExternalAuth.QQConnect.AspNet.Client
{
    public abstract class OAuth2Client : IAuthenticationClient
    {
        private readonly string providerName;
        protected OAuth2Client(string providerName)
        {
            Requires.NotNull<string>(providerName, "providerName");
            this.providerName = providerName;
        }
        protected abstract Uri GetServiceLoginUrl(Uri returnUrl);
        protected abstract IDictionary<string, string> GetOpenIdData(string accessToken);
        protected abstract IDictionary<string, string> GetUserData(IDictionary<string, string> openIdData, string accessToken);
        protected abstract string QueryAccessToken(Uri returnUrl, string authorizationCode);
        public virtual void RequestAuthentication(HttpContextBase context, Uri returnUrl)
        {
            Requires.NotNull<HttpContextBase>(context, "context");
            Requires.NotNull<Uri>(returnUrl, "returnUrl");
            string absoluteUri = this.GetServiceLoginUrl(returnUrl).AbsoluteUri;
            context.Response.Redirect(absoluteUri, true);
        }
        public AuthenticationResult VerifyAuthentication(HttpContextBase context)
        {
            throw new InvalidOperationException("OAuthRequireReturnUrl");
        }
        public virtual AuthenticationResult VerifyAuthentication(HttpContextBase context, Uri returnPageUrl)
        {
            string userName;
            Requires.NotNull<HttpContextBase>(context, "context");
            string str = context.Request.QueryString["code"];

            if (string.IsNullOrEmpty(str))
            {
                return AuthenticationResult.Failed;
            }
            string accessToken = this.QueryAccessToken(returnPageUrl, str);
            if (accessToken == null)
            {
                return AuthenticationResult.Failed;
            }

            IDictionary<string, string> openIdData = this.GetOpenIdData(accessToken);

            if (openIdData == null)
            {
                return AuthenticationResult.Failed;
            }
            IDictionary<string, string> userData = this.GetUserData(openIdData, accessToken);
            if (userData == null)
            {
                return AuthenticationResult.Failed;
            }

            string providerUserId = openIdData["openid"];
            if (!userData.TryGetValue("nickname", out userName))
            {
                userName = providerUserId;
            }
            userData.AddItemIfNotEmpty("accesstoken", accessToken);
            userData.AddItemIfNotEmpty("id", openIdData["openid"]);

            return new AuthenticationResult(true, this.ProviderName, providerUserId, userName, userData);
        }
        public string ProviderName
        {
            get
            {
                return this.providerName;
            }
        }
    }
}
