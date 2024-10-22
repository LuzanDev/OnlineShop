using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Models;
using System.Diagnostics;
using OnlineShop.Views.ViewModel;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Models.Services;
using OnlineShop.Models.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace OnlineShop.Controllers
{
    
    public class FavoritesController : Controller
    {
        private readonly IUserFavoritesService _userFavoritesService;

        public FavoritesController(IUserFavoritesService userFavoritesService, IProductService productService)
        {
            _userFavoritesService = userFavoritesService;
        }

        [HttpGet]
        public async Task<ActionResult> GetFavorites()
        {
            var response = await _userFavoritesService.GetAllFavorites(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (response.IsSuccess)
            {
                return Json(new { data = response.Data, count = response.Count });
            }
            else
            {
                return StatusCode(response.ErrorCode, response.ErrorMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SyncFavorites([FromBody] List<int> favoriteProductIds)
        {
            if (favoriteProductIds == null || !favoriteProductIds.Any())
            {
                return Json(new { success = true });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Получить список id товаров которые уже в избранном (из базы данных)
            var responseFavorites = await _userFavoritesService.GetFavoriteProductIds(userId);

            if (responseFavorites.IsSuccess)
            {
                // Cписок id продуктов, которые уже в избранном
                var existingFavorites = responseFavorites.Data.ToList();

                foreach (var idProduct in favoriteProductIds)
                {
                    if (!existingFavorites.Contains(idProduct))
                    {
                        var addResult = await _userFavoritesService.AddProductToFavorites(idProduct, userId);

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
                return StatusCode(500, new { success = false, errorMessage = responseFavorites.ErrorMessage });
            }
        }




        [HttpGet]
        public async Task<ActionResult> GetFavoriteCount()
        {
            var response = await _userFavoritesService.GetCountFavorite(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return Json(new { count = response.Data });
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddFavorites([FromBody] long productId)
        {
            var response = await _userFavoritesService
            .AddProductToFavorites(productId, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (response.IsSuccess)
            {
                return Json(new { data = response.Data, success = true });
            }
            else
            {
                return StatusCode(500, new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage });
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> DeleteFavorites([FromBody]long productId)
        {
            var response = await _userFavoritesService
            .DeleteProductFromFavorites(productId, User.FindFirstValue(ClaimTypes.NameIdentifier));
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
        [Route("favorites")]
        public IActionResult Favorites()
        {
            return View();
        }
    }


    
}
