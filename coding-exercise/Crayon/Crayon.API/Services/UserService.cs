using Crayon.API.Configuration;
using Crayon.Domain.Models;
using Crayon.Repository;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static LanguageExt.Prelude;

namespace Crayon.API.Services;

public interface IUserService
{
    Task<Either<LoginError, string>> Login(string email, string password, CancellationToken ct);
    Task<User> GetLoggedInUser(CancellationToken ct);
}

public class UserService(
    AppSettings settings,
    ICurrentUserAccessor userAccessor,
    IAuthenticationService authenticationService,
    CrayonDbContext dbContext) : IUserService
{
    public async Task<Either<LoginError, string>> Login(string email, string password, CancellationToken ct)
    {
        if (!ValidateCredentials(email, password))
            return LoginError.InvalidCredentials;

        var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email, ct);

        var jwt = Optional(user)
            .ToEither(LoginError.UserNotFound)
            .Map(u => authenticationService.GenerateJwtToken(u.Id));

        return jwt;
    }

    public async Task<User> GetLoggedInUser(CancellationToken ct) =>
        await dbContext.Users.SingleAsync(u => u.Id == userAccessor.User().UserId, cancellationToken: ct);

    private bool ValidateCredentials(string email, string password)
        => password == "123123";
}

public enum LoginError
{
    InvalidCredentials,
    UserNotFound
}