namespace Crayon.Domain.Models;

public class Licence
{
    public int Id { get; set; }             
    public int SubscriptionId { get; set; } 
    public required DateTimeOffset ValidTo { get; set; }
    public required int LicenceCount { get; set; }
    public required string LicenceKey { get; set; }

    public virtual Subscription Subscription { get; set; } = null!;
}