using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.DTO;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Models.Services;
using SendGrid;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
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

        [HttpPost]
        public async Task<IActionResult> SyncCartItem([FromBody] List<long> cartItemsId)
        {
            if (cartItemsId == null || !cartItemsId.Any())
            {
                return Json(new { success = true });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Получить список id товаров которые уже в избранном (из базы данных)
            var CartItemsInDataBase = await _cartService.GetCartProductIds(userId);

            if (CartItemsInDataBase.IsSuccess)
            {
                // Cписок id продуктов, которые уже в избранном
                var resultListCartItemsInDataBase = CartItemsInDataBase.Data.ToList();

                foreach (var idProduct in cartItemsId)
                {
                    if (!resultListCartItemsInDataBase.Contains(idProduct))
                    {
                        var addResult = await _cartService.AddItemToCart(userId, idProduct);

                        if (!addResult.IsSuccess)
                        {
                            return StatusCode(500, new { success = false, errorMessage = addResult.ErrorMessage });
                        }
                    }
                }

                return Json(new { success = true });
            }
            else
            {
                return StatusCode(500, new { success = false, errorMessage = CartItemsInDataBase.ErrorMessage });
            }
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
