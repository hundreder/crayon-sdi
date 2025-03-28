using Microsoft.EntityFrameworkCore;

namespace Crayon.Repository;

public class CrayonDbContext: DbContext
{
    public CrayonDbContext(DbContextOptions<CrayonDbContext> options) : base(options)
    {
        
    }
}