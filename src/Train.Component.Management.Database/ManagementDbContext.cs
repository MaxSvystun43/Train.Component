using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Train.Component.Management.Database.Configurations;
using Train.Component.Management.Database.Entities;
using Train.Component.Management.Database.EntityConfiguration;

namespace Train.Component.Management.Database;

public class ManagementDbContext(DbContextOptions options, IOptions<DatabaseConfiguration> schemaName) : DbContext(options)
{
    private string SchemaName => schemaName.Value.SchemaName;
    
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemQuantity> ItemQuantities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItemConfiguration).Assembly);
    }
}