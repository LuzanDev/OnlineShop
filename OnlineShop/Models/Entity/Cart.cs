namespace OnlineShop.Models.Entity
{
    public class Cart
    {
        public int Id { get; set; } 
        public string UserId { get; set; } 
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>(); 
        public decimal TotalAmount => CartItems.Sum(item => item.Total);
    }
}
