using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Entity;
using OnlineShop.Views.ViewModel;

namespace OnlineShop.Models.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<CollectionResult<Category>> GetAllCategory();
        Task<BaseResult<Category>> CreateCategoryAsync(CategoryViewModel model);
        Task<BaseResult<Category>> UpdateCategory(Category brand);
        Task<BaseResult<Category>> DeleteCategory(int id);
    }
}
