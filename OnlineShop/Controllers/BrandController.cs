using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Models.Interfaces.Services;

namespace OnlineShop.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBrands()
        {
            var response = await _brandService.GetAllBrands();
            if (response.IsSuccess)
            {
                var options = response.Data.Select(brand => new SelectListItem
                {
                    Value = brand.Id.ToString(),
                    Text = brand.Name
                }).OrderBy(x => x.Text).ToList();
                return Json(options);
            }
            else
            {
                return StatusCode(500, new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage });
            }
            //return StatusCode(500, new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _brandService.DeleteBrand(id);
            if (response.IsSuccess)
            {
                return Ok(response.Data.Name); 
            }
            else
            {
                return StatusCode(500, new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage });
            }
        }

        public IActionResult GetListBrands()
        {
            return PartialView();
        }
    }
}
