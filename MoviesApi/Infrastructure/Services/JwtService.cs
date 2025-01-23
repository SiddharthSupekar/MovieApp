using App.Core.Dtos;
using App.Core.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(TokenDto dto, string apiKey)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]));
            var claims = new List<Claim>
            {
                new Claim("email", dto.email),
                new Claim("userId",dto.id.ToString()),
                new Claim("apiKey",apiKey),
                new Claim("roleName", dto.roleName),
                new Claim(ClaimTypes.Role,dto.roleName)
               
            };
            var token = new JwtSecurityToken(
                    issuer : issuer,
                    audience : audience,
                    signingCredentials:credentials,
                    claims:claims,
                    expires:expiry
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
