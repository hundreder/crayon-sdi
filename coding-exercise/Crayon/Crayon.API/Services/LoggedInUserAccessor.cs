using System.Security.Claims;
using Crayon.API.Endpoints.Dto;

namespace Crayon.API.Services;

public interface ILoggedInUserAccessor
{
    LoggedInUserResponse User();
}

public class LoggedInUserAccessor(IHttpContextAccessor httpContextAccessor) : ILoggedInUserAccessor
{
    public LoggedInUserResponse User()
    {
        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var user = httpContext.User;
        string email = user.Claims.Single(c => c.Type == ClaimTypes.Email).Value;

        return new LoggedInUserResponse(email, email);
    }
}