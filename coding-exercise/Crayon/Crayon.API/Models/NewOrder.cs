
namespace Crayon.API.Models;

public record NewOrder(string CustomerId, string AccountId, IList<NewOrderItem> Items);