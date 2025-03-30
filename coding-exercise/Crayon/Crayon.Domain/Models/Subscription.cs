namespace Crayon.Domain.Models;

public class Subscription
{
    public int Id { get; set; }
    public required int SoftwareId { get; set; }
    public required string SoftwareName { get; set; } 
    public required SubscriptionStatus Status { get; set; }       
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public required int AccountId { get; set; }   

    public virtual Account Account { get; set; } = null!;
    public virtual ICollection<Licence> Licences { get; set; } = new List<Licence>();
}

public enum SubscriptionStatus
{
    Active,
    Cancelled
}
