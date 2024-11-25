using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Interfaces.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace OnlineShop.Controllers
{

    public class FavoritesController : Controller
    {
        private readonly IFavoriteService _userFavoritesService;

        public FavoritesController(IFavoriteService userFavoritesService, IProductService productService)
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _userFavoritesService.SyncFavorites(userId, favoriteProductIds);

            if (response.IsSuccess)
            {
                return Json(new { success = response.Data });
            }
            else
            {
                return StatusCode(500, new { success = response.Data, errorMessage = response.ErrorMessage });
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
        public async Task<ActionResult> DeleteFavorites([FromBody] long productId)
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