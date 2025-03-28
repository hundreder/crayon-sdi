using Crayon.API.ApiClients;
using Crayon.API.Configuration;
using Crayon.API.Endpoints;
using Crayon.API.Services;
using Crayon.API.Plumbing;
using Crayon.API.Repository;
using Crayon.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>()!;
services.AddSingleton(appSettings);

services
    .AddCustomAuthentication(appSettings)
    .AddScoped<IAuthenticationService, AuthenticationService>()
    .AddHttpContextAccessor()
    .AddScoped<ILoggedInUserAccessor, LoggedInUserAccessor>()
    .AddScoped<ICustomerAccountsService, CustomerAccountsService>()
    .AddScoped<ISoftwareCatalogRepository, SoftwareCatalogRepository>()
    .AddScoped<ISoftwareCatalogService, SoftwareCatalogService>()
    .AddScoped<IOrdersService, OrdersService>();

services.AddHttpClient<ICcpApiClient, CcpApiClient>();
services.AddDbContext<CrayonDbContext>(builder =>
{
    builder.UseNpgsql(appSettings.ConnectionString)
        .UseSnakeCaseNamingConvention();
});

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
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

await app.RunAsync();