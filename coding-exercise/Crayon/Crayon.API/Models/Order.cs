namespace Crayon.API.Models;

public record Order
{
    private Order(string Id, string AccountId, IEnumerable<OrderItem> Items, OrderStatus Status, string? ExternalOrderId = null, string? FailureReason = null)
    {
        this.Id = Id;
        this.AccountId = AccountId;
        this.Items = Items;
        this.Status = Status;
    }

    public static Order CreateAsInitialized(NewOrder newOrder)
    {
        string id = Guid.NewGuid().ToString();
        var items = newOrder.Items.Select(i => OrderItem.Create(i, id)).ToList();

        return new Order(id, newOrder.AccountId, items, OrderStatus.Initialized);
    }

    public Order AsCompleted(string externalOrderId) =>
        this with { Status = OrderStatus.Completed, ExternalOrderId = externalOrderId };
    
    public Order AsFailed(string failureReason) =>
        this with { Status = OrderStatus.Failed, FailureReason = failureReason };


    public string Id { get; init; }
    public string AccountId { get; init; }
    public IEnumerable<OrderItem> Items { get; init; }
    public OrderStatus Status { get; init; }
    public string? ExternalOrderId { get; init; }
    public string? FailureReason { get; init; }
}

public enum OrderStatus
{
    Initialized,
    Completed,
    Failed
}