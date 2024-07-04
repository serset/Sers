#region << version 1.3 >>
/* ========================================================================
 * Author  : Lith
 * Version : 1.3
 * Date    : 2023-06-18
 * Email   : serset@yeah.net
 * ======================================================================== */
#endregion

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;

using Microsoft.IdentityModel.Tokens;

using Vit.Core.Module.Log;
using Vit.Extensions.Serialize_Extensions;
using Vit.SSO.Model;

namespace Vit.SSO.Service.RS256
{
    public class JwtValidateService_RS256Base
    {
        public string issuer { get; set; }

        public List<string> audiences { get; set; }

        public string publicKeysDiscovery_Url { get; set; }

        public List<JsonWebKey> GetPublicJsonWebKeysByUrl()
        {
            if (!string.IsNullOrWhiteSpace(publicKeysDiscovery_Url))
            {

                using (var client = new HttpClient())
                {
                    var strPublicKey = client.GetStringAsync(publicKeysDiscovery_Url).Result;
                    var jwkList = strPublicKey.Deserialize<List<JsonWebKey>>();
                    return jwkList;
                }
            }
            return null;
        }



        protected SecurityKey _publicSecurityKey = null;
        protected virtual SecurityKey GetPublicSecurityKey()
        {
            if (_publicSecurityKey == null)
            {
                if (!string.IsNullOrWhiteSpace(publicKeysDiscovery_Url))
                {
                    try
                    {
                        var jwkList = GetPublicJsonWebKeysByUrl();
                        var jwk = jwkList?.FirstOrDefault();

                        var _credentials = new SigningCredentials(jwk, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.Sha256);
                        _publicSecurityKey = _credentials.Key;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }
            return _publicSecurityKey;
        }


        public UserInfo ValidateToken(string token, List<string> audiences = null)
        {
            //#1 get validation config
            var securityKey = GetPublicSecurityKey();
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            if (issuer != null)
            {
                parameters.ValidateIssuer = true;
                parameters.ValidIssuer = issuer;
            }

            //#2 validate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ValidateToken(token, parameters, out var securityToken);
            var jwtToken = securityToken as JwtSecurityToken;

            //#3 validate Audience
            if (audiences == null) audiences = this.audiences;
            if (audiences?.Any() == true)
            {
                if (!jwtToken.Audiences.Any(a => audiences.Contains(a.ToString(), StringComparer.OrdinalIgnoreCase)))
                {
                    throw new SecurityTokenValidationException($"Token contains invalid audience.");
                }
            }

            //#4 get userInfo
            return new UserInfo
            {
                Claims = jwtToken.Claims.ToDictionary(x => x.Type, x => x.Value),
                Audiences = jwtToken.Audiences,
                Issuer = jwtToken.Issuer,
                ValidTo = jwtToken.ValidTo
            };
        }
    }
}
