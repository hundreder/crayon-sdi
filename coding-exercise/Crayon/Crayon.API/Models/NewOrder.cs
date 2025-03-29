
namespace Crayon.API.Models;

public record NewOrder(int CustomerId, int AccountId, IEnumerable<NewOrderItem> Items);