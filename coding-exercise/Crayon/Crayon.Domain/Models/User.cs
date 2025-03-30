namespace Crayon.Domain.Models;

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    
    public virtual Customer? Customer { get; set; }
}