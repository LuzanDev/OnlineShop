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

        [HttpPut]
        public async Task<IActionResult> Update(int id, string newNameBrand)
        {
            var response =  await _brandService.UpdateBrand(id, newNameBrand);
            if (response.IsSuccess)
            {
                return Ok(response.Data.Name);
            }
            else
            {
                return BadRequest(new { errorMessage = response.ErrorMessage });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(string nameBrand)
        {
            var response = await _brandService.CreateBrandAsync(nameBrand);
            if (response.IsSuccess)
            {
                return Ok(response.Data.Name);
            }
            else
            {
                return BadRequest(new { errorMessage = response.ErrorMessage });
            }
        }

        public async Task<bool> BrandAlreadyExists(string brandName)
        {
            return await _brandService.BrandAlreadExists(brandName);
        }

        public IActionResult Brands()
        {
            return PartialView();
        }

        public IActionResult CreateBrandForm()
        {
            return PartialView();
        }

       
    }
}
