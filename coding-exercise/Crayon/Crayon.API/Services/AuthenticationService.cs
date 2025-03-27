using System.Security.Claims;

namespace Crayon.API.Services;

public interface ILoggedInUserAccessor
{
    LoggedInUser User();
}

public record LoggedInUser(string Email);

public class LoggedInUserAccessor(IHttpContextAccessor httpContextAccessor) : ILoggedInUserAccessor
{

    public LoggedInUser User()
    {
        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var user = httpContext.User;
        string email = user.Claims.Single(c => c.Type == ClaimTypes.Email).Value;

        return new LoggedInUser(email);
    }
}