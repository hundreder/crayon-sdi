namespace Crayon.Domain.Models;

public class Account
{
    public int Id { get; set; }          
    public required string Name { get; set; }   
    public required int CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}