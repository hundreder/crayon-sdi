namespace Crayon.API.Endpoints;

public static class EndpointsMapper
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapUserEndpoints()
            .MapCustomerEndpoints()
            .MapWeatherEndpoints();
        return builder;
    }
}