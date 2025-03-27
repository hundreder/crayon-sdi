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
        
        var softwareExists = await SoftwareExists(order.Items.Select(i => i.SoftwareId), ct);
        if (!softwareExists)
            return CreateOrderError.SoftwareNotFound;

        return Order.Create(order);
    }

    private async Task<bool> SoftwareExists(IEnumerable<string> softwareIds, CancellationToken ct)
    {
        var softwareList = await softwareCatalogService.GetSoftware(softwareIds, ct);
        return softwareList.Count() == softwareIds.Count(); //poor man's check
    }
}

public record Order(string Id, string AccountId, IEnumerable<OrderItem> Items)
{
    public static Order Create(NewOrder newOrder)
    {
        string id = Guid.NewGuid().ToString();
        var items = newOrder.Items.Select(i => OrderItem.Create(i, id)).ToList();
        
        return new Order(id, newOrder.AccountId, items);
    }
}

public record OrderItem(string Id, string OrderId, string SoftwareId, int LicenseCount, DateTime LicencedUntil)
{
    public static OrderItem Create(NewOrderItem newOrderItem, string orderId) => new OrderItem(
        Guid.NewGuid().ToString(),
        orderId,
        newOrderItem.SoftwareId,
        newOrderItem.LicenseCount,
        newOrderItem.LicencedUntil);

}

public record NewOrder(string CustomerId, string AccountId, IList<NewOrderItem> Items);

public record NewOrderItem(string SoftwareId, int LicenseCount, DateTime LicencedUntil);

public enum CreateOrderError
{
    AccountNotFound,
    SoftwareNotFound
}