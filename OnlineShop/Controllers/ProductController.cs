using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Models;
using System.Diagnostics;
using OnlineShop.Views.ViewModel;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Models.Services;

namespace OnlineShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
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




        [HttpPost]
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

        public IActionResult Products()
        {
            return PartialView();
        }

        public IActionResult AddProduct()
        {
            return PartialView();
        }
    }


}
