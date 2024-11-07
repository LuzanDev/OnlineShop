using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Interfaces.Services;

namespace OnlineShop.Controllers
{
    public class CityController : Controller
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }


        [HttpGet]
        [Route("get-cities")]
        public async Task<IActionResult> GetCities()
        {
            var response = await _cityService.GetAllCity();
            if (response.IsSuccess)
            {
                return Json(new { success = true, data = response.Data, count = response.Count });
            }
            return StatusCode(500, new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage });
        }
    }
}
