using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatGptApi.TokenConfig
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _mySettings;
        public TokenService(IConfiguration mySettings)
        {
            _mySettings = mySettings;
        }

        public string CreateToken(IdentityUser user)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_mySettings.GetValue<string>("Token:TokenSecurityKey")));
            int TokenExpiry = Convert.ToInt32(_mySettings.GetValue<string>("Token:TokenExpiryDays"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                Expires = DateTime.Now.AddDays(TokenExpiry),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
