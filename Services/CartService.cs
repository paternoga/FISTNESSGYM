using FISTNESSGYM.Data;
using FISTNESSGYM.Models.database;
using FISTNESSGYM.Models.Database;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FISTNESSGYM.Services
{
    public class CartService : ICartService
    {
        private readonly databaseContext _context;

        public CartService(databaseContext context)
        {
            _context = context;
        }

        public async Task AddToCart(string userId, Product product, int quantity)
        {
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == product.Id);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                _context.CartItems.Update(existingCartItem); 
            }
            else
            {
                var newCartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = product.Id,
                    Quantity = quantity
                };
                await _context.CartItems.AddAsync(newCartItem); 
            }

            await _context.SaveChangesAsync();
        }

        public void RemoveFromCart(string userId, int productId)
        {
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.UserId == userId && ci.ProductId == productId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }
        }

        public List<CartItem> GetCartItems(string userId)
        {
            return _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product) 
                .ToList();
        }

        public decimal GetTotal(string userId)
        {
            return _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Sum(ci => ci.Quantity * ci.Product.Price); 
        }

        public void ClearCart(string userId)
        {
            var cartItems = _context.CartItems.Where(ci => ci.UserId == userId).ToList();
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();
        }

        public async Task<CartItem> GetCartItemAsync(string userId, int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
        }

        public async Task AddCartItemAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CartItem>> GetCartItemsAsync(string userId)
        {
            return await _context.CartItems
                                 .Where(ci => ci.UserId == userId)
                                 .Include(ci => ci.Product) 
                                 .ToListAsync();
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            var cartItem = await _context.CartItems
                                          .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var cartItems = await _context.CartItems
                                           .Where(ci => ci.UserId == userId)
                                           .ToListAsync();
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task AddToCartAsync(string userId, CartItem item)
        {
            var existingItem = await _context.CartItems
                                              .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                item.UserId = userId;
                await _context.CartItems.AddAsync(item);
            }

            await _context.SaveChangesAsync();
        }

        public async Task PlaceOrderAsync(Order order, List<CartItem> cartItems)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price,
                    CreationDate = DateTime.Now
                };

                await _context.OrderItems.AddAsync(orderItem);
                
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product != null)
                {
                    if (product.StockQuantity >= cartItem.Quantity)
                    {
                        product.StockQuantity -= cartItem.Quantity; 
                    }
                    else
                    {
                        throw new InvalidOperationException("Niewystarczająca ilość produktu w magazynie.");
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        //Analityka sklepu///////
        public async Task<int> GetTotalSoldTodayAsync()
        {
            var todayStart = DateTime.Today;
            var todayEnd = todayStart.AddDays(1).AddSeconds(-1); 

            var totalSoldToday = await _context.OrderItems
                .Where(oi => oi.CreationDate >= todayStart && oi.CreationDate <= todayEnd) 
                .SumAsync(oi => oi.Quantity); 

            return totalSoldToday;
        }

        public async Task<List<SalesData>> GetSalesDataForLastDaysAsync(int days = 7)
        {
            var todayStart = DateTime.Today;
            var startDate = todayStart.AddDays(-days); 

            var salesData = await _context.OrderItems
                .Where(oi => oi.CreationDate >= startDate && oi.CreationDate < todayStart)
                .GroupBy(oi => oi.CreationDate.Date) 
                .Select(g => new SalesData
                {
                    Date = g.Key.ToString("yyyy-MM-dd"), 
                    Quantity = g.Sum(oi => oi.Quantity) 
                })
                .OrderBy(s => s.Date) 
                .ToListAsync();

            return salesData;
        }

        public async Task<List<OrderItem>> GetOrderItemsSoldTodayAsync()
        {
            var allOrderItems = await _context.OrderItems.ToListAsync();

            return allOrderItems.Where(item => item.CreationDate.Date == DateTime.Today).ToList();
        }

        public async Task<List<SalesData>> GetSalesDataForWeekAsync()
        {
            var startDate = DateTime.Today.AddDays(-6);
            var endDate = DateTime.Today;

            var sales = await _context.OrderItems
                .Where(item => item.CreationDate >= startDate && item.CreationDate <= endDate)
                .GroupBy(item => item.CreationDate.Date) 
                .Select(group => new
                {
                    Date = group.Key, 
                    Quantity = group.Sum(item => item.Quantity)
                })
                .ToListAsync();

            var salesData = sales.Select(x => new SalesData
            {
                Date = x.Date.ToString("yyyy-MM-dd"), 
                Quantity = x.Quantity
            }).ToList();

            return salesData;
        }

        public async Task<List<SalesData>> GetSalesDataForPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var salesData = await _context.OrderItems
                .Where(o => o.CreationDate >= startDate && o.CreationDate <= endDate)
                .GroupBy(o => o.CreationDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Quantity = g.Sum(item => item.Quantity)
                })
                .ToListAsync();

            return salesData
                .Select(g => new SalesData
                {
                    Date = g.Date.ToString("yyyy-MM-dd"),
                    Quantity = g.Quantity
                })
                .ToList();
        }

        public async Task<List<SalesData>> GetSalesDataForMonthAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var salesDataQuery = await _context.OrderItems
                .Where(o => o.CreationDate >= startDate && o.CreationDate <= endDate)
                .GroupBy(o => o.CreationDate.Day) 
                .Select(g => new SalesData
                {
                    Date = g.Key.ToString(), 
                    Quantity = g.Sum(item => item.Quantity)
                })
                .ToListAsync();

            var allDaysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                .Select(day => new SalesData
                {
                    Date = day.ToString(), 
                    Quantity = 0 
                })
                .ToList();

            foreach (var dayData in salesDataQuery)
            {
                var day = int.Parse(dayData.Date);
                var existingDay = allDaysInMonth.FirstOrDefault(d => d.Date == day.ToString());
                if (existingDay != null)
                {
                    existingDay.Quantity = dayData.Quantity;
                }
            }

            return allDaysInMonth;
        }

        public async Task<List<TopProduct>> GetTopProductsForMonthAsync(int year, int month)
        {
            return await _context.OrderItems
                .Where(item => item.CreationDate.Year == year && item.CreationDate.Month == month)
                .GroupBy(item => new { item.ProductId, item.Product.Name }) 
                .Select(group => new TopProduct
                {
                    ProductId = group.Key.ProductId,
                    ProductName = group.Key.Name,
                    TotalQuantity = group.Sum(item => item.Quantity)
                })
                .OrderByDescending(product => product.TotalQuantity)
                .Take(5)
                .ToListAsync();
        }

        public async Task<int> GetTotalSoldForMonthAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1); 

            var totalSoldForMonth = await _context.OrderItems
                .Where(oi => oi.CreationDate >= startDate && oi.CreationDate <= endDate)
                .SumAsync(oi => oi.Quantity); 

            return totalSoldForMonth;
        }
        public async Task<decimal> GetTotalRevenueForMonthAsync(int year, int month)
        {
            var totalSoldMonth = await _context.OrderItems
                .Where(oi => oi.CreationDate.Year == year && oi.CreationDate.Month == month)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice);

            return totalSoldMonth;
        }

        public async Task<List<CategorySalesData>> GetCategoryRankingForMonthAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

            var ranking = await _context.OrderItems
                .Where(o => o.CreationDate >= startDate && o.CreationDate <= endDate)
                .Join(_context.Products,
                      orderItem => orderItem.ProductId,
                      product => product.Id,
                      (orderItem, product) => new { orderItem.Quantity, product.Category })
                .GroupBy(x => x.Category)
                .Select(group => new CategorySalesData
                {
                    CategoryName = group.Key,
                    TotalQuantity = group.Sum(x => x.Quantity)
                })
                .OrderByDescending(data => data.TotalQuantity)
                .ToListAsync();

            return ranking;
        }

        public async Task<List<OrderItem>> GetOrderItemsSoldForMonthAsync(int year, int month)
        {
            return await _context.OrderItems
                .Where(oi => oi.CreationDate.Year == year && oi.CreationDate.Month == month)
                .ToListAsync();
        }










    }
}
