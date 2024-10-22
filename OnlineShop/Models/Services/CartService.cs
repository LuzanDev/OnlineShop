using Microsoft.EntityFrameworkCore;
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

        public CartService(IBaseRepository<Cart> cartRepository, IBaseRepository<Product> productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<BaseResult<Cart>> AddItemToCart(string userId, long productId)
        {
            var cart = await _cartRepository.GetAll()
                .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
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

            var cartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
            if (cartItem != null)
            {
                return new BaseResult<Cart>()
                {
                    ErrorMessage = "ProductAlreadyInCart",
                    ErrorCode = (int)ErrorCodes.ProductAlreadyInCart
                };
            }

            var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == productId);
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

            return new BaseResult<Cart>()
            {
                Data = cart
            };
        }
        public async Task<BaseResult<Cart>> GetCart(string id)
        {

            var cart = await _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(x => x.Images)
                .FirstOrDefaultAsync(x => x.UserId == id);

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

        public async Task<BaseResult<Cart>> MapCartItemsToCart(List<CartItemDto> cartItems)
        {
            var cart = new Cart();

            var cartItemsResult = new List<CartItem>();

            foreach (var item in cartItems)
            {
                var product = await _productRepository.GetAll()
                    .Include(x => x.Images)
                    .FirstOrDefaultAsync(x => x.Id == item.ProductId);

                if (product == null)
                {
                    return new BaseResult<Cart>()
                    {
                        ErrorMessage = "ProductNotFound",
                        ErrorCode = (int)ErrorCodes.ProductNotFound
                    };
                }

                cartItemsResult.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Product = product,
                });
            }

            cart.CartItems = cartItemsResult;

            return new BaseResult<Cart>()
            {
                Data = cart
            };
        }


        public async Task<BaseResult<int>> GetCartItemCount(string userId)
        {
            var cart = await _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                return new BaseResult<int>
                {
                    ErrorMessage = "CartNotFound",
                    ErrorCode = (int)ErrorCodes.CartNotFound
                };
            }
            var cartItemCount = cart?.CartItems.Count ?? 0;

            return new BaseResult<int>()
            {
                Data = cartItemCount
            };
        }

        public async Task<BaseResult<IEnumerable<long>>> GetCartProductIds(string userId)
        {
            try
            {
                var cart = await _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

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
            var cart = await _cartRepository.GetAll()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (cart == null)
            {
                return new BaseResult<decimal>
                {
                    ErrorMessage = "CartNotFound",
                    ErrorCode = (int)ErrorCodes.CartNotFound
                };
            }
            return new BaseResult<decimal>()
            {
                Data = cart.TotalAmount
            };
        }

        public async Task<BaseResult<Cart>> RemoveItemToCart(string userId, long productId)
        {
            var cart = await _cartRepository.GetAll()
                .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.Images)
                .FirstOrDefaultAsync(x => x.UserId == userId);
            if (cart == null)
            {
                return new BaseResult<Cart>
                {
                    ErrorMessage = "CartNotFound",
                    ErrorCode = (int)ErrorCodes.CartNotFound
                };
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





            return new BaseResult<Cart>()
            {
                Data = cart
            };
        }

        public async Task<BaseResult<decimal>> UpdateCartItemQuantity(string userId, long productId, int quantity)
        {
            var cart = await _cartRepository.GetAll()
                .Include(x => x.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
            {
                return new BaseResult<decimal>
                {
                    ErrorMessage = "CartNotFound",
                    ErrorCode = (int)ErrorCodes.CartNotFound
                };
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

            return new BaseResult<decimal>
            {
                Data = cartItem.Total
            };
        }
    }
}
