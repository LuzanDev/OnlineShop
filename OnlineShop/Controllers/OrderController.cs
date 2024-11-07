using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Enums;
using OnlineShop.Models.Identity;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Views.ViewModel;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        
        [HttpGet]
        [Authorize]
        [Route("cabinet/orders")]
        public async Task<IActionResult> Orders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _orderService.GetAllOrders(userId);

            if (response.IsSuccess)
            {
                var user = await _userManager.FindByIdAsync(userId);
                var ordersModel = _orderService.ConvertOrders(response.Data, user);
                return View(ordersModel);
            }
            else if (response.ErrorCode == (int)ErrorCodes.OrderCollectionNotFound) 
            {
                return View(Enumerable.Empty<OrderViewModel>());
            }

            return BadRequest();
        }


    }
}
