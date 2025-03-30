using System.Security.Claims;

namespace Crayon.API.Services;

public interface ICurrentUserAccessor
{
    CurrentUser User();
}

public class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public CurrentUser User()
    {
        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var user = httpContext.User;
        string subject = user.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
        int userId = int.Parse(subject);
        
        return new CurrentUser(userId);
    }
}

public record CurrentUser(int UserId);