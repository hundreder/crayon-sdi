namespace Crayon.API.Configuration;

public class AppSettings
{
    public required string JwtSecretKey { get; init; }
    public required string ConnectionString { get; init; }
}