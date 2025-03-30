using Crayon.API.Endpoints.Dto;
using Crayon.API.Services;
using Crayon.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Crayon.API.Endpoints;

public static class PublicEndpoints
{
    public static IEndpointRouteBuilder MapPublicEndpoints(this IEndpointRouteBuilder builder)
    {
        var publicApiGroup = builder.MapGroup("api/vi")
            .WithTags("PublicApi");

        publicApiGroup
            .MapGet("health", Results.NoContent)
            .Produces(StatusCodes.Status204NoContent)
            .WithDescription("Service health endpoint");
        
        publicApiGroup
            .MapPost("login", async (
                [FromBody] LoginRequest request,
                [FromServices] IUserService loginService,
                CancellationToken ct) =>
            {
                var token = (await loginService.Login(request.Email, request.Password, ct))
                    .Match(
                        jwt => Results.Ok(new LoginResponse(jwt)),
                        _ => Results.Problem(new ProblemDetails
                        {
                            Title = "InvalidCredentials", // we dont want to return the real reason, but we should log what happened
                            Detail = "Login failed.",
                            Status = StatusCodes.Status400BadRequest,
                        }));

                return token;
            })
            .WithDescription("Login with credentials endpoint")
            .Produces<LoginResponse>()
            .ProducesProblem(400);

        publicApiGroup.MapGet("catalog", async (
                [FromServices] ISoftwareCatalogService softwareCatalogService,
                [FromQuery] string? nameLike,
                CancellationToken ct,
                [FromQuery] int? skip = 0,
                [FromQuery] int? take = 10
            ) => 
                //basic validation should be performed
                (await softwareCatalogService.GetSoftwareCatalog(nameLike, skip, take, ct)).ToResponse())
            .Produces<SoftwareCatalogResponse>()
            .ProducesProblem(400)
            .WithDescription("Get software catalog endpoint");


        return builder;
    }
}