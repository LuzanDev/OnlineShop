﻿using OnlineShop.Models.DTO;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;

namespace OnlineShop.Models.Interfaces.Services
{
    public interface ICartService
    {
        Task<BaseResult<Cart>> GetCart(string userId);
        Task<BaseResult<Cart>> GetCartById(int id);
        Task<BaseResult<decimal>> GetTotalAmountCart(string id);
        Task<BaseResult<Cart>> AddItemToCart(string userId, long productId);
       // Task<BaseResult<bool>> AddRangeItemsToCart(List<long> productIds, string userId);
        Task<BaseResult<Cart>> RemoveItemToCart(string userId, long productId);
        Task<BaseResult<decimal>> UpdateCartItemQuantity(string userId, long productId, int quantity);
        Task<BaseResult<int>> GetCartItemCount(string userId);
        Task<BaseResult<IEnumerable<long>>> GetCartProductIds(string userId);
        Task<BaseResult<Cart>> MapCartItemsToCart(List<CartItemDto> cartItems);
        Task<BaseResult<bool>> SyncCartItems(string userId, List<long> cartItemsIdsClient);
    }
}
