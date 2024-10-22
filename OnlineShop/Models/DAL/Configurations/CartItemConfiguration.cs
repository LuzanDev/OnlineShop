using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models.Entity;

namespace OnlineShop.Models.DAL.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.HasOne(x => x.Product)
                   .WithMany() 
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Cart) 
               .WithMany(c => c.CartItems)
               .HasForeignKey(x => x.CartId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Quantity)
                   .IsRequired(); 
        }
    }
}
