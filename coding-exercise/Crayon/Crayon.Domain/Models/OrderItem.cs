namespace Crayon.Domain.Models;

public class OrderItem
{
    public int Id { get; set; }           
    public int OrderId { get; set; }      
    public required int SoftwareId { get; set; }   
    public required int LicenceCount { get; set; } 
    public required DateTimeOffset LicenceValidTo { get; set; }

    public virtual Order Order { get; set; } = null!;
}