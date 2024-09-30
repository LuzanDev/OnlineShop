using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.Models.Entity;

namespace OnlineShop.Models.DAL.Configurations
{
    public class FavoriteProductConfiguration:IEntityTypeConfiguration<FavoriteProduct>
    {
        public void Configure(EntityTypeBuilder<FavoriteProduct> builder)
        {
            builder.Property(fp => fp.UserId).IsRequired();
            builder.Property(fp => fp.ProductId).IsRequired();

            // Уникальный индекс для предотвращения дубликатов (один пользователь - один товар)
            builder.HasIndex(fp => new { fp.UserId, fp.ProductId }).IsUnique();
        }
    }
}
