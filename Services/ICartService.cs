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
        Task<List<TopProduct>> GetTopProductsForMonthAsync(int year, int month);
        Task<int> GetTotalSoldForMonthAsync(int year, int month);
        Task<decimal> GetTotalRevenueForMonthAsync(int year, int month);
        Task<List<CategorySalesData>> GetCategoryRankingForMonthAsync(int year, int month);

        Task<List<OrderItem>> GetOrderItemsSoldForMonthAsync(int year, int month);
        Task<List<TopBuyer>> GetTopBuyersForMonthAsync(int year, int month);
        Task<LastPurchasedProduct> GetLastPurchasedProductAsync();
        Task<List<TopProduct>> GetLeastSoldProductsForMonthAsync(int year, int month);
        Task<int> GetTotalSoldForYearAsync(int year);
        Task<List<SalesData>> GetSalesDataForYearAsync(int year);
        Task<List<SalesData>> GetSalesDataForQuarterAsync(int year, int quarter);
        Task<List<TopProduct>> GetTopProductsForYearAsync(int year);
        Task<decimal> GetTotalRevenueForYearAsync(int year);
        Task<List<CategorySalesData>> GetCategoryRankingForYearAsync(int year);
        Task<List<TopBuyer>> GetTopBuyersForYearAsync(int year);
        Task<List<OrderItem>> GetOrderItemsSoldForYearAsync(int year);
        Task<List<TopProduct>> GetLeastSoldProductsForYearAsync(int year);
        Task<List<OrderItem>> GetOrderItemsForYearAsync(int year);
        Task<Dictionary<string, List<TopBuyer>>> GetTopBuyersWithQuartersAsync(int year);


    }
}
