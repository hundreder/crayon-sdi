using Crayon.Domain.Models;
using Crayon.Repository;
using Crayon.Repository.ApiClients;

namespace Crayon.Services.Services;

public interface IPurchaseService
{
    Task ProcessOrder(int accountId, CcpOrder externalOrder);
}

public class PurchaseService(CrayonDbContext dbContext) : IPurchaseService
{
    public async Task ProcessOrder(int accountId, CcpOrder externalOrder)
    {
        var licenceKeysBySoftwareId = externalOrder.Licences
            .GroupBy(licence => licence.SoftwareId)
            .ToDictionary(group => group.Key, group => group.Select(licence => licence));


        var subscriptions = licenceKeysBySoftwareId.Select(item => new Subscription()
            {
                AccountId = accountId,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = SubscriptionStatus.Active,
                SoftwareId = item.Key,
                SoftwareName = item.Key.ToString(),
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