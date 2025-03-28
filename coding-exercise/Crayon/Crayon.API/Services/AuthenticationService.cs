using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Crayon.API.Configuration;
using Crayon.Domain.Models;
using Crayon.Repository;
using Microsoft.IdentityModel.Tokens;
using LanguageExt;

namespace Crayon.API.Services;

public interface IAuthenticationService
{
    Task<Either<LoginError, string>> Login(string email, string password);
    Task<User> GetLoggedInUser();
}

public class AuthenticationService(AppSettings settings, CrayonDbContext dbContext) : IAuthenticationService
{
    public async Task<Either<LoginError, string>> Login(string email, string password)
    {
        if (!ValidateCredentials(email, password))
            return LoginError.InvalidCredentials;

        var user = dbContext.Users.SingleOrDefault(u => u.Email == email);
        
        var jwt = Prelude.Optional(user)
            .ToEither(LoginError.UserNotFound)
            .Map(GenerateJwtToken);

        return jwt;
    }

    public Task<User> GetLoggedInUser()
    {
        throw new NotImplementedException();
    }

    private bool ValidateCredentials(string email, string password)
        => password == "123456";
    
    
    private string GenerateJwtToken(User user)
    {
        var secretKey = settings.JwtSecretKey;
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
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

public enum LoginError
{
    InvalidCredentials,
    UserNotFound
}