using OnlineShop.Models.Enums;

namespace OnlineShop.Models.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public DateTime OrderDate { get; set; }
        public Status Status { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public decimal TotalAmount => OrderItems.Sum(item => item.Total);
    }
}
