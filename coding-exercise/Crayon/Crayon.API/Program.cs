using Crayon.API;
using Crayon.API.Configuration;
using Crayon.API.Endpoints;
using Crayon.API.Services;
using Crayon.API.Plumbing;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>()!;
services.AddSingleton(appSettings);

services
    .AddCustomAuthentication(appSettings)
    .AddScoped<ILoginService, LoginService>()
    .AddHttpContextAccessor()
    .AddScoped<ILoggedInUserAccessor, LoggedInUserAccessor>()
    .AddScoped<ICustomerAccountsService, CustomerAccountsService>();
    
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1"); 
    });
    
}

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

await app.RunAsync();