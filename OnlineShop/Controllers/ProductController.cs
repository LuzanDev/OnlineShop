using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Models;
using System.Diagnostics;
using OnlineShop.Views.ViewModel;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult AddProduct()
        {
            return PartialView();
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
    }


}
