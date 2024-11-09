using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Views.ViewModel;

namespace OnlineShop.Models.Interfaces.Services
{
    public interface IProductService
    {
        Task<CollectionResult<Product>> GetAllProducts();
        Task<CollectionResult<Product>> GetProductsByCategoryId(int id);
        Task<CollectionResult<Product>> GetProductsByListId(List<long> listId);
        Task<BaseResult<Product>> DeleteProduct(long id);
        Task<BaseResult<Product>> CreateProductAsync(ProductViewModel model);
        Task<BaseResult<Product>> UpdateProduct(long id,ProductViewModel model);
        Task<BaseResult<Product>> GetProductById(long id);

        
    }
}
