namespace Crayon.Domain.Models;

public class Order
{
    public int Id { get; set; }          
    public required int AccountId { get; set; }   
    public required OrderStatus Status { get; set; }
    public string? ExternalOrderId { get; set; } 
    public string? FailureReason { get; set; } 
    public required DateTimeOffset? CreatedAt { get; set; } 
    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual Account Account { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    Initialized,
    Completed,
    Failed
}