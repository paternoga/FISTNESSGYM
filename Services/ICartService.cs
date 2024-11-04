using FISTNESSGYM.Models.database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FISTNESSGYM.Services
{
    public interface ICartService
    {
        Task AddToCart(string userId, Product product, int quantity);

        void RemoveFromCart(string userId, int productId);

        List<CartItem> GetCartItems(string userId);

        decimal GetTotal(string userId);

        void ClearCart(string userId);

        Task<CartItem> GetCartItemAsync(string userId, int productId);

        Task AddCartItemAsync(CartItem cartItem);

        Task UpdateCartItemAsync(CartItem cartItem);

        Task<List<CartItem>> GetCartItemsAsync(string userId);

        Task RemoveFromCartAsync(string userId, int productId);

        Task ClearCartAsync(string userId);

        Task AddToCartAsync(string userId, CartItem item);

        Task PlaceOrderAsync(Order order, List<CartItem> cartItems);

    }
}
