using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Entity;
using OnlineShop.Views.ViewModel;

namespace OnlineShop.Models.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<CollectionResult<Category>> GetAllCategory();
        Task<BaseResult<Category>> CreateCategoryAsync(string nameCategory);
        Task<BaseResult<Category>> UpdateCategory(int categoryId, string newNameCategory);
        Task<BaseResult<Category>> DeleteCategory(int id);
        Task<bool> CategoryAlreadExists(string categoryName);
    }
}
