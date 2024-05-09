using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Views.ViewModel;

namespace OnlineShop.Models.Interfaces.Services
{
    public interface IProductService
    {
        Task<CollectionResult<Product>> GetAllProducts();
        Task<BaseResult<Product>> CreateProductAsync(ProductViewModel model);
        
    }
}
