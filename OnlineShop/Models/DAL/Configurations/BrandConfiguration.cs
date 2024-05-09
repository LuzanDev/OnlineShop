using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.Models.Entity;

namespace OnlineShop.Models.DAL.Configurations
{
    public class BrandConfiguration:IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).HasMaxLength(50);    
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }


}
