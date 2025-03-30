using System.Text.Json.Serialization;
using Crayon.API.Configuration;
using Crayon.API.Endpoints;
using Crayon.API.Services;
using Crayon.API.Plumbing;
using Crayon.Repository;
using Crayon.Repository.ApiClients;
using Crayon.Services.Common;
using Crayon.Services.Services;
using Crayon.Services.Services.Events;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

var services = builder.Services;
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>()!;
services.AddSingleton(appSettings);

services.AddValidatorsFromAssemblyContaining<Program>();
services.AddFluentValidationAutoValidation();

services.ConfigureHttpJsonOptions(options => { options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
//services.AddHttpLogging(o => { });

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
    .AddSingleton<IAuthenticationService, AuthenticationService>()
    .AddSingleton<IDateTimeProvider, DateTimeProvider>()
    .AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssemblyContaining<CompletedOrderHandler>();
        cfg.Lifetime = ServiceLifetime.Scoped;
    })
    .AddDbContext<CrayonDbContext>(optionsBuilder =>
    {
        optionsBuilder.UseNpgsql(appSettings.ConnectionString)
            .UseLazyLoadingProxies()
            .UseSnakeCaseNamingConvention();
    })
    .AddOpenApi(options =>
    {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
    });


services.AddHttpClient<ICcpApiClient, CcpApiClient>();

var app = builder.Build();
app.UseGlobalExceptionHandler(app.Environment.IsDevelopment());
//app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

await app.RunAsync();