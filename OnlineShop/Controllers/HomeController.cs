using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Models.Interfaces.Services;
using System.Diagnostics;

namespace OnlineShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _productService.GetAllProducts();
            if (response.IsSuccess) 
            {
                return View(response.Data);
            }
            else
            {

            }
            ///Обработать
            throw new NotImplementedException();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
