using Crayon.API.Endpoints.Dto;
using Crayon.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crayon.API.Endpoints;

public static class PublicEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder builder)
    {
        var publicApiGroup = builder.MapGroup("api/vi")
            .WithTags("PublicApi");

        publicApiGroup
            .MapPost("login", async (
                [FromBody] LoginRequest request,
                [FromServices] IAuthenticationService loginService,
                CancellationToken ct) =>
            {
                var token = await loginService.Login(request.Email, request.Password);
                return new LoginResponse(token);
            })
            .Produces<LoginResponse>()
            .ProducesProblem(400);

        publicApiGroup.MapGet("catalog", async (
                [FromServices] ISoftwareCatalogService softwareCatalogService,
                [FromQuery] string? nameLike,
                CancellationToken ct,
                [FromQuery] int? skip = 0,
                [FromQuery] int? take = 10
            ) =>
            {
                var sc = await softwareCatalogService.GetSoftwareCatalog(nameLike, skip, take, ct);

                return SoftwareCatalogResponse.Create(sc);
            })
            .Produces<SoftwareCatalogResponse>()
            .ProducesProblem(400);

        return builder;
    }
}