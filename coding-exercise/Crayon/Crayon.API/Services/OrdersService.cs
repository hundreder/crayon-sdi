using Crayon.API.ApiClients;
using Crayon.API.Models;
using Crayon.API.Services.Events;
using Crayon.Domain.Models;
using Crayon.Repository;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Crayon.API.Services;

public interface IOrdersService
{
    Task<Either<CreateOrderError, CompletedOrder>> CreateOrder(NewOrder newOrder, CancellationToken ct);
}

public class OrdersService(
    CrayonDbContext dbContext,
    ISoftwareCatalogService softwareCatalogService,
    IMediator mediator,
    ICcpApiClient ccpApiClient) : IOrdersService
{
    public async Task<Either<CreateOrderError, CompletedOrder>> CreateOrder(NewOrder newOrder, CancellationToken ct)
    {
        if (!newOrder.Items.Any())
            return CreateOrderError.NoItemsInOrder;

        
        var accountBelongsToUser = await AccountBelongsToUser(newOrder, ct);
        if (!accountBelongsToUser)
            // Logger.logwarning account does not belongs to a customer
            return CreateOrderError.AccountNotFound;

        if (!await OrderedSoftwareIsAvailable(newOrder, ct))
            return CreateOrderError.SoftwareNotFound;


        var order = new Order()
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

        dbContext.Add(order);
        await dbContext.SaveChangesAsync(ct);

        var result =  (await ccpApiClient.SendOrder(order))
            .Map(ccpOrder =>
                {
                    order.Status = OrderStatus.Completed;
                    order.UpdatedAt = DateTimeOffset.UtcNow;
                    order.ExternalOrderId = ccpOrder.Id;
                    
                    return (order, ccpOrder);
                }
            )
            .MapLeft(error =>
            {
                // Logger.logwarning saving order as failed for analysis of what happend
                order.Status = OrderStatus.Failed;
                order.UpdatedAt = DateTimeOffset.UtcNow;
                order.FailureReason = error.ToString();
                return error;
            });

        await dbContext.SaveChangesAsync(ct);

        return await result.MapAsync(async o =>
        {
            await mediator.Publish(new CompletedOrderEvent(newOrder.AccountId, o.ccpOrder), ct);
            return new CompletedOrder(o.order.Id, o.ccpOrder.Id);
        });
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

public record CompletedOrder(int OrderId, string CcpOrderId);