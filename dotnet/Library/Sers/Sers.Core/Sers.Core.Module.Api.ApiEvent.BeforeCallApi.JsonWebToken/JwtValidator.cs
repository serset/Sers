#region << version >>
/* ========================================================================
 * Author  : Lith
 * Version : 1.0
 * Date    : 2023-06-15
 * Email   : serset@yeah.net
 * ======================================================================== */
#endregion


using System.Collections.Generic;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace Vit.SSO
{
    public class JwtValidator
    {
        public static byte[] GetSecurityKey(string Secret)
        {
            // Convert the secret key to a byte array and ensure that the length is larger than 32*8 bits
            var key = Encoding.UTF8.GetBytes(Secret ?? "");
            if (key.Length < 32)
            {
                var newKey = new byte[32];
                key.CopyTo(newKey, 0);
                key = newKey;
            }
            return key;
        }

        public static UserInfo ValidateToken(string token, string Secret, string issuer = null, List<string> audiences = null)
        {
            return ValidateToken(token, GetSecurityKey(Secret), issuer, audiences);
        }

        public static UserInfo ValidateToken(string token, byte[] securityKeyBytes, string issuer = null, List<string> audiences = null)
        {
            //#1 get validation config
            var key = new SymmetricSecurityKey(securityKeyBytes);
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
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
            var jwt = tokenHandler.ValidateToken(token, parameters, out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            //#3 validate Audience
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

        public class UserInfo
        {
            public Dictionary<string, string> Claims { get; set; }
            public IEnumerable<string> Audiences { get; set; }
            public string Issuer { get; set; }
            public DateTime ValidTo { get; set; }
        }
    }
}
