using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Agriculture.Helpers
{
    public class Token
    {
        public string GenerateToken(string value, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ProjectReactNativeSecretKeyToken"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new();
            claims.Add(new(JwtRegisteredClaimNames.Sub, value));
            claims.Add(new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(ClaimTypes.Role, role));
            var token = new JwtSecurityToken(
                   issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
              );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
