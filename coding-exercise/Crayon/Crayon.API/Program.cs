using Crayon.API;
using Crayon.API.Configuration;
using Crayon.API.Services;
using Crayon.API.Plumbing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>()!;
services.AddSingleton(appSettings);

services//.AddSwagger()
    .AddCustomAuthentication(appSettings)
    .AddScoped<ILoginService, LoginService>();
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Optional: customize your Swagger doc info
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MySwaggerApi",
        Version = "v1",
        Description = "An example ASP.NET Core Web API with Swagger documentation",
        
    });
});
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// services.AddOpenApi(o => o.AddOperationTransformer((operation, context, cancellationToken) =>
// {
//     if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
//     {
//         operation.Security =
//         [
//             new()
//             {
//                 {
//                     new OpenApiSecurityScheme()
//                     {
//                         Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
//                                       "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
//                                       "Example: \"Bearer 12345abcdef\"",
//                         Name = "Authorization",
//                         In = ParameterLocation.Header,
//                         Type = SecuritySchemeType.Http,
//                         Scheme = "bearer", // IMPORTANT: must be "bearer"
//                         BearerFormat = "JWT" // (Optional) can be used to specify the token format
//                     },
//                     ["Bearer"]
//                 }
//             }
//         ];
//     }
//
//     return Task.CompletedTask;
// }));



var app = builder.Build();

app.UseSwagger();

// Enable the Swagger UI middleware
app.UseSwaggerUI(c =>
{
    // The Swagger JSON endpoint (mapped to the doc we specified above, "v1")
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MySwaggerApi v1");
    // Optional: serve the Swagger UI at application's root (http://localhost:<port>/)
    // c.RoutePrefix = string.Empty;  // Uncomment to serve it at the root
});

//app.UseCustomSwagger("http://localhost:5072", true);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    
}

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

await app.RunAsync();