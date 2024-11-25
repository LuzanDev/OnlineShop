using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;

namespace OnlineShop.Models.Interfaces.Services
{
    public interface IFavoriteService
    {
        Task<CollectionResult<Product>> GetAllFavorites(string userId);
        Task<BaseResult<FavoriteProduct>> AddProductToFavorites(long productId, string userId);
        Task<BaseResult<bool>> AddRangeProductsToFavorites(List<long> productIds, string userId);
        Task<BaseResult<FavoriteProduct>> DeleteProductFromFavorites(long productId, string userId);
        Task<BaseResult<int>> GetCountFavorite(string userId);
        Task<BaseResult<bool>> SyncFavorites(string userId, List<int> favoritesIdsClient);
        Task<BaseResult<IEnumerable<long>>> GetFavoriteProductIds(string userId);
    }
}
