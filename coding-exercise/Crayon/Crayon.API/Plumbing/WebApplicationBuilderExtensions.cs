using System.Text;
using Crayon.API.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Crayon.API.Plumbing;

public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Simple custom authentication 
    /// </summary>
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, AppSettings appSettings)
    {
        var keyBytes = Encoding.ASCII.GetBytes(appSettings.JwtSecretKey);
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddOpenTelemetryTracing(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CrayonServices"))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
                    // .AddOtlpExporter(otlpOptions =>
                    // {
                    //     otlpOptions.Endpoint = new Uri("http://localhost:4318/v1/traces");
                    // });
            });
        return services;
    }
}