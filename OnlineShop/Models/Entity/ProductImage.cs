
using System.Text.Json.Serialization;

namespace OnlineShop.Models.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string FileName { get; set; } 
        public int Order { get; set; }
        public long ProductId { get; set; }

        public string GetImageUrl()
        {
            return $"/images/products/{FileName}";
        }

        public string GetThumbnailUrl()
        {
            return $"/images/products/thumbnails/{FileName}";
        }
    }

}
