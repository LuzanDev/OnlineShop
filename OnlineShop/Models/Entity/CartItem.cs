using System.Text.Json.Serialization;

namespace OnlineShop.Models.Entity
{
    public class CartItem
    {
        public int Id { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Total  => Quantity * Product.Price;

        public int CartId { get; set; }

        [JsonIgnore]
        public Cart Cart { get; set; } 
    }
}
