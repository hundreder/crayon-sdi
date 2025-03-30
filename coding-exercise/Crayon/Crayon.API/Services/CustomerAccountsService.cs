using Crayon.Domain.Models;
using Crayon.Repository;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Crayon.API.Services;

public interface ICustomerAccountsService
{
    Task<Customer> GetCustomerAndAccounts(int customerId, CancellationToken ct);
    Task<List<Subscription>> GetSubscriptions(int customerId, int accountId, CancellationToken ct);
    Task<Either<CancelSubscriptionError, Unit>> CancelSubscription(int customerId, int subscriptionId, CancellationToken ct);
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

    public async Task<Either<CancelSubscriptionError, Unit>> CancelSubscription(int customerId, int subscriptionId, CancellationToken ct)
    {
        var subscription = await dbContext.Subscriptions
            .AsSingleQuery()
            .Include(s => s.Account)
            .SingleOrDefaultAsync(sub => sub.Id == subscriptionId, cancellationToken: ct);

        if (subscription is null)
            return CancelSubscriptionError.SubscriptionDoesNotExist;

        if (!SubscriptionBelongsToCustomer(customerId, subscription))
            return CancelSubscriptionError.SubscriptionDoesNotExist;
        
        // Could have been implemented to be idempotent, then this check wouldn't be performed
        if (subscription.Status == SubscriptionStatus.Cancelled) 
            return CancelSubscriptionError.SubscriptionAlreadyCanceled; 

        subscription.Status = SubscriptionStatus.Cancelled;
        subscription.UpdatedAt = DateTimeOffset.UtcNow;
        // if there is any side work to do, do it here. like calling ccp to cancel licences. Then some intermediate state od subscription should be added.
        
        await dbContext.SaveChangesAsync(ct);

        return Unit.Default;
        
        bool SubscriptionBelongsToCustomer(int customerId, Subscription subscription) => subscription.Account.CustomerId == customerId;
    }
    
    public async Task<Either<ChangeLicenceCountError, Unit>> ChangeLicenceCount(int customerId, int licenceId, int newLicenceCount, CancellationToken ct)
    {
        var licence = await dbContext.Licences
            .AsSingleQuery()
            .Include(l => l.Subscription)
            .ThenInclude(s => s.Account)
            .SingleOrDefaultAsync(l => l.Id == licenceId, cancellationToken: ct);

        if (licence is null)
            return ChangeLicenceCountError.LicenceDoesNotExist;

        if (!LicenceBelongsToCustomer(customerId, licence))
            return ChangeLicenceCountError.LicenceDoesNotExist;

        if (licence.LicenceCount == newLicenceCount)
            return ChangeLicenceCountError.NewLicenceCountCantBeSameAsExising;

        // do stuff required for getting new licences. Maybe new order?
        licence.LicenceCount = newLicenceCount;

        await dbContext.SaveChangesAsync(ct);

        return Unit.Default;
        
        bool LicenceBelongsToCustomer(int customerId, Licence licence) => licence.Subscription.Account.CustomerId == customerId;
    }
    
}

public enum ChangeLicenceCountError
{
    LicenceDoesNotExist,
    NewLicenceCountCantBeSameAsExising
}

public enum CancelSubscriptionError
{
    SubscriptionDoesNotExist,
    SubscriptionAlreadyCanceled
}