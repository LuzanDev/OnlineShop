namespace OnlineShop.Models.Entity
{
    public class OrderItem
    {
        public int Id { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Quantity * Product.Price; 
        public Product Product { get; set; } 
    }
}
