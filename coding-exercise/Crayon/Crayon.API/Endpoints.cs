using Crayon.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crayon.API;

public static class Endpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/weatherforecast", () =>
            {
                var summaries = new[]
                {
                    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
                };
                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast");


        // ------------------------------------------------------------------------------------------------


        var loginGroup = app.MapGroup("user")
            .WithTags("User");

        loginGroup
            .MapPost("login", async (
                [FromBody] LoginRequest request,
                [FromServices] ILoginService logionService,
                CancellationToken ct) =>
            {
                var token = await logionService.Login(request.Email, "password");
                return new LoginResponse(token);
            })
            .Produces<LoginResponse>()
            .ProducesProblem(400);

        loginGroup
            .MapGet("", (
                    [FromServices] ILoggedInUserAccessor loggedInUserAccessor,
                    HttpContext context,
                    CancellationToken ct) =>
                loggedInUserAccessor.User())
            .RequireAuthorization()
            .Produces<LoggedInUser>()
            .ProducesProblem(401);
    }
}

public record LoginRequest(string Email);

public record LoginResponse(string Token);

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}