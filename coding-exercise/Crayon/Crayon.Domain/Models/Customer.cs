namespace Crayon.Domain.Models;

public class Customer
{
    public int Id { get; init; }           
    public required string Name { get; init; }      

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}

public class User
{
    public int Id { get; init; }        
    public required string Email { get; init; }    
    public required int? CustomerId { get; set; } 

    // Navigation property
    public virtual Customer Customer { get; set; } = null!;
}

public class Account
{
    public int Id { get; init; }          
    public required int CustomerId { get; init; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class Order
{
    public int Id { get; init; }          
    public required int AccountId { get; init; }   
    public required OrderStatus Status { get; init; }   
    public required string? FailureReason { get; init; } 
    public required DateTimeOffset? CreatedAt { get; init; } 
    public required DateTimeOffset? UpdatedAt { get; init; }

    public virtual Account Account { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public int Id { get; init; }           
    public required int OrderId { get; init; }      
    public required int SoftwareId { get; init; }   
    public required int LicenceCount { get; init; } 
    public required DateTimeOffset? LicencedUntil { get; init; }

    public virtual Order Order { get; set; } = null!;
}

public class Subscription
{
    public int Id { get; init; }            
    public required string SoftwareName { get; init; } 
    public required SubscriptionStatus Status { get; init; }       
    public required DateTimeOffset CreatedAt { get; init; }

    public virtual ICollection<Licence> Licences { get; set; } = new List<Licence>();
}

public class Licence
{
    public int Id { get; init; }             
    public required int SubscriptionId { get; init; } 
    public required DateTimeOffset ValidTo { get; init; }   
    public required string LicenceKey { get; init; }

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
