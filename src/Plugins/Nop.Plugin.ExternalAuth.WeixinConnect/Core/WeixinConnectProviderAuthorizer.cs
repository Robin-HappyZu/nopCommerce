using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.AspNet;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication.External;
using Senparc.Weixin;
using Senparc.Weixin.Open.QRConnect;
using Senparc.Weixin.Open;

namespace Nop.Plugin.ExternalAuth.WeixinConnect.Core
{
    public class WeixinConnectProviderAuthorizer : IOAuthProviderWeixinConnectAuthorizer
    {
        private readonly IExternalAuthorizer _authorizer;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly WeixinConnectExternalAuthSettings _weixinConnectExternalAuthSettings;
        private readonly HttpContextBase _httpContext;
        private readonly IWebHelper _webHelper;

        public WeixinConnectProviderAuthorizer(IExternalAuthorizer authorizer,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            WeixinConnectExternalAuthSettings weixinConnectExternalAuthSettings,
            HttpContextBase httpContext,
            IWebHelper webHelper)
        {
            this._authorizer = authorizer;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._weixinConnectExternalAuthSettings = weixinConnectExternalAuthSettings;
            this._httpContext = httpContext;
            this._webHelper = webHelper;
        }

        private AuthorizeState VerifyAuthentication(string returnUrl)
        {
            string code = _httpContext.Request.QueryString["code"];

            if (string.IsNullOrEmpty(code))
            {
                var state = new AuthorizeState(returnUrl, OpenAuthenticationStatus.Error);
                state.AddError("Code为空");
                return state;
            }

            var tokenResult = QRConnectAPI.GetAccessToken(_weixinConnectExternalAuthSettings.ClientKeyIdentifier,
                _weixinConnectExternalAuthSettings.ClientSecret, code);

            if (tokenResult.errcode != ReturnCode.请求成功)
            {
                // 请求token失败
                var state = new AuthorizeState(returnUrl, OpenAuthenticationStatus.Error);
                state.AddError("请求Token失败");
                return state;
            }

            var userInfo = QRConnectAPI.GetUserInfo(tokenResult.access_token, tokenResult.openid);

            var parameters = new OAuthAuthenticationParameters(Provider.SystemName)
            {
                ExternalIdentifier = tokenResult.openid,
                OAuthToken = tokenResult.access_token,
                OAuthAccessToken = tokenResult.openid,
            };

            if (_externalAuthenticationSettings.AutoRegisterEnabled)
                ParseClaims(tokenResult, userInfo, parameters);

            var result = _authorizer.Authorize(parameters);
            return new AuthorizeState(returnUrl, result);
        }

        private void ParseClaims(QRConnectAccessTokenResult tokenResult, QRConnectUserInfo userInfo, OAuthAuthenticationParameters parameters)
        {
            var claims = new UserClaims();
            claims.Contact = new ContactClaims();
            claims.Name = new NameClaims();

            if (!string.IsNullOrEmpty(userInfo.openid))
            {
                claims.Contact.Email = "WX" + userInfo.openid + "@" + _webHelper.GetStoreLocation().ToLower().Replace("www.", "").Replace("http://", "").Replace("/", "");
            }

            if (!string.IsNullOrEmpty(userInfo.nickname))
            {
                claims.Name.Nickname = userInfo.nickname;
            }

            parameters.AddClaim(claims);
        }

        private Uri GenerateLocalCallbackUri()
        {
            string url = string.Format("{0}plugins/externalauthWeixinConnect/logincallback/", _webHelper.GetStoreLocation());
            return new Uri(url);
        }

        private AuthorizeState RequestAuthentication()
        {
            var redirectUrl = GenerateLocalCallbackUri().AbsoluteUri;
            var scopes = new[] { OAuthScope.snsapi_login };
            var authUrl = QRConnectAPI.GetQRConnectUrl(_weixinConnectExternalAuthSettings.ClientKeyIdentifier,
                redirectUrl, String.Empty, scopes);
            return new AuthorizeState("", OpenAuthenticationStatus.RequiresRedirect) { Result = new RedirectResult(authUrl) };
        }
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="verifyResponse"></param>
        /// <returns></returns>
        public AuthorizeState Authorize(string returnUrl, bool? verifyResponse = null)
        {
            if (!verifyResponse.HasValue)
                throw new ArgumentException("WeixinConnect plugin cannot automatically determine verifyResponse property");

            if (verifyResponse.Value)
                return VerifyAuthentication(returnUrl);

            return RequestAuthentication();
        }
    }
}
