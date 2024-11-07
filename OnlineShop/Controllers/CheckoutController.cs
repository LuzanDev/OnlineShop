using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Identity;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Views.ViewModel;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ICartService cartService, UserManager<ApplicationUser> userManager, IOrderService orderService)
        {
            _cartService = cartService;
            _userManager = userManager;
            _orderService = orderService;
        }

        [Authorize]
        [HttpGet]
        [Route("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);
            var responseCartService = await _cartService.GetCart(userId);
            var cart = responseCartService.Data;


            CheckoutViewModel model = new CheckoutViewModel()
            {
                CartId = cart.Id,
                Cart = cart,
                UserName = user.Name,
                UserSurname = user.Surname,
                UserEmail = user.Email,
                UserPhoneNumber = user.PhoneNumber,

            };

            return PartialView(model);
        }

        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cartResponse = await _cartService.GetCartById(model.CartId);
            if (!cartResponse.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Не удалось получить корзину. Пожалуйста, попробуйте позже.");
                return PartialView(model);
            }

            model.Cart = cartResponse.Data;
            
            if (!ModelState.IsValid)
            {
                return PartialView(model);
            }

            var result = await _orderService.CreateOrder(model);
            if (result.IsSuccess)
            {
                var user = await _userManager.FindByIdAsync(result.Data.UserId);
                var claims = new List<Claim>();
                if (user.Surname == null)
                {
                    user.Surname = model.UserSurname;
                    claims.Add(new Claim(ClaimTypes.Surname, model.UserSurname));
                }
                if (user.PhoneNumber == null)
                {
                    user.PhoneNumber = model.UserPhoneNumber;
                    claims.Add(new Claim(ClaimTypes.MobilePhone, model.UserPhoneNumber));
                }
                if (claims.Any())
                {
                    await _userManager.AddClaimsAsync(user, claims);
                    await _userManager.UpdateAsync(user);
                }

                return RedirectToAction("OrderConfirmation", new { id = result.Data.Id, userId = result.Data.UserId });
            }


            ModelState.AddModelError(string.Empty, "Не удалось создать заказ. Пожалуйста, попробуйте позже.");
            return PartialView(model);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int id, string userId)
        {
            var user =  await _userManager.FindByIdAsync(userId);
            var model = await _orderService.CreateOrderViewModel(id, user);
            
            
            return View(model);
        }

        
    }
}
