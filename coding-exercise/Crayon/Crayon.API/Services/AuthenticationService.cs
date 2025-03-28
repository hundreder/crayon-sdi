using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Crayon.API.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Crayon.API.Services;

public interface IAuthenticationService
{
    Task<string> Login(string email, string password);
}

public class AuthenticationService(AppSettings settings) : IAuthenticationService
{
    public async Task<string> Login(string email, string password)
    {
        return GenerateJwtToken(email);
    }

    private string GenerateJwtToken(string email)
    {
        var secretKey = settings.JwtSecretKey;
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, "User")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(securityToken);
    }
}