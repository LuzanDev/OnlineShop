using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Models.Services;

namespace OnlineShop.Controllers
{
    public class CategoryController: Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var response = await _categoryService.GetAllCategory();
            if (response.IsSuccess)
            {
                var options = response.Data.Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name
                }).OrderBy(x => x.Text).ToList();
                return Json(options);
            }
            else
            {
                return Json(new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage });
            }
            //return Json(new { errorCode = 1, errorMessage = "Error from Category" });
        }
    }
}
