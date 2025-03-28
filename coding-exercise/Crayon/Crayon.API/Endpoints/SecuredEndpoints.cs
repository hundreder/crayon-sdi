using System.Net;
using Crayon.API.Services;
using Microsoft.AspNetCore.Mvc;
using Crayon.API.Endpoints.Dto;
using Crayon.API.Models;

namespace Crayon.API.Endpoints;

public static class SecuredEndpoints
{
    public static IEndpointRouteBuilder MapSecureEndpoints(this IEndpointRouteBuilder builder)
    {
        var apiGroup = builder.MapGroup("api/v1")
            .RequireAuthorization()
            .WithTags("Secured API")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        apiGroup
            .MapGet("user", (
                    [FromServices] ILoggedInUserAccessor loggedInUserAccessor,
                    HttpContext context,
                    CancellationToken ct) =>
                loggedInUserAccessor.User())
            .Produces<LoggedInUserResponse>();

        apiGroup
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
            .Produces<CustomerAccountsResponse>();

        apiGroup
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
                            Status = error == CreateOrderError.SubmittingOrderToExternalProviderFailed
                                ? StatusCodes.Status502BadGateway
                                : StatusCodes.Status400BadRequest,
                        })
                    );

                return createdOrder;
            })
            .Produces<NewOrderResponse>(201)
            .ProducesProblem(400);
        
       

        return builder;
    }
}