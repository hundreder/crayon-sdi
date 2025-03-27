using Crayon.API.Services;
using Microsoft.AspNetCore.Mvc;
using Crayon.API.Endpoints.Dto;
using Crayon.API.Models;

namespace Crayon.API.Endpoints;

public static class CustomersEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder builder)
    {
        var customersGroup = builder.MapGroup("customers")
            .RequireAuthorization()
            .WithTags("Customers");

        customersGroup
            .MapGet("accounts", async (
                [FromServices] ICustomerAccountsService accountsService,
                [FromServices] ILoggedInUserAccessor loggedInUserAccessor,
                CancellationToken ct) =>
            {
                var user = loggedInUserAccessor.User();
                var accounts = await accountsService.GetAccounts(user.CustomerId, ct);
                var response = CustomerAccountsResponse.Create(accounts);

                return response;
            })
            .Produces<CustomerAccountsResponse>()
            .ProducesProblem(401);

        customersGroup
            .MapPost("accounts/{accountId}/orders", async (
                [FromRoute] string accountId,
                [FromBody] NewOrderRequest newOrderRequest,
                [FromServices] IOrdersService ordersService,
                [FromServices] ILoggedInUserAccessor loggedInUserAccessor,
                CancellationToken ct
            ) =>
            {
                var customerId = loggedInUserAccessor.User().CustomerId;
                var items = newOrderRequest
                    .ItemsToOrder
                    .Select(i => new NewOrderItem(i.SoftwareId, i.LicenseCount, i.LicencedUntil))
                    .ToList();

                var newOrder = new NewOrder(customerId, accountId, items);
                var createdOrder = 
                    (await ordersService.CreateOrder(newOrder, ct))
                    .Match(
                        order => Results.Created($"accounts/{accountId}/orders/{order.Id}", new NewOrderResponse(order.Id)),
                        error=> Results.Problem(new ProblemDetails()
                        {
                            Title = error.ToString(),
                            Detail = "Failed to create order.",
                            Status = StatusCodes.Status400BadRequest
                        })
                    );

                return createdOrder;
            })
            .Produces<NewOrderResponse>()
            .ProducesProblem(400)
            .RequireAuthorization()
            ;

        builder.MapGet("catalog", async (
                [FromServices] ISoftwareCatalogService softwareCatalogService,
                [FromQuery] string? nameLike,
                CancellationToken ct,
                [FromQuery] int? skip = 0,
                [FromQuery] int? take = 10
            ) =>
            {
                var sc = await softwareCatalogService.GetSoftwareCatalog(nameLike, skip, take, ct);

                return SoftwareCatalogResponse.Create(sc);
            })
            .Produces<SoftwareCatalog>()
            .ProducesProblem(400);

        return builder;
    }
}