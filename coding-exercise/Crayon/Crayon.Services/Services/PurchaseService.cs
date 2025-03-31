using Crayon.Domain.Models;
using Crayon.Repository;
using Crayon.Repository.ApiClients;
using Crayon.Services.Common;

namespace Crayon.Services.Services;

public interface IPurchaseService
{
    Task ProcessOrder(int accountId, CcpOrder externalOrder);
}

public class PurchaseService(CrayonDbContext dbContext, IDateTimeProvider dateTimeProvider) : IPurchaseService
{
    public async Task ProcessOrder(int accountId, CcpOrder externalOrder)
    {
        var licenceKeysBySoftwareId = externalOrder.Licences
            .GroupBy(licence => licence.Software)
            .ToDictionary(group => group.Key, group => group.Select(licence => licence));


        var subscriptions = licenceKeysBySoftwareId.Select(item => new Subscription()
            {
                AccountId = accountId,
                CreatedAt = dateTimeProvider.UtcNow,
                Status = SubscriptionStatus.Active,
                SoftwareId = item.Key.Id,
                SoftwareName = item.Key.Name,
                Licences = item.Value.Select(licence => new Licence()
                    {
                        LicenceKey = licence.LicenceKey,
                        LicenceCount = licence.LicenceCount,
                        ValidTo = licence.LicenceValidTo,
                    })
                    .ToList()
            })
            .ToList();

        dbContext.Subscriptions.AddRange(subscriptions);
        
        await dbContext.SaveChangesAsync();
    }
}