using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Train.Component.Management.Database.Entities;

namespace Train.Component.Management.Database.EntityConfiguration;

public class ItemQuantityConfiguration : IEntityTypeConfiguration<ItemQuantity>
{
    public void Configure(EntityTypeBuilder<ItemQuantity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.ItemId).IsRequired();
        
        builder.HasOne(x => x.Item)
            .WithOne(x => x.ItemQuantity)
            .HasForeignKey<ItemQuantity>(x => x.ItemId);
    }
}