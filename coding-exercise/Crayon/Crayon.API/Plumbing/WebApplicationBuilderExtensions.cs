using System.Text;
using Crayon.API.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
    
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen((Action<SwaggerGenOptions>) (c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Crayon",
                Version = "v1"
            });
            c.EnableAnnotations();
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "JWT Authorization header using the Bearer scheme. \\r\\n\\r\\n \n                      Enter 'Bearer' [space] and then your token in the text input below.\n                      \\r\\n\\r\\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            SwaggerGenOptions swaggerGenOptions = c;
            swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = new ReferenceType?(ReferenceType.SecurityScheme),
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    (IList<string>) new List<string>()
                }
            });
        }));
        return services;
    }
}