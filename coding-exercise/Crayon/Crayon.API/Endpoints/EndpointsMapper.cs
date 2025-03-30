using FluentValidation;

namespace Crayon.API.Endpoints;

public static class EndpointsMapper
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder
            .MapPublicEndpoints()
            .MapSecureEndpoints();
        
        return builder;
    }
}
