using Crayon.API.Services;
using Microsoft.AspNetCore.Mvc;
using Crayon.API.Endpoints.Dto;
using Crayon.Domain.Errors;
using Crayon.Services.Services;

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
            .Produces<LoggedInUserResponse>()
            .WithDescription("Get logged in user data.");

        apiGroup
            .MapGet("accounts", async (
                [FromServices] ICustomerAccountsService accountsService,
                [FromServices] ICurrentUserAccessor loggedInUserAccessor,
                CancellationToken ct) =>
            {
                var customerId = loggedInUserAccessor.User().UserId;
                return (await accountsService.GetCustomerAndAccounts(customerId, ct)).ToResponse();
            })
            .Produces<CustomerAccountsResponse>()
            .WithDescription("Get logged in user account.");

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
            .ProducesProblem(400)
            .WithDescription("Make order for account.");

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
            .Produces<SubscriptionsResponse>()
            .WithDescription("Get accounts subscriptions");

        apiGroup
            .MapPost("subscriptions{subscriptionId}/cancel", async (
                    [FromRoute] int subscriptionId,
                    [FromServices] ICustomerAccountsService accountsService,
                    [FromServices] ICurrentUserAccessor loggedInUserAccessor,
                    CancellationToken ct) =>
                (await accountsService.CancelSubscription(
                    loggedInUserAccessor.User().UserId,
                    subscriptionId,
                    ct))
                .Match(
                    _ => Results.NoContent(),
                    error => Results.Problem(new ProblemDetails
                    {
                        Title = error.ToString(),
                        Detail = "Canceling subscription failed. Check your input and try again.",
                        Status = error == CancelSubscriptionError.SubscriptionDoesNotExist
                            ? StatusCodes.Status404NotFound
                            : StatusCodes.Status400BadRequest
                    })
                ))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Cancel specific subscription.");

        apiGroup
            .MapPost("licences/{licenceId}/licence-count", async (
                    [FromRoute] int licenceId,
                    [FromBody] NewLicenceCountRequest request,
                    [FromServices] ICustomerAccountsService accountsService,
                    [FromServices] ICurrentUserAccessor loggedInUserAccessor,
                    CancellationToken ct) =>
                (await accountsService.ChangeLicenceCount(
                    loggedInUserAccessor.User().UserId,
                    licenceId,
                    request.LicenceCount,
                    ct))
                .Match(
                    _ => Results.NoContent(),
                    error =>
                        Results.Problem(new ProblemDetails()
                            {
                                Title = error.ToString(),
                                Detail = "Setting new licence count failed. Check your input and try again.",
                                Status = error == ChangeLicenceCountError.LicenceDoesNotExist
                                    ? StatusCodes.Status404NotFound
                                    : StatusCodes.Status400BadRequest,
                            }
                        )
                ))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Change licence count for specific licence.");


        apiGroup
            .MapPost("licences/{licenceId}/valid-to", async (
                    [FromRoute] int licenceId,
                    [FromBody] ExtendLicenceValidToDateRequest request,
                    [FromServices] ICustomerAccountsService accountsService,
                    [FromServices] ICurrentUserAccessor loggedInUserAccessor,
                    CancellationToken ct) =>
                (await accountsService.ExtendLicenceValidToDate(
                    loggedInUserAccessor.User().UserId,
                    licenceId,
                    request.NewValidToDate,
                    ct))
                .Match(
                    _ => Results.NoContent(),
                    error =>
                        Results.Problem(new ProblemDetails()
                            {
                                Title = error.ToString(),
                                Detail = "Extending licence valid to date failed. Check your input and try again.",
                                Status = error == ExtendLicenceValidToDateError.LicenceDoesNotExist
                                    ? StatusCodes.Status404NotFound
                                    : StatusCodes.Status400BadRequest,
                            }
                        )
                ))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Change licence valid to for specific licence.");


        return builder;
    }
}