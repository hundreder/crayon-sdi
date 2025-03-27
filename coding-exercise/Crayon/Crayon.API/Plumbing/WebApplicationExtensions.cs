using Microsoft.OpenApi.Models;

namespace Crayon.API.Plumbing;

public static class WebApplicationExtensions
{
    public static WebApplication UseCustomSwagger(this WebApplication app, string apiBaseUrl, bool isDevelopment)
    {
        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                swaggerDoc.Servers = new List<OpenApiServer>
                {
                    new()
                    {
                        Url = apiBaseUrl
                    }
                };
            });
        });
        app.UseSwaggerUI();

        return app;
    }
}