using Crayon.API.Models;

namespace Crayon.API.Services;

public interface ICustomerAccountsService
{
    Task<IEnumerable<CustomerAccount>> GetAccounts(string customerId, CancellationToken ct);
}

public class CustomerAccountsService : ICustomerAccountsService
{
    public async Task<IEnumerable<CustomerAccount>> GetAccounts(string customerId, CancellationToken ct) =>
    [
        new CustomerAccount("1", "Account 1", customerId),
        new CustomerAccount("2", "Account 2", customerId)
    ];
}