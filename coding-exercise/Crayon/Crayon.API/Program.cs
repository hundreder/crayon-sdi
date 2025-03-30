using System.Reflection;
using Crayon.API.ApiClients;
using Crayon.API.Configuration;
using Crayon.API.Endpoints;
using Crayon.API.Services;
using Crayon.API.Plumbing;
using Crayon.API.Repository;
using Crayon.API.Services.Events;
using Crayon.Domain.Models;
using Crayon.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>()!;
services.AddSingleton(appSettings);

services
    .AddCustomAuthentication(appSettings)
    .AddScoped<IUserService, UserService>()
    .AddHttpContextAccessor()
    .AddScoped<ICurrentUserAccessor, CurrentUserAccessor>()
    .AddScoped<ICustomerAccountsService, CustomerAccountsService>()
    .AddScoped<ISoftwareCatalogRepository, SoftwareCatalogRepository>()
    .AddScoped<ISoftwareCatalogService, SoftwareCatalogService>()
    .AddScoped<IOrdersService, OrdersService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IPurchaseService, PurchaseService>()
    .AddSingleton<IAuthenticationService, AuthenticationService>();

services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.Lifetime = ServiceLifetime.Scoped;
});

services.AddHttpClient<ICcpApiClient, CcpApiClient>();
services.AddDbContext<CrayonDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(appSettings.ConnectionString)
        .UseLazyLoadingProxies()
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