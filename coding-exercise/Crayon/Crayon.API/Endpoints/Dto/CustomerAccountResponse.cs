using Crayon.API.Models;
using Crayon.API.Services;

namespace Crayon.API.Endpoints.Dto;

public record CustomerAccountResponse(string Id, string Name)
{
    public static CustomerAccountResponse Create(CustomerAccount account) =>
        new(account.Id, account.Name);
}

public record CustomerAccountsResponse(IEnumerable<CustomerAccountResponse> Accounts)
{
    public static CustomerAccountsResponse Create(IEnumerable<CustomerAccount> accounts) =>
        new(accounts.Select(CustomerAccountResponse.Create));
}