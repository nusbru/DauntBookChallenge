using Daunt.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Daunt.Persistence.Context;

public class DauntDbContext(DbContextOptions<DauntDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DauntDbContext).Assembly);
    }
}

