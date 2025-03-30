using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Crayon.API.Plumbing;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app, bool includeExceptionDetails = false)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/problem+json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature is not null)
                {
                    // Build the ProblemDetails object
                    var problem = new ProblemDetails
                    {
                        Title = "An unexpected error occurred.",
                        Status = context.Response.StatusCode,
                        Detail = includeExceptionDetails
                            ? contextFeature.Error.ToString()
                            : "Please try again later.",
                        Instance = context.Request.Path
                    };

                    // Serialize the ProblemDetails to JSON
                    var jsonResponse = JsonSerializer.Serialize(problem);
                    await context.Response.WriteAsync(jsonResponse);
                }
            });
        });

        return app;
    }
}