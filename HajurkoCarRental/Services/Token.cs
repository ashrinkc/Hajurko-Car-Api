using HajurkoCarRental.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HajurkoCarRental.Services
{
    public class Token
    {
        private readonly IConfiguration _config;
        public Token(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateToken(AppUser user)
        {
            
            //create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserType)
            };

            //create a key and sign the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                    issuer: _config["JwtIssuer"],
                    audience: _config["JwtAudience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
              );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
