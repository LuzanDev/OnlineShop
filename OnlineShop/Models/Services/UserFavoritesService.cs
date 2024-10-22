using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Enums;
using OnlineShop.Models.Identity;
using OnlineShop.Models.Interfaces.Repository;
using OnlineShop.Models.Interfaces.Services;
using System.Security.Claims;

namespace OnlineShop.Models.Services
{
    public class UserFavoritesService : IUserFavoritesService
    {
        private readonly IBaseRepository<FavoriteProduct> _favoritesRepository;
        private readonly IBaseRepository<Product> _productRepository;

        public UserFavoritesService(IBaseRepository<FavoriteProduct> favoritesRepository, IBaseRepository<Product> productRepository)
        {
            _favoritesRepository = favoritesRepository;
            _productRepository = productRepository;
        }

        public async Task<BaseResult<Product>> AddProductToFavorites(long productId, string userId)
        {
            var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
            {
                return new BaseResult<Product>()
                {
                    ErrorMessage = "Products not found",
                    ErrorCode = (int)ErrorCodes.ProductNotFound,
                };
            }

            var existingFavorite = await _favoritesRepository.GetAll()
            .FirstOrDefaultAsync(fp => fp.UserId == userId && fp.ProductId == product.Id);

            if (existingFavorite != null)
            {
                return new BaseResult<Product>()
                {
                    ErrorMessage = $"{product.Name} уже находится в избранном!",
                    ErrorCode = (int)ErrorCodes.ProductAlreadyExistsFavorites
                };
            }

            var favoriteProduct = new FavoriteProduct
            {
                UserId = userId,
                ProductId = product.Id
            };

            try
            {
                var result = await _favoritesRepository.AddAsync(favoriteProduct);
                return new BaseResult<Product>()
                {
                    Data = product
                };
            }
            catch (Exception ex)
            {
                return new BaseResult<Product>()
                {
                    ErrorMessage = "Произошла ошибка при добавлении продукта в избранное.\n" + ex.Message,
                    ErrorCode = (int)ErrorCodes.DatabaseError,
                };
            }
        }

        public async Task<BaseResult<Product>> DeleteProductFromFavorites(long productId, string userId)
        {
            try
            {
                var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == productId);
                if (product == null)
                {
                    return new BaseResult<Product>()
                    {
                        ErrorCode = (int)ErrorCodes.ProductNotFound,
                        ErrorMessage = "ProductNotFound"
                    };
                }

                var productFavorite = await _favoritesRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.ProductId == product.Id && x.UserId == userId);
                if (productFavorite == null)
                {
                    return new BaseResult<Product>()
                    {
                        ErrorMessage = "Товар не находиться в избранном",
                        ErrorCode = (int)ErrorCodes.NoFavoritesFound
                    };
                }

                await _favoritesRepository.Delete(productFavorite);

                return new BaseResult<Product>()
                {
                    Data = product
                };
            }
            catch (Exception ex)
            {
                return new BaseResult<Product>()
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
                var favoriteProducts = await _favoritesRepository.GetAll()
                                .Where(x => x.UserId == userId)
                                .ToListAsync();
                if (favoriteProducts == null || !favoriteProducts.Any())
                {
                    return new CollectionResult<Product>()
                    {
                        ErrorMessage = "Ваш список избранного пуст.",
                        ErrorCode = (int)ErrorCodes.NoFavoritesFound
                    };
                }
                var productsId = favoriteProducts.Select(x => x.ProductId).ToList();

                var products = await _productRepository.GetAll()
                                        .Include(x => x.Images)
                                        .Include(x => x.Brand)
                                        .Include(x => x.Category)
                                        .Where(p => productsId.Contains(p.Id))
                                        .ToListAsync();

                return new CollectionResult<Product>
                {
                    Data = products,
                    Count = products.Count
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
            var listFavoriteProducts = await _favoritesRepository.GetAll()
                .Where(x => x.UserId == userId).ToListAsync();

            return new BaseResult<int>()
            {
                Data = listFavoriteProducts.Count
            };
        }

        public async Task<BaseResult<IEnumerable<long>>> GetFavoriteProductIds(string userId)
        {
            try
            {
                var listFavoriteIds = await _favoritesRepository.GetAll()
                .Where(id => id.UserId == userId).Select(x => x.ProductId).ToListAsync();

                return new BaseResult<IEnumerable<long>>()
                {
                    Data = listFavoriteIds
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
    }
}
