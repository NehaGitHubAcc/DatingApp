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
        // Get the token key from configuration. This key is used to sign the token and should be kept secret. It is typically stored in appsettings.json or environment variables.
        var tokenKey = configuration["TokenKey"] ?? throw new Exception("TokenKey is not configured in appsettings.json");
        
        // Check if the token key is at least 64 characters long 
        // why? Because we are using HMACSHA512 algorithm to create the token 
        // and it requires a key of at least 64 bytes (512 bits) for optimal security.
        if(tokenKey.Length <64)
            throw new Exception("TokenKey must be at least 64 characters long");
        
        // Create a symmetric security key using the token key from configuration
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        // Create a list of claims to include in the token. Claims are pieces of information about the user that we want to include in the token.
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id),
            // new("CustomClaim", "CustomValue")
        };
        
        // Create signing credentials using the security key and the HMACSHA512 algorithm. This will be used to sign the token to ensure its integrity and authenticity.
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        // Create a security token descriptor that describes the contents of the token, including the claims, expiration time, and signing credentials.
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        // Create a token handler to create the token based on the token descriptor and then write the token as a string to return to the client.
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
