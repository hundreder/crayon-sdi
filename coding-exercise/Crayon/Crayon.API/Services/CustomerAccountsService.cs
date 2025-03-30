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
    Task<Either<CancelSubscriptionError, Unit>> CancelSubscription(int customerId, int subscriptionId, CancellationToken ct);
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
    }

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

        if (subscription.Status == SubscriptionStatus.Cancelled)
            return CancelSubscriptionError.SubscriptionAlreadyCanceled;

        subscription.Status = SubscriptionStatus.Cancelled;

        subscription.UpdatedAt = DateTimeOffset.UtcNow;
        
        await dbContext.SaveChangesAsync(ct);

        return Unit.Default;
    }

    private bool SubscriptionBelongsToCustomer(int customerId, Subscription subscription) => subscription.Account.CustomerId == customerId;
    private bool LicenceBelongsToCustomer(int customerId, Licence licence) => licence.Subscription.Account.CustomerId == customerId;
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