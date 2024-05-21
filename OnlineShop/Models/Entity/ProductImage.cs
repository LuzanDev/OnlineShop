
using System.Text.Json.Serialization;

namespace OnlineShop.Models.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        [JsonIgnore]
        public byte[] Data { get; set; }
        public int Order { get; set; }
        public long ProductId { get; set; }
    }
}
