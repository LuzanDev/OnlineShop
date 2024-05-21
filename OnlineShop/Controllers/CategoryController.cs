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
                return StatusCode(500, new { errorCode = response.ErrorCode, errorMessage = response.ErrorMessage });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(string nameCategory)
        {
            var response = await _categoryService.CreateCategoryAsync(nameCategory);
            if (response.IsSuccess)
            {
                return Ok(response.Data.Name);
            }
            else
            {
                return BadRequest(new { errorMessage = response.ErrorMessage });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _categoryService.DeleteCategory(id);
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
        public async Task<IActionResult> Update(int id, string newNameCategory)
        {
            var response = await _categoryService.UpdateCategory(id, newNameCategory);
            if (response.IsSuccess)
            {
                return Ok(response.Data.Name);
            }
            else
            {
                return BadRequest(new { errorMessage = response.ErrorMessage });
            }
        }

        public async Task<bool> CategoryAlreadyExists(string categoryName)
        {
            return await _categoryService.CategoryAlreadExists(categoryName);
        }

        public IActionResult Categories()
        {
            return PartialView();
        }

        public IActionResult CreateCategoryForm()
        {
            return PartialView();
        }
    }
}
