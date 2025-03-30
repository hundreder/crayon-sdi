using Crayon.Domain.Models;
using Crayon.Repository;
using Microsoft.EntityFrameworkCore;

namespace Crayon.API.Services;

public interface ICustomerAccountsService
{
    Task<Customer> GetCustomerAndAccounts(int customerId, CancellationToken ct);
    Task<List<Subscription>> GetSubscriptions(int customerId, int accountId, CancellationToken ct);
}

public class CustomerAccountsService( CrayonDbContext dbContext) : ICustomerAccountsService
{
    public async Task<Customer> GetCustomerAndAccounts(int customerId, CancellationToken ct) =>
        await
            dbContext.Customers
                .Include(c => c.Accounts)
                .SingleAsync(c => c.Id == customerId, cancellationToken: ct);

    public async Task<List<Subscription>> GetSubscriptions(int customerId, int accountId, CancellationToken ct) =>
        await dbContext.Accounts.Where(acc => acc.CustomerId == customerId && acc.Id == accountId)
            .SelectMany(acc => acc.Subscriptions)
            .ToListAsync(ct);

}