using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Enums;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Views.ViewModel;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IFavoriteService _userFavoritesService;

        public ProductController(IProductService productService, IFavoriteService userFavoritesService, ICartService cartService)
        {
            _productService = productService;
            _userFavoritesService = userFavoritesService;
            _cartService = cartService;
        }

        [HttpGet]
        [Route("product/get-all-products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllProducts();
            if (response.IsSuccess)
            {
                return Json(new { data = response.Data, count = response.Count });
            }
            else
            {
                return StatusCode(500, new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage });
            }
        }

        [HttpDelete]
        [Route("Product/Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _productService.DeleteProduct(id);
            if (response.IsSuccess)
            {
                return Ok(response.Data.Name);
            }
            else
            {
                return StatusCode(500, new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage });
            }
        }

        [HttpPut]
        [Route("Product/Update")]
        public async Task<IActionResult> Update(long id, ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.UpdateProduct(id, model);
                return Json(new { success = true, productName = response.Data.Name });
            }
            else
            {
                return Json(new { errorMessage = "Форма заполнена не корректными данными" });
            }
        }

        [HttpPost]
        [Route("product/create")]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProductAsync(model);
                return Json(new { success = true, productName = response.Data.Name });
            }
            else
            {
                return Json(new { errorMessage = "Форма заполнена не корректными данными" });
            }
        }

        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> Products(int? categoryId)
        {
            var responseProduct = categoryId.HasValue
            ? await _productService.GetProductsByCategoryId(categoryId.Value)
            : await _productService.GetAllProducts();

            if (responseProduct.IsSuccess)
            {
                var listFavorite = new List<long>();

                if (User.Identity.IsAuthenticated)
                {
                    var listFavoriteIds = await _userFavoritesService.GetFavoriteProductIds(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (listFavoriteIds.IsSuccess)
                    {
                        listFavorite = listFavoriteIds.Data.ToList();
                    }
                }

                var model = new ProductsViewModel()
                {
                    Products = responseProduct.Data,
                    FavoriteIds = listFavorite
                };

                return View(model);
            }
            else if (responseProduct.ErrorCode == (int)ErrorCodes.ProductCollectionNotFound)
            {
                return View(new ProductsViewModel()
                {
                    Products = new List<Product>(),
                    FavoriteIds = new List<long>()
                });
            }
            return BadRequest();

        }

        [HttpGet]
        [Route("product/{id:long}")]
        public async Task<IActionResult> Product([FromRoute] long id)
        {
            var response = await _productService.GetProductById(id);
            if (!response.IsSuccess)
            {
               return View("Error", new ErrorViewModel()
               {
                   RequestId = response.ErrorMessage 
               });
            }

            var listFavorite = new List<long>();
            var productIdsInCart = new List<long>();

            var isFavorite = false;
            var inCart = false;
            if (User.Identity.IsAuthenticated)
            {
                var responseListFavoriteIds = await _userFavoritesService
                    .GetFavoriteProductIds(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var responseProductListIdsInCart = await _cartService
                    .GetCartProductIds(User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (responseListFavoriteIds.IsSuccess && responseProductListIdsInCart.IsSuccess)
                {
                    listFavorite = responseListFavoriteIds.Data.ToList();
                    productIdsInCart = responseProductListIdsInCart.Data.ToList();
                    isFavorite = listFavorite.Contains(id);
                    inCart = productIdsInCart.Contains(id);
                }
            }
            var model = new ProductPageViewModel()
            {
                Product = response.Data,
                isFavorite = isFavorite,
                inCart = inCart
            };

            return View(model);
        }

        [HttpGet]
        [Route("product/add-product-form")]
        public IActionResult AddProductForm()
        {
            return PartialView();
        }

        [HttpGet]
        [Route("product/management")]
        public IActionResult ProductManagement()
        {
            return PartialView();
        }


        [HttpPost]
        [Route("product/get-products-by-listid")]
        public async Task<IActionResult> GetProductsByListId([FromBody] List<long> listId)
        {
            var response = await _productService.GetProductsByListId(listId);
            if (response.IsSuccess)
            {
                return Json(response.Data);
            }
            else
            {
                return StatusCode(response.ErrorCode, response.ErrorMessage);
            }
        }
    }


}