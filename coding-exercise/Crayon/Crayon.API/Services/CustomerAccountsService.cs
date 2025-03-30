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
        if (!await LicenceBelongsToCustomer(licenceId, licenceId, ct))
            return ChangeLicenceCountError.LicenceDoesNotExist;
        
        var licence = await dbContext.Licences.SingleOrDefaultAsync(l => l.Id == licenceId, cancellationToken: ct);
        if (licence is null)
            return ChangeLicenceCountError.LicenceDoesNotExist;

        if (licence.LicenceCount == newLicenceCount)
            return ChangeLicenceCountError.NewLicenceCountCantBeSameAsExising;
        
        // do stuff required for getting new licence
        licence.LicenceCount = newLicenceCount;

        await dbContext.SaveChangesAsync(ct);

        return Unit.Default;
    }

    private async Task<bool> LicenceBelongsToCustomer(int customerId, int licenceId, CancellationToken ct)
    {
        var licence = await dbContext.Licences
            .SingleOrDefaultAsync(l => l.Id == licenceId && l.Subscription.Account.CustomerId == customerId, cancellationToken: ct);

        return licence != null;
    }

        
}

public enum ChangeLicenceCountError
{
    LicenceDoesNotExist,
    NewLicenceCountCantBeSameAsExising
}