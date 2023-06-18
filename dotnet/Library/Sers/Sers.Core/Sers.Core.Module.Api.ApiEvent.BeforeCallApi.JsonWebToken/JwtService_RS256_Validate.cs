#region << version 1.2 >>
/* ========================================================================
 * Author  : Lith
 * Version : 1.2
 * Date    : 2023-06-17
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
using Vit.Extensions.Json_Extensions;


namespace Vit.SSO
{
    public class UserInfo
    {
        public Dictionary<string, string> Claims { get; set; }
        public IEnumerable<string> Audiences { get; set; }
        public string Issuer { get; set; }
        public DateTime ValidTo { get; set; }
    }

    public class JwtService_RS256_Validate
    {


        #region validate token

        public string publicKeysDiscovery_Url { get; set; }
        public List<JsonWebKey> GetPublicJsonWebKeysFromUrl()
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
        public List<string> audiences { get; set; }





        protected SecurityKey _publicSecurityKey = null;
        SecurityKey GetPublicSecurityKey()
        {
            if (_publicSecurityKey != null) return _publicSecurityKey;

            if (!string.IsNullOrWhiteSpace(publicKeysDiscovery_Url))
            {
                try
                {
                    var jwkList = GetPublicJsonWebKeysFromUrl();
                    var jwk = jwkList?.FirstOrDefault();

                    var _credentials = new SigningCredentials(jwk, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.Sha256);
                    _publicSecurityKey = _credentials.Key;
                    return _publicSecurityKey;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                publicKeysDiscovery_Url = null;
            }
            return null;
        }


        public UserInfo ValidateToken(string token, string issuer = null, List<string> audiences = null)
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
        #endregion
    }
}
