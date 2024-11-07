using EllipticCurve.Utils;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Enums;
using OnlineShop.Models.Identity;
using OnlineShop.Models.Interfaces.Repository;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Views.ViewModel;

namespace OnlineShop.Models.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseRepository<Order> _orderRepository;

        public OrderService(IBaseRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<BaseResult<Order>> CreateOrder(CheckoutViewModel model)
        {
            try
            {
                var order = new Order()
                {
                    OrderDate = DateTime.UtcNow,
                    PaymentMethod = "при получении",
                    Status = Enums.Status.InProcessing,
                    UserId = model.Cart.UserId,
                    ShippingAddress = model.DeliveryType == Enums.DeliveryType.SelfPickup
    ? model.NovaPoshtaDepartment
    : $"{model.Street} дом. {model.HouseNumber} {(string.IsNullOrEmpty(model.ApartmentNumber) ? "" : $"кв. {model.ApartmentNumber}")}",
                    CityId = model.CityId
                };

                foreach (var item in model.Cart.CartItems)
                {
                    order.OrderItems.Add(new OrderItem()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                    });
                }

                var result = await _orderRepository.AddAsync(order);

                return new BaseResult<Order>()
                {
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new BaseResult<Order>()
                {
                    ErrorMessage = ex.InnerException?.Message ?? ex.Message,
                    ErrorCode = (int)ErrorCodes.DatabaseError
                };
            }

        }

        public async Task<CollectionResult<Order>> GetAllOrders(string userId)
        {
            try
            {
                var orders = await _orderRepository.GetAll()
                .Include(x => x.City)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.Images)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();
                if (orders.Count > 0)
                {
                    return new CollectionResult<Order>()
                    {
                        Data = orders,
                        Count = orders.Count
                    };
                }
                return new CollectionResult<Order>()
                {
                    ErrorMessage = "OrderCollectionNotFound",
                    ErrorCode = (int)ErrorCodes.OrderCollectionNotFound
                };
            }
            catch (Exception ex)
            {
                return new CollectionResult<Order>()
                {
                    ErrorMessage = ex?.InnerException?.Message ?? ex.Message,
                    ErrorCode = (int)ErrorCodes.DatabaseError
                };
            }
        }
        public IEnumerable<OrderViewModel> ConvertOrders(IEnumerable<Order> orders, ApplicationUser user)
        {
            return orders.Select(x => new OrderViewModel()
            {
                Id = x.Id,
                OrderDate = x.OrderDate,
                Status = x.Status,
                City = x.City.Name,
                FullUserName = $"{user.Surname} {user.Name}",
                OrderItems = x.OrderItems,
                UserEmailAddress = user.Email,
                UserNumberPhone = user.PhoneNumber,
                ShippingAddress = x.ShippingAddress
            });
        }

        public async Task<BaseResult<Order>> GetOrder(int id)
        {
            try
            {
                var order = await _orderRepository.GetAll()
                    .Include(x => x.City)
                    .Include(x => x.OrderItems)
                    .ThenInclude(x => x.Product)
                    .ThenInclude(x => x.Images)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (order == null)
                {
                    return new BaseResult<Order>()
                    {
                        ErrorMessage = "OrderNotFound",
                        ErrorCode = (int)ErrorCodes.OrderNotFound
                    };
                }
                return new BaseResult<Order>()
                {
                    Data = order
                };
            }
            catch (Exception ex)
            {
                return new BaseResult<Order>()
                {
                    ErrorMessage = ex?.InnerException?.Message ?? ex.Message,
                    ErrorCode = (int)ErrorCodes.DatabaseError
                };
            }
        }
        public async Task<OrderViewModel> CreateOrderViewModel(int orderId, ApplicationUser user)
        {
            var orderResponse = await GetOrder(orderId);
            var order = orderResponse.Data;

            return new OrderViewModel()
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                City = order.City.Name,
                FullUserName = $"{user.Surname} {user.Name}",
                OrderItems = order.OrderItems,
                UserEmailAddress = user.Email,
                UserNumberPhone = user.PhoneNumber,
                ShippingAddress = order.ShippingAddress,
            };
        }
    }
}
