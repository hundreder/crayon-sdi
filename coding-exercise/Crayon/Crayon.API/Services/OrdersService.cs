using Crayon.API.Models;
using LanguageExt;

namespace Crayon.API.Services;

public interface IOrdersService
{
    Task<Either<CreateOrderError, Order>> CreateOrder(NewOrder order, CancellationToken ct);
}

public class OrdersService(
    ICustomerAccountsService customerAccountsService,
    ISoftwareCatalogService softwareCatalogService) : IOrdersService
{
    public async Task<Either<CreateOrderError, Order>> CreateOrder(NewOrder order, CancellationToken ct)
    {
        var customerAccounts = await customerAccountsService.GetAccounts(order.CustomerId, ct);

        if (customerAccounts.All(ca => ca.Id != order.AccountId))
            return CreateOrderError.AccountNotFound;

        var softwareExists = await SoftwareExists(order
                .Items
                .Select(i => i.SoftwareId)
                .ToList(),
            ct);
        
        if (!softwareExists)
            return CreateOrderError.SoftwareNotFound;

        //make order trough CCP
        //if all ok save order and add software to account. It can be done async
        
        
        return Order.Create(order);
    }

    private async Task<bool> SoftwareExists(List<string> softwareIds, CancellationToken ct)
    {
        var softwareList = await softwareCatalogService.GetSoftware(softwareIds, ct);
        return softwareList.Count() == softwareIds.Count; //poor man's check. Ideally list of nonexistent ids should be returned
    }
}

public enum CreateOrderError
{
    AccountNotFound,
    SoftwareNotFound
}