using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crayon.Domain.Models;

public class Customer
{
    [Key] [ForeignKey(nameof(User))] public int Id { get; set; }
    public required string Name { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}