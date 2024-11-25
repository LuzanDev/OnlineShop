using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnlineShop.Models.DTO;
using OnlineShop.Models.Interfaces.Services;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService, IMemoryCache cache)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var response = await _cartService.GetCart(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (response.IsSuccess)
            {
                return Json(new { data = response.Data });
            }
            else
            {
                return StatusCode(response.ErrorCode, response.ErrorMessage);
            }
        }

        [HttpPost] //Ужасно
        public async Task<IActionResult> SyncCartItem([FromBody] List<long> cartItemsId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _cartService.SyncCartItems(userId,cartItemsId);

            if (response.IsSuccess)
            {
                return Json(new { success = response.Data });
            }
            return StatusCode(500, new { success = response.Data, errorMessage = response.ErrorMessage });
        }


        [HttpPost]
        public async Task<IActionResult> GetCartItemsNotAuthorized([FromBody] List<CartItemDto> cartItems)
        {
            var response = await _cartService.MapCartItemsToCart(cartItems);
            if (response.IsSuccess)
            {
                return Json(new { data = response.Data });
            }
            else
            {
                return StatusCode(response.ErrorCode, response.ErrorMessage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            var response = await _cartService.GetCartItemCount(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (response.IsSuccess)
            {
                return Json(new { count = response.Data });
            }
            else
            {
                return StatusCode(response.ErrorCode, response.ErrorMessage);
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateItemQuantity([FromBody] UpdateQuantityDto model)
        {

            var response = await _cartService.UpdateCartItemQuantity(User.FindFirstValue(ClaimTypes.NameIdentifier), model.ProductId, model.Quantity);

            if (response.IsSuccess)
            {
                return Json(new { data = response.Data });
            }
            else
            {
                return StatusCode(response.ErrorCode, response.ErrorMessage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTotalAmount()
        {
            var response = await _cartService.GetTotalAmountCart(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (response.IsSuccess)
            {
                return Json(new { data = response.Data });
            }
            else
            {
                return StatusCode(response.ErrorCode, response.ErrorMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] long productId)
        {
            var response = await _cartService
            .AddItemToCart(User.FindFirstValue(ClaimTypes.NameIdentifier), productId);
            if (response.IsSuccess)
            {
                return Json(new { data = response.Data, success = true });
            }
            else
            {
                return StatusCode(500, new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct([FromBody] long productId)
        {
            var response = await _cartService
            .RemoveItemToCart(User.FindFirstValue(ClaimTypes.NameIdentifier), productId);
            if (response.IsSuccess)
            {
                return Json(new { data = response.Data, success = true });
            }
            else
            {
                return StatusCode(500, new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage });
            }
        }



        [HttpGet]
        [Route("cart")]
        public IActionResult Cart()
        {
            return View();
        }
    }
}
