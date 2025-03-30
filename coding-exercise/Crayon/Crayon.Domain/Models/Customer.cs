using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crayon.Domain.Models;

public class Customer
{
    [Key]
    [ForeignKey(nameof(User))]
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }

    // Navigation property for one-to-one relationship
    public virtual Customer? Customer { get; set; }
}

public class Account
{
    public int Id { get; set; }          
    public required string Name { get; set; }   
    public required int CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}

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

public class OrderItem
{
    public int Id { get; set; }           
    public int OrderId { get; set; }      
    public required int SoftwareId { get; set; }   
    public required int LicenceCount { get; set; } 
    public required DateTimeOffset LicenceValidTo { get; set; }

    public virtual Order Order { get; set; } = null!;
}

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

public class Licence
{
    public int Id { get; set; }             
    public int SubscriptionId { get; set; } 
    public required DateTimeOffset ValidTo { get; set; }
    public required int LicenceCount { get; set; }
    public required string LicenceKey { get; set; }

    public virtual Subscription Subscription { get; set; } = null!;
}

public enum OrderStatus
{
    Initialized,
    Completed,
    Failed
}

public enum SubscriptionStatus
{
    Active,
    Cancelled
}
