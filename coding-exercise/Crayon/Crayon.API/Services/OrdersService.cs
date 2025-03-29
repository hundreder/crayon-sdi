using Crayon.API.ApiClients;
using Crayon.API.Models;
using Crayon.Repository;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Order = Crayon.Domain.Models.Order;
using OrderItem = Crayon.Domain.Models.OrderItem;
using OrderStatus = Crayon.Domain.Models.OrderStatus;

namespace Crayon.API.Services;

public interface IOrdersService
{
    Task<Either<CreateOrderError, Order>> CreateOrder(NewOrder newOrder, CancellationToken ct);
}

public class OrdersService(
    CrayonDbContext dbContext,
    ICustomerAccountsService customerAccountsService,
    ISoftwareCatalogService softwareCatalogService,
    ICcpApiClient ccpApiClient) : IOrdersService
{
    public async Task<Either<CreateOrderError, Order>> CreateOrder(NewOrder newOrder, CancellationToken ct)
    {
        //validation if account belongs to customer
        bool accountBelongsToUser = await dbContext.Accounts.AnyAsync(ca => ca.Id == newOrder.AccountId && ca.CustomerId == newOrder.CustomerId, cancellationToken: ct);
        if (!accountBelongsToUser)
        {
            // Logger.logwarning account does not belongs to a customer
            return CreateOrderError.AccountNotFound;
        }

        var softwareExists = await SoftwareExists(newOrder
                .Items
                .Select(i => i.SoftwareId)
                .ToList(),
            ct);

        if (!softwareExists)
            return CreateOrderError.SoftwareNotFound;

        //order processing
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
                initializedOrder.Status = OrderStatus.Failed;
                initializedOrder.UpdatedAt = DateTimeOffset.UtcNow;
                initializedOrder.FailureReason = error.ToString();
                return error;
            });

        await dbContext.SaveChangesAsync(ct);

        return order;
    }
    
    private async Task<bool> SoftwareExists(List<int> softwareIds, CancellationToken ct)
    {
        var softwareList = await softwareCatalogService.GetSoftware(softwareIds, ct);
        return softwareList.Count() == softwareIds.Count; //poor man's check. Ideally list of nonexistent ids should be returned
    }
}