namespace Crayon.API.Models;

public record Order(string Id, string AccountId, IEnumerable<OrderItem> Items)
{
    public static Order Create(NewOrder newOrder)
    {
        string id = Guid.NewGuid().ToString();
        var items = newOrder.Items.Select(i => OrderItem.Create(i, id)).ToList();
        
        return new Order(id, newOrder.AccountId, items);
    }
}