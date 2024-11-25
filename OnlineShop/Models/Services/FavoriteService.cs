using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Enums;
using OnlineShop.Models.Interfaces.Repository;
using OnlineShop.Models.Interfaces.Services;
using System.Collections.Generic;

namespace OnlineShop.Models.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IBaseRepository<FavoriteProduct> _favoritesRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IMemoryCache _cache;
        public FavoriteService(IBaseRepository<FavoriteProduct> favoritesRepository, IBaseRepository<Product> productRepository, IMemoryCache cache)
        {
            _favoritesRepository = favoritesRepository;
            _productRepository = productRepository;
            _cache = cache;
        }
        private async Task<List<Product>> LoadProductsAndSaveCacheAsync()
        {
            var products = await _productRepository.GetAll()
              .Include(x => x.Images)
              .Include(x => x.Brand)
              .Include(x => x.Category)
              .ToListAsync();

            foreach (var product in products)
            {
                product.Images = product.Images.OrderBy(image => image.Order).ToList();
            }

            _cache.Set("Products_All", products, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });
            return products;
        }
        private async Task<List<long>> LoadFavoritesCacheAsync(string userId)
        {
            var favoriteIds = await _favoritesRepository.GetAll()
                .Where(x => x.UserId == userId)
                .Select(x => x.ProductId).ToListAsync();
            
            return favoriteIds;
        }
        public async Task<BaseResult<FavoriteProduct>> AddProductToFavorites(long productId, string userId)
        {
            if (!_cache.TryGetValue($"FavoriteIds_{userId}", out List<long> favoriteIds))
            {
                favoriteIds = await LoadFavoritesCacheAsync(userId);
            }
            
            if (favoriteIds.Contains(productId))
            {
                return new BaseResult<FavoriteProduct>()
                {
                    ErrorMessage = "Товар уже находится в избранном!",
                    ErrorCode = (int)ErrorCodes.ProductAlreadyExistsFavorites
                };
            }

            var favoriteProduct = new FavoriteProduct
            {
                UserId = userId,
                ProductId = productId
            };

            try
            {
                var resultAddToFavorites = await _favoritesRepository.AddAsync(favoriteProduct);
                favoriteIds.Add(productId);

                _cache.Set($"FavoriteIds_{userId}", favoriteIds, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });

                return new BaseResult<FavoriteProduct>()
                {
                    Data = resultAddToFavorites
                };
            }
            catch (Exception ex)
            {
                return new BaseResult<FavoriteProduct>()
                {
                    ErrorMessage = "Произошла ошибка при добавлении продукта в избранное.\n" + ex.Message,
                    ErrorCode = (int)ErrorCodes.DatabaseError,
                };
            }
        }
        public async Task<BaseResult<FavoriteProduct>> DeleteProductFromFavorites(long productId, string userId)
        {
            try
            {
                if (!_cache.TryGetValue($"FavoriteIds_{userId}", out List<long> favoriteIds))
                {
                    favoriteIds = await LoadFavoritesCacheAsync(userId);
                }

                var removedItem = await _favoritesRepository.GetAll()
                    .Where(x => x.UserId == userId && x.ProductId == productId).FirstOrDefaultAsync();

                if (!favoriteIds.Contains(productId) || removedItem == null)
                {
                    return new BaseResult<FavoriteProduct>()
                    {
                        ErrorMessage = "Товар не находиться в избранном",
                        ErrorCode = (int)ErrorCodes.NoFavoritesFound
                    };
                }
                await _favoritesRepository.Delete(removedItem);
                favoriteIds.Remove(productId);

                _cache.Set($"FavoriteIds_{userId}", favoriteIds, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });

                return new BaseResult<FavoriteProduct>()
                {
                    Data = removedItem
                };
            }
            catch (Exception ex)
            {
                return new BaseResult<FavoriteProduct>()
                {
                    ErrorMessage = "Произошла ошибка при удалении избранного товара.\n" + ex.Message,
                    ErrorCode = (int)ErrorCodes.DatabaseError,
                };
            }
        }
        public async Task<CollectionResult<Product>> GetAllFavorites(string userId)
        {
            try
            {
                List<Product> products;
                List<long> favoriteIds;

                var productsTask = Task.Run(async () =>
                {
                    if (!_cache.TryGetValue("Products_All", out products))
                    {
                        products = await LoadProductsAndSaveCacheAsync();
                    }
                    return products;
                });

                var favoriteIdsTask = Task.Run(async () =>
                {
                    if (!_cache.TryGetValue($"FavoriteIds_{userId}", out favoriteIds))
                    {
                        favoriteIds = await LoadFavoritesCacheAsync(userId);

                        _cache.Set($"FavoriteIds_{userId}", favoriteIds, new MemoryCacheEntryOptions
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(10)
                        });
                    }
                    return favoriteIds;
                });

                await Task.WhenAll(productsTask, favoriteIdsTask);

                products = await productsTask;
                favoriteIds = await favoriteIdsTask;

                var result = products.Where(x => favoriteIds.Contains(x.Id)).ToList();
                if (result.Count < 1)
                {
                    return new CollectionResult<Product>()
                    {
                        ErrorMessage = "Ваш список избранного пуст.",
                        ErrorCode = (int)ErrorCodes.NoFavoritesFound
                    };
                }
                return new CollectionResult<Product>
                {
                    Data = result,
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                return new CollectionResult<Product>()
                {
                    ErrorMessage = "Произошла ошибка при получении избранных товаров.\n" + ex.Message,
                    ErrorCode = (int)ErrorCodes.DatabaseError,
                };
            }
        }
        public async Task<BaseResult<int>> GetCountFavorite(string userId)
        {
            if (!_cache.TryGetValue($"FavoriteIds_{userId}", out List<long> favoriteIds))
            {
                favoriteIds = await LoadFavoritesCacheAsync(userId);

                _cache.Set($"FavoriteIds_{userId}", favoriteIds, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });
            }

            return new BaseResult<int>()
            {
                Data = favoriteIds.Count
            };
        }
        public async Task<BaseResult<IEnumerable<long>>> GetFavoriteProductIds(string userId)
        {
            try
            {
                if (!_cache.TryGetValue($"FavoriteIds_{userId}", out List<long> favoriteIds))
                {
                    favoriteIds = await LoadFavoritesCacheAsync(userId);

                    _cache.Set($"FavoriteIds_{userId}", favoriteIds, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(10)
                    });
                }
                return new BaseResult<IEnumerable<long>>()
                {
                    Data = favoriteIds
                };
            }
            catch (Exception ex)
            {
                return new BaseResult<IEnumerable<long>>()
                {
                    ErrorCode = (int)ErrorCodes.DatabaseError,
                    ErrorMessage = ex.Message
                };
            }
        }
        public async Task<BaseResult<bool>> SyncFavorites(string userId, List<int> favoritesIdsClient)
        {
            if (favoritesIdsClient == null || favoritesIdsClient.Count < 1)
            {
                return new BaseResult<bool>()
                {
                    Data = true
                };
            }
            if (!_cache.TryGetValue($"FavoriteIds_{userId}", out List<long> favoriteIds))
            {
                favoriteIds = await LoadFavoritesCacheAsync(userId);
            }

            var favoriteIdsSet = new HashSet<long>(favoriteIds);

            var newFavorites = favoritesIdsClient.Where(id => !favoriteIdsSet.Contains(id))
                .Select(id => (long)id)
                .ToList();

            if (newFavorites.Count > 0)
            {
                // Добавляем все новые элементы массово
                var addResult = await AddRangeProductsToFavorites(newFavorites, userId);
                if (!addResult.IsSuccess)
                {
                    return new BaseResult<bool>
                    {
                        ErrorMessage = addResult.ErrorMessage
                    };
                }
                favoriteIds.AddRange(newFavorites);
                _cache.Set($"FavoriteIds_{userId}", favoriteIds, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });
            }

            return new BaseResult<bool>
            {
                Data = true
            };
        }
        public async Task<BaseResult<bool>> AddRangeProductsToFavorites(List<long> productIds, string userId)
        {
            try
            {
                var favorites = productIds.Select(id => new FavoriteProduct
                {
                    UserId = userId,
                    ProductId = id,
                }).ToList();

                await _favoritesRepository.AddRangeAsync(favorites);

                return new BaseResult<bool>
                {
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResult<bool>
                {
                    ErrorMessage = $"Ошибка при добавлении избранных товаров: {ex.Message}"
                };
            }
        }
    }
}
