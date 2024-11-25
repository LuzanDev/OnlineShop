using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Identity;
using OnlineShop.Views.ViewModel;

namespace OnlineShop.Models.Interfaces.Services
{
    public interface IOrderService
    {
        Task<CollectionResult<Order>> GetAllOrders(string userId);
        Task<BaseResult<Order>> GetOrder(int id, string userId);
        Task<BaseResult<Order>> CreateOrder(CheckoutViewModel model);
        Task<OrderViewModel> CreateOrderViewModel(int orderId, ApplicationUser user);
        IEnumerable<OrderViewModel> ConvertOrders(IEnumerable<Order> orders, ApplicationUser user);

    }
}
