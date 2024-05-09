using Microsoft.EntityFrameworkCore;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Enums;
using OnlineShop.Models.Interfaces.Repository;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Views.ViewModel;

namespace OnlineShop.Models.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseRepository<Category> _categoryRepository;

        public CategoryService(IBaseRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<BaseResult<Category>> CreateCategoryAsync(CategoryViewModel model)
        {
            Category category = new Category()
            {
                Name = model.Name,
            };
            category = await _categoryRepository.AddAsync(category);
            return new BaseResult<Category>()
            {
                Data = category
            };
        }

        public async Task<BaseResult<Category>> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return new BaseResult<Category>()
                {
                    ErrorMessage = "CategoryNotFound",
                    ErrorCode = (int)ErrorCodes.CategoryNotFound
                };
            }
            else
            {
                _categoryRepository.Delete(category);
                return new BaseResult<Category>()
                {
                    Data = category
                };
            }
        }

        public async Task<CollectionResult<Category>> GetAllCategory()
        {
            var categories = await _categoryRepository.GetAll().ToListAsync();
            if (!categories.Any())
            {
                return new CollectionResult<Category>()
                {
                    ErrorMessage = "CategoryCollectionNotFound",
                    ErrorCode = (int)ErrorCodes.CategoryCollectionNotFound
                };
            }
            else
            {
                return new CollectionResult<Category>()
                {
                    Data = categories,
                    Count = categories.Count
                };
            }
        }

        public async Task<BaseResult<Category>> UpdateCategory(Category brand)
        {
            var category = await _categoryRepository.GetAll().Select(x => x.Name).ToListAsync();
            if (category.Contains(brand.Name))
            {
                return new BaseResult<Category>()
                {
                    ErrorMessage = "CategoryAlreadyExists",
                    ErrorCode = (int)ErrorCodes.CategoryAlreadyExists
                };
            }
            else
            {
                await _categoryRepository.Update(brand);
                return new BaseResult<Category>()
                {
                    Data = brand
                };
            }
        }
    }
}
