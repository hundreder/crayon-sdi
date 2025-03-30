using MediatR;

namespace Crayon.API.Services.Events;

public class CompletedOrderHandler(IPurchaseService purchaseService): INotificationHandler<CompletedOrderEvent>
{
    public async Task Handle(CompletedOrderEvent notification, CancellationToken cancellationToken)
    {
        await purchaseService.ProcessOrder(notification.AccountId, notification.Order);
    }
}