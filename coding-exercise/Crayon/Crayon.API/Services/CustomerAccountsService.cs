using Crayon.Domain.Models;
using Crayon.Repository;
using Microsoft.EntityFrameworkCore;

namespace Crayon.API.Services;

public interface ICustomerAccountsService
{
    Task<Customer> GetCustomerAndAccounts(int userId, CancellationToken ct);
}

public class CustomerAccountsService( CrayonDbContext dbContext) : ICustomerAccountsService
{
    public async Task<Customer> GetCustomerAndAccounts(int userId, CancellationToken ct) =>
        await
            dbContext.Customers
                .Include(c => c.Accounts)
                .SingleAsync(c => c.Id == userId, cancellationToken: ct);
}