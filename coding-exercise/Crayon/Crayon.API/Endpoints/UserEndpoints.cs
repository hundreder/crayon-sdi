using Crayon.API.Endpoints.Dto;
using Crayon.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crayon.API.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder builder)
    {
        var loginGroup = builder.MapGroup("user")
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

        return builder;
    }
}