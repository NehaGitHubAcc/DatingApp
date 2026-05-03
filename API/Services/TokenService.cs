using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        // tokenkey(string) -> Key(byte) -> creds(SigningCredentials)
        var tokenKey = configuration["TokenKey"] ?? throw new Exception("TokenKey is not configured in appsettings.json");
        if(tokenKey.Length <64) throw new Exception("TokenKey must be at least 64 characters long");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        // claims: user information to be included in the token
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id),
            // new("CustomClaim", "CustomValue")
        };
        
        // tokenDescriptor: describes the token's content and properties
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        // tokenHandler: creates and writes the token based on the descriptor
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
