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
            .MapGet("user", async (
                [FromServices] IUserService userService,
                CancellationToken ct) => (await userService.GetLoggedInUser(ct)).ToResponse())
            .Produces<LoggedInUserResponse>();

        apiGroup
            .MapGet("accounts", async (
                [FromServices] ICustomerAccountsService accountsService,
                [FromServices] ICurrentUserAccessor loggedInUserAccessor,
                CancellationToken ct) =>
            {
                var userId = loggedInUserAccessor.User().UserId;
                return (await accountsService.GetCustomerAndAccounts(userId, ct)).ToResponse();
            })
            .Produces<CustomerAccountsResponse>();

        apiGroup
            .MapPost("accounts/{accountId}/orders", async (
                [FromRoute] string accountId,
                [FromBody] NewOrderRequest newOrderRequest,
                [FromServices] IOrdersService ordersService,
                [FromServices] ICurrentUserAccessor loggedInUserAccessor,
                CancellationToken ct
            ) =>
            {
                var customerId = loggedInUserAccessor.User().UserId.ToString();
                var items = newOrderRequest
                    .ItemsToOrder
                    .Select(i => new NewOrderItem(i.SoftwareId, i.LicenseCount, i.LicencedUntil))
                    .ToList();

                var newOrder = new NewOrder(customerId, accountId, items);
                var createdOrder =
                    (await ordersService.CreateOrder(newOrder, ct))
                    .Match(
                        order => Results.Created($"accounts/{accountId}/orders/{order.Id}", new NewOrderResponse(order.Id)),
                        error => Results.Problem(new ProblemDetails()
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