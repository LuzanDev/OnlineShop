using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OnlineShop.Models.DTO;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Enums;
using OnlineShop.Models.Interfaces.Repository;
using OnlineShop.Models.Interfaces.Services;

namespace OnlineShop.Models.Services
{
    public class CartService : ICartService
    {
        private readonly IBaseRepository<Cart> _cartRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IMemoryCache _cache;

        public CartService(IBaseRepository<Cart> cartRepository, IBaseRepository<Product> productRepository, IMemoryCache cache)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _cache = cache;
        }

        public async Task<BaseResult<Cart>> AddItemToCart(string userId, long productId)
        {
            var cacheCartKey = $"Cart_{userId}";
            var cacheProductsKey = "Products_All";

            if (!_cache.TryGetValue(cacheCartKey, out Cart cart))
            {
                cart = await LoadCart(userId);
            }

            var cartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
            if (cartItem != null)
            {
                return new BaseResult<Cart>()
                {
                    ErrorMessage = "ProductAlreadyInCart",
                    ErrorCode = (int)ErrorCodes.ProductAlreadyInCart
                };
            }


            if (!_cache.TryGetValue(cacheProductsKey, out List<Product> products))
            {
                products = await _productRepository.GetAll()
                    .Include(x => x.Images)
                    .Include(x => x.Brand)
                    .Include(x => x.Category)
                    .ToListAsync();

                foreach (var item in products)
                {
                    item.Images = item.Images.OrderBy(image => image.Order).ToList();
                }

                _cache.Set(cacheProductsKey, products, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            }


            var product = products.FirstOrDefault(x => x.Id == productId);
            if (product == null)
            {
                return new BaseResult<Cart>()
                {
                    ErrorMessage = "ProductNotFound",
                    ErrorCode = (int)ErrorCodes.ProductNotFound
                };
            }

            cartItem = new CartItem()
            {
                Product = product,
                ProductId = product.Id,
                Quantity = 1,
                CartId = cart.Id,
                Cart = cart,
            };

            cart.CartItems.Add(cartItem);

            await _cartRepository.Update(cart);

            _cache.Set(cacheCartKey, cart, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            });

            return new BaseResult<Cart>()
            {
                Data = cart
            };
        }
        public async Task<BaseResult<Cart>> GetCart(string userId)
        {
            var cacheKey = $"Cart_{userId}";

            if (!_cache.TryGetValue(cacheKey, out Cart cart))
            {
                cart = await LoadCart(userId);

                _cache.Set(cacheKey, cart, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });
            }

            return new BaseResult<Cart>()
            {
                Data = cart
            };
        }
        public async Task<BaseResult<Cart>> MapCartItemsToCart(List<CartItemDto> cartItems)
        {
            var cacheProductsKey = "Products_All";

            if (!_cache.TryGetValue(cacheProductsKey, out List<Product> products))
            {

                products = await _productRepository.GetAll()
                    .Include(x => x.Images)
                    .Include(x => x.Brand)
                    .Include(x => x.Category)
                    .ToListAsync();

                foreach (var product in products)
                {
                    product.Images = product.Images.OrderBy(image => image.Order).ToList();
                }

                _cache.Set(cacheProductsKey, products, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            }

            var productDictionary = products.ToDictionary(x => x.Id, x => x);

            var cartItemsResult = cartItems.Select(item =>
            {
                if (!productDictionary.TryGetValue(item.ProductId, out var product))
                {
                    return null;
                }

                return new CartItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Product = product,
                };
            }).Where(cartItem => cartItem != null).ToList();

            if (cartItemsResult.Count < cartItems.Count)
            {
                return new BaseResult<Cart>()
                {
                    ErrorMessage = "ProductNotFound",
                    ErrorCode = (int)ErrorCodes.ProductNotFound
                };
            }

            return new BaseResult<Cart>()
            {
                Data = new Cart { CartItems = cartItemsResult }
            };
        }
        public async Task<BaseResult<int>> GetCartItemCount(string userId)
        {
            var cacheKey = $"Cart_{userId}";

            if (!_cache.TryGetValue(cacheKey, out Cart cart))
            {
                cart = await LoadCart(userId);

                _cache.Set(cacheKey, cart, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });
            }

            return new BaseResult<int>()
            {
                Data = cart?.CartItems.Count ?? 0
            };
        }
        public async Task<BaseResult<IEnumerable<long>>> GetCartProductIds(string userId)
        {
            var cacheKey = $"Cart_{userId}";

            if (!_cache.TryGetValue(cacheKey, out Cart cart))
            {
                cart = await LoadCart(userId);

                _cache.Set(cacheKey, cart, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });
            }

            try
            {
                var listCartProductIds = cart?.CartItems.Select(ci => ci.ProductId).ToList() ?? new List<long>();

                return new BaseResult<IEnumerable<long>>()
                {
                    Data = listCartProductIds
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
        public async Task<BaseResult<decimal>> GetTotalAmountCart(string id)
        {
            var cacheKey = $"Cart_{id}";

            if (!_cache.TryGetValue(cacheKey, out Cart cart))
            {
                cart = await LoadCart(id);

                _cache.Set(cacheKey, cart, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });
            }

            return new BaseResult<decimal>()
            {
                Data = cart.TotalAmount
            };
        }
        public async Task<BaseResult<Cart>> RemoveItemToCart(string userId, long productId)
        {
            var cacheKey = $"Cart_{userId}";

            if (!_cache.TryGetValue(cacheKey, out Cart cart))
            {
                cart = await LoadCart(userId);
            }
            else
            {
                _cartRepository.Attach(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
            if (cartItem == null)
            {
                return new BaseResult<Cart>()
                {
                    ErrorMessage = "ProductAbsentInCart",
                    ErrorCode = (int)ErrorCodes.ProductAbsentInCart
                };
            }

            cart.CartItems.Remove(cartItem);

            await _cartRepository.Update(cart);

            _cache.Set(cacheKey, cart, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            });


            return new BaseResult<Cart>()
            {
                Data = cart
            };
        }
        public async Task<BaseResult<decimal>> UpdateCartItemQuantity(string userId, long productId, int quantity)
        {
            var cacheKey = $"Cart_{userId}";

            if (!_cache.TryGetValue(cacheKey, out Cart cart))
            {
                cart = await LoadCart(userId);
            }


            var cartItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem == null)
            {
                return new BaseResult<decimal>
                {
                    ErrorMessage = "CartItemNotFound",
                    ErrorCode = (int)ErrorCodes.CartItemNotFound
                };
            }

            cartItem.Quantity = quantity;

            await _cartRepository.Update(cart);

            _cache.Set(cacheKey, cart, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            });

            return new BaseResult<decimal>
            {
                Data = cartItem.Total
            };
        }
        public async Task<BaseResult<Cart>> GetCartById(int id)
        {
            var cart = await _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (cart == null)
            {
                return new BaseResult<Cart>
                {
                    ErrorMessage = "CartNotFound",
                    ErrorCode = (int)ErrorCodes.CartNotFound
                };
            }
            return new BaseResult<Cart>()
            {
                Data = cart
            };
        }
        private async Task<Cart> LoadCart(string userId)
        {
            var cart = await _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(x => x.Images)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
            {
                cart = new Cart()
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                await _cartRepository.AddAsync(cart);
            }

            return cart;
        }
        public async Task<BaseResult<bool>> SyncCartItems(string userId, List<long> cartItemsIdsClient)
        {

            var cartCacheKey = $"Cart_{userId}";
            if (!_cache.TryGetValue(cartCacheKey, out Cart cart))
            {
                cart = await LoadCart(userId);
            }
            var productIdsSet = new HashSet<long>(cart.CartItems.Select(p => p.ProductId));

            var newListCartItemsIds = cartItemsIdsClient.Where(id => !productIdsSet.Contains(id)).ToList();

            await AddRangeItemsToCart(newListCartItemsIds, cart);

            return new BaseResult<bool>
            {
                Data = true
            };
        }
        public async Task<BaseResult<bool>> AddRangeItemsToCart(List<long> productIds, Cart cart)
        {
            var cartItems = productIds
                .Select(id => new CartItem
                {
                    ProductId = id,
                    Quantity = 1,
                    CartId = cart.Id,
                });

            foreach (var item in cartItems)
            {
                cart.CartItems.Add(item);
            }
            await _cartRepository.Update(cart);
            var result = await GetCartById(cart.Id);
            if (result.IsSuccess)
            {
                cart = result.Data;
            }
            else
            {
                return new BaseResult<bool>()
                {
                    ErrorMessage = result.ErrorMessage
                };
            }

            _cache.Set($"Cart_{cart.UserId}", cart, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            });

            return new BaseResult<bool>
            {
                Data = true
            };
        }
    }
}
