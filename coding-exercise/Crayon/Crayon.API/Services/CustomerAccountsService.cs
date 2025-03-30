using Crayon.Domain.Models;
using Crayon.Repository;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Crayon.API.Services;

public interface ICustomerAccountsService
{
    Task<Customer> GetCustomerAndAccounts(int customerId, CancellationToken ct);
    Task<List<Subscription>> GetSubscriptions(int customerId, int accountId, CancellationToken ct);
    Task<Either<ChangeLicenceCountError, Unit>> ChangeLicenceCount(int customerId, int licenceId, int newLicenceCount, CancellationToken ct);
}

public class CustomerAccountsService(CrayonDbContext dbContext) : ICustomerAccountsService
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

    
    public async Task<Either<ChangeLicenceCountError, Unit>> ChangeLicenceCount(int customerId, int licenceId, int newLicenceCount, CancellationToken ct)
    {
        var licence = await dbContext.Licences
            .AsSingleQuery()
            .Include(l => l.Subscription)
            .ThenInclude(s=>s.Account)
            .SingleOrDefaultAsync(l => l.Id == licenceId, cancellationToken: ct);
        
        if (licence is null)
            return ChangeLicenceCountError.LicenceDoesNotExist;        
        
        if (!await LicenceBelongsToCustomer(customerId, licence))
            return ChangeLicenceCountError.LicenceDoesNotExist;
        
        if (licence.LicenceCount == newLicenceCount)
            return ChangeLicenceCountError.NewLicenceCountCantBeSameAsExising;
        
        // do stuff required for getting new licences. Maybe new order?
        licence.LicenceCount = newLicenceCount;

        await dbContext.SaveChangesAsync(ct);

        return Unit.Default;
    }

    private async Task<bool> LicenceBelongsToCustomer(int customerId, Licence licence) => licence.Subscription.Account.CustomerId == customerId;
}

public enum ChangeLicenceCountError
{
    LicenceDoesNotExist,
    NewLicenceCountCantBeSameAsExising
}