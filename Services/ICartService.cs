﻿using FISTNESSGYM.Models.database;

namespace FISTNESSGYM.Services
{
    public interface ICartService
    {
        void AddToCart(string userId, Product product);
        void RemoveFromCart(string userId, int productId);
        List<CartItem> GetCartItems(string userId);
        decimal GetTotal(string userId);
        void ClearCart(string userId);
    }


}