using Crayon.API.ApiClients;
using Crayon.API.Models;
using Crayon.Domain.Models;
using Crayon.Repository;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
namespace Crayon.API.Services;

public interface IOrdersService
{
    Task<Either<CreateOrderError, Order>> CreateOrder(NewOrder newOrder, CancellationToken ct);
}

public class OrdersService(
    CrayonDbContext dbContext,
    ISoftwareCatalogService softwareCatalogService,
    ICcpApiClient ccpApiClient) : IOrdersService
{
    public async Task<Either<CreateOrderError, Order>> CreateOrder(NewOrder newOrder, CancellationToken ct)
    {
        if (!newOrder.Items.Any())
            return CreateOrderError.NoItemsInOrder;

        
        var accountBelongsToUser = await AccountBelongsToUser(newOrder, ct);
        if (!accountBelongsToUser)
            // Logger.logwarning account does not belongs to a customer
            return CreateOrderError.AccountNotFound;

        if (!await OrderedSoftwareIsAvailable(newOrder, ct))
            return CreateOrderError.SoftwareNotFound;


        var initializedOrder = new Order()
        {
            AccountId = newOrder.AccountId,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = OrderStatus.Initialized,
            OrderItems = newOrder.Items
                .Select(item => new OrderItem()
                {
                    OrderId = 1,
                    LicenceCount = 3,
                    SoftwareId = item.SoftwareId,
                    LicenceValidTo = item.LicencedExpiration
                })
                .ToList()
        };

        dbContext.Add(initializedOrder);
        await dbContext.SaveChangesAsync(ct);

        var order = (await ccpApiClient.SendOrder(initializedOrder))
            .Map(externalOrderId =>
                {
                    initializedOrder.Status = OrderStatus.Completed;
                    initializedOrder.UpdatedAt = DateTimeOffset.UtcNow;
                    initializedOrder.ExternalOrderId = externalOrderId;
                    return initializedOrder;
                }
            )
            .MapLeft(error =>
            {
                // Logger.logwarning saving order as failed for analysis of what happend
                initializedOrder.Status = OrderStatus.Failed;
                initializedOrder.UpdatedAt = DateTimeOffset.UtcNow;
                initializedOrder.FailureReason = error.ToString();
                return error;
            });

        await dbContext.SaveChangesAsync(ct);

        return order;
    }

    private async Task<bool> AccountBelongsToUser(NewOrder newOrder, CancellationToken ct) =>
        await dbContext
            .Accounts
            .AnyAsync(ca => ca.Id == newOrder.AccountId
                            && ca.CustomerId == newOrder.CustomerId
                , cancellationToken: ct);


    private async Task<bool> OrderedSoftwareIsAvailable(NewOrder newOrder, CancellationToken ct)
    {
        var orderedSoftwareIds = newOrder.Items
            .Select(i => i.SoftwareId)
            .ToList();

        var existingSoftwareIds = await softwareCatalogService.GetSoftware(orderedSoftwareIds, ct);
        return existingSoftwareIds.Count() == orderedSoftwareIds.Count; //poor man's check. Ideally list of nonexistent ids should be returned
    }
}