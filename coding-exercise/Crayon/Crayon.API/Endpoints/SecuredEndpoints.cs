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
                var customerId = loggedInUserAccessor.User().UserId;
                return (await accountsService.GetCustomerAndAccounts(customerId, ct)).ToResponse();
            })
            .Produces<CustomerAccountsResponse>();

        apiGroup
            .MapPost("accounts/{accountId}/orders", async (
                [FromRoute] int accountId,
                [FromBody] NewOrderRequest newOrderRequest,
                [FromServices] IOrdersService ordersService,
                [FromServices] ICurrentUserAccessor loggedInUserAccessor,
                CancellationToken ct
            ) =>
            {
                var customerId = loggedInUserAccessor.User().UserId;
                var newOrder = newOrderRequest.ToModel(customerId, accountId);

                //validation
                //minimum 1 item in order

                var createdOrder =
                    (await ordersService.CreateOrder(newOrder, ct))
                    .Match(
                        order => Results.Created($"accounts/{accountId}/orders/{order.OrderId}", new NewOrderResponse(order.OrderId, order.CcpOrderId)),
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

        apiGroup
            .MapGet("accounts/{accountId}/subscriptions", async (
                [FromRoute] int accountId,
                [FromServices] ICustomerAccountsService accountsService,
                [FromServices] ICurrentUserAccessor loggedInUserAccessor,
                CancellationToken ct
            ) =>
            {
                var customerId = loggedInUserAccessor.User().UserId;
                return (await accountsService.GetSubscriptions(customerId, accountId, ct))
                    .ToResponse();
            })
            .Produces<SubscriptionsResponse>();


        apiGroup
            .MapPost("licences/{licenceId}/licence-count", async (
                [FromRoute] int licenceId,
                [FromBody] NewLicenceCountRequest request,
                [FromServices] ICustomerAccountsService accountsService,
                [FromServices] ICurrentUserAccessor loggedInUserAccessor,
                CancellationToken ct
            ) =>
            {
                var customerId = loggedInUserAccessor.User().UserId;

                return (await accountsService.ChangeLicenceCount(customerId, licenceId, request.LicenceCount, ct))
                    .Match(
                        _ => Results.Ok(),
                        error => error switch
                        {
                            ChangeLicenceCountError.LicenceDoesNotExist => Results.Problem(new ProblemDetails()
                            {
                                Title = error.ToString(),
                                Detail = "Setting new licence count failed.",
                                Status = StatusCodes.Status404NotFound,
                            }),
                            _ => throw new ArgumentOutOfRangeException(nameof(error), error, null)
                        });
            })
            .Produces<SubscriptionsResponse>();


        return builder;
    }
}