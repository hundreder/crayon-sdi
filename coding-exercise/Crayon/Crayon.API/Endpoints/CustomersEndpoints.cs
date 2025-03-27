using Crayon.API.Services;
using Microsoft.AspNetCore.Mvc;
using Crayon.API.Endpoints.Dto;

namespace Crayon.API.Endpoints;

public static class CustomersEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder builder)
    {
        var loginGroup = builder.MapGroup("customers")
            .WithTags("Customers");

        loginGroup
            .MapGet("accounts", async (
                [FromServices] ICustomerAccountsService accountsService,
                [FromServices] ILoggedInUserAccessor loggedInUserAccessor,
                CancellationToken ct) =>
            {
                var user = loggedInUserAccessor.User();
                var accounts =  await  accountsService.GetAccounts(user.CustomerId, ct);
                var response = CustomerAccountsResponse.Create(accounts);
                
                return response;

            })
            .Produces<CustomerAccountsResponse>()
            .RequireAuthorization()
            .ProducesProblem(401);
        
        return builder;
    }
}