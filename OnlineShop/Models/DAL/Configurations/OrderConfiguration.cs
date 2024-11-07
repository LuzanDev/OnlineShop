using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.Models.Entity;

namespace OnlineShop.Models.DAL.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            
            builder.Property(o => o.UserId)
                .IsRequired(); 

            builder.Property(o => o.OrderDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); 

            builder.Property(o => o.Status)
                .IsRequired(); 

            builder.Property(o => o.CityId)
                .IsRequired(); 

            builder.Property(o => o.ShippingAddress)
                .IsRequired()
                .HasMaxLength(250); 

            builder.Property(o => o.PaymentMethod)
                .IsRequired()
                .HasMaxLength(100); 
            
            builder.HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey(o => o.OrderId) 
                .OnDelete(DeleteBehavior.Cascade); 
            
            builder.HasIndex(o => o.UserId); 
        }
    }

}
