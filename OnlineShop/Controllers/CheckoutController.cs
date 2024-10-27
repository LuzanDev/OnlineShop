using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Identity;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Views.ViewModel;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
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
                Cart = cart,
                UserName = user.Name,
                UserSurname = user.Surname,
                UserEmail = user.Email,
                UserPhoneNumber = user.PhoneNumber,
            };
            return PartialView(model);
        }
    }
}
