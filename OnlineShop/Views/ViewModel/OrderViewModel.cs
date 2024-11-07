using OnlineShop.Models.Entity;
using OnlineShop.Models.Enums;

namespace OnlineShop.Views.ViewModel
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string City { get; set; }
        public Status Status { get; set; }
        public string ShippingAddress { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } 
        public decimal TotalAmount => OrderItems.Sum(item => item.Total);
        public string FullUserName { get; set; }
        public string UserNumberPhone { get; set; }
        public string UserEmailAddress { get; set; }
    }
}
