using Nop.Services.Authentication.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.QQConnect.Authentication.External
{
    public struct RegistrationDetails
    {
        /// <summary>
        /// 获取授权id昵称
        /// </summary>
        /// <param name="parameters"></param>
        public RegistrationDetails(OpenAuthenticationParameters parameters)
            : this()
        {
            if (parameters.UserClaims != null)
                foreach (var claim in parameters.UserClaims)
                {
                    //username
                    if (string.IsNullOrEmpty(UserName))
                    {
                        if (claim.Name != null)
                        {
                            UserName = claim.Name.Nickname;
                        }
                    }
                }
        }

        public string UserName { get; set; }
    }
}
