using Crayon.API.ApiClients;
using Crayon.API.Models;
using LanguageExt;

namespace Crayon.API.Services;

public interface IOrdersService
{
    Task<Either<CreateOrderError, Order>> CreateOrder(NewOrder newOrder, CancellationToken ct);
}

public class OrdersService(
    ICustomerAccountsService customerAccountsService,
    ISoftwareCatalogService softwareCatalogService,
    ICcpApiClient ccpApiClient) : IOrdersService
{
    // public async Task<Either<CreateOrderError, Order>> CreateOrder(NewOrder newOrder, CancellationToken ct)
    // {
    //     //validation
    //     var customerAccounts = await customerAccountsService.GetCustomerAndAccounts(1, ct);
    //
    //     if (customerAccounts.All(ca => ca.Id != newOrder.AccountId))
    //         return CreateOrderError.AccountNotFound;
    //
    //     var softwareExists = await SoftwareExists(newOrder
    //             .Items
    //             .Select(i => i.SoftwareId)
    //             .ToList(),
    //         ct);
    //     
    //     if (!softwareExists)
    //         return CreateOrderError.SoftwareNotFound;
    //     
    //     
    //     //order processing
    //     var initializedOrder = Order.CreateAsInitialized(newOrder);
    //
    //     var order = 
    //         (await ccpApiClient.SendOrder(initializedOrder))
    //         .Map(externalOrderId =>
    //             {
    //                 var order = initializedOrder.AsCompleted(externalOrderId);
    //                 // repo.save order;
    //                 return order;
    //             }
    //         )
    //         .MapLeft(error =>
    //         {
    //             var order = initializedOrder.AsFailed(error.ToString());
    //             // repo.save order;
    //
    //             return error;
    //         });
    //
    //     return order;
    // }

    private async Task<bool> SoftwareExists(List<string> softwareIds, CancellationToken ct)
    {
        var softwareList = await softwareCatalogService.GetSoftware(softwareIds, ct);
        return softwareList.Count() == softwareIds.Count; //poor man's check. Ideally list of nonexistent ids should be returned
    }

    public Task<Either<CreateOrderError, Order>> CreateOrder(NewOrder newOrder, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}