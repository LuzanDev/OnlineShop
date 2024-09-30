using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;

namespace OnlineShop.Models.Interfaces.Services
{
    public interface IUserFavoritesService
    {
        Task<CollectionResult<Product>> GetAllFavorites(string userId);
        Task<BaseResult<Product>> AddProductToFavorites(long productId, string userId);
        Task<BaseResult<Product>> DeleteProductFromFavorites(long productId, string userId);
        Task<BaseResult<int>> GetCountFavorite(string userId);
        Task<BaseResult<IEnumerable<long>>> GetFavoriteProductIds(string userId);
    }
}
