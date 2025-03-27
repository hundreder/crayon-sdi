namespace Crayon.API.Services;

public interface ILoginService
{
    Task<string> Login(string email, string password);
}

public class LoginService : ILoginService
{
    public async Task<string> Login(string email, string password)
    {
        return "";
    }
}