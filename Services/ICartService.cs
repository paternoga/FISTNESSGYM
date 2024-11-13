using FISTNESSGYM.Models.database;
using FISTNESSGYM.Models.Database;
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
        Task<int> GetTotalSoldTodayAsync();
        Task<List<SalesData>> GetSalesDataForLastDaysAsync(int days = 7);
        Task<List<OrderItem>> GetOrderItemsSoldTodayAsync();
        Task<List<SalesData>> GetSalesDataForWeekAsync();
        Task<List<SalesData>> GetSalesDataForPeriodAsync(DateTime startDate, DateTime endDate);
        Task<List<SalesData>> GetSalesDataForMonthAsync(int year, int month);



    }
}
