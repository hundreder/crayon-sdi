using Crayon.API.ApiClients;
using Crayon.Domain.Models;
using Crayon.Repository;

namespace Crayon.API.Services;

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
            CreatedAt = DateTimeOffset.Now,
            Status = SubscriptionStatus.Active,
            SoftwareId = item.Key,
            SoftwareName = item.Key.ToString(),
            Licences = item.Value.Select(licence => new Licence()
                {
                    LicenceKey = licence.LicenceKey,
                    ValidTo = licence.LicenceValidTo,
                })
                .ToList()
        });

        dbContext.Add(subscriptions);
        
        await dbContext.SaveChangesAsync();
}
}