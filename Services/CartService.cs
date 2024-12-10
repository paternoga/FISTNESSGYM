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

        public async Task<List<TopBuyer>> GetTopBuyersForMonthAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var topBuyers = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => o.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalSpent = g.Sum(o => o.TotalAmount),
                    OrdersCount = g.Count()
                })
                .OrderByDescending(g => g.TotalSpent)
                .Take(5)
                .ToListAsync();

            // Pobierz dane użytkowników
            var userIds = topBuyers.Select(b => b.UserId).ToList();
            var users = await _context.AspNetUsers
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName, u.Email })
                .ToDictionaryAsync(u => u.Id);

            // Połącz dane
            return topBuyers.Select(b => new TopBuyer
            {
                UserName = users[b.UserId]?.UserName ?? "Nieznany użytkownik",
                Email = users[b.UserId]?.Email ?? "Brak e-maila",
                TotalSpent = b.TotalSpent,
                OrdersCount = b.OrdersCount
            }).ToList();
        }


        public async Task<LastPurchasedProduct> GetLastPurchasedProductAsync()
        {
            var lastOrderItem = await _context.OrderItems
                .OrderByDescending(oi => oi.CreationDate)
                .FirstOrDefaultAsync();

            if (lastOrderItem == null)
                return null;

            var productName = _context.Products
                .Where(p => p.Id == lastOrderItem.ProductId)
                .Select(p => p.Name)
                .FirstOrDefault();

            return new LastPurchasedProduct
            {
                ProductName = productName,
                Quantity = lastOrderItem.Quantity,
                UnitPrice = lastOrderItem.UnitPrice,
                CreationDate = lastOrderItem.CreationDate
            };
        }

        public async Task<List<TopProduct>> GetLeastSoldProductsForMonthAsync(int year, int month)
        {
            var leastSoldProducts = await _context.OrderItems
                .Where(o => o.CreationDate.Year == year && o.CreationDate.Month == month)  // Filtracja po roku i miesiącu
                .GroupBy(o => new { o.ProductId, o.Product.Name })  // Grupowanie po ID produktu oraz nazwie
                .Select(g => new TopProduct
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalQuantity = g.Sum(o => o.Quantity)  // Liczenie sumy zamówionych produktów
                })
                .OrderBy(p => p.TotalQuantity)  // Sortowanie po najmniejszej ilości
                .Take(5)  // Wybieranie 5 najrzadziej kupowanych
                .ToListAsync();

            return leastSoldProducts;
        }


        public async Task<int> GetTotalSoldForYearAsync(int year)
        {
            var totalSoldForYear = await _context.OrderItems
                .Where(oi => oi.CreationDate.Year == year)
                .SumAsync(oi => oi.Quantity);

            return totalSoldForYear;
        }

        public async Task<List<SalesData>> GetSalesDataForYearAsync(int year)
        {
            var salesData = await _context.OrderItems
                .Where(oi => oi.CreationDate.Year == year)
                .GroupBy(oi => oi.CreationDate.Month)
                .Select(g => new SalesData
                {
                    Date = g.Key.ToString(), // Miesiąc jako numer
                    Quantity = g.Sum(oi => oi.Quantity)
                })
                .OrderBy(s => s.Date)
                .ToListAsync();

            // Wypełnienie brakujących miesięcy (jeśli są)
            var allMonthsInYear = Enumerable.Range(1, 12)
                .Select(month => new SalesData
                {
                    Date = month.ToString(),
                    Quantity = 0
                })
                .ToList();

            foreach (var monthData in salesData)
            {
                var month = int.Parse(monthData.Date);
                var existingMonth = allMonthsInYear.FirstOrDefault(d => d.Date == month.ToString());
                if (existingMonth != null)
                {
                    existingMonth.Quantity = monthData.Quantity;
                }
            }

            return allMonthsInYear;
        }


        public async Task<List<SalesData>> GetSalesDataForQuarterAsync(int year, int quarter)
        {
            // Obliczanie zakresu daty dla danego kwartału
            var startMonth = (quarter - 1) * 3 + 1;
            var endMonth = startMonth + 2;
            var startDate = new DateTime(year, startMonth, 1);
            var endDate = new DateTime(year, endMonth, DateTime.DaysInMonth(year, endMonth));

            // Pobieranie danych z bazy
            var salesData = await _context.OrderItems
                .Where(oi => oi.CreationDate >= startDate && oi.CreationDate <= endDate)
                .GroupBy(oi => oi.CreationDate.Date)
                .Select(g => new SalesData
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Quantity = g.Sum(oi => oi.Quantity)
                })
                .OrderBy(s => s.Date)
                .ToListAsync();

            // Obsługuje brakujące dni
            var missingDays = Enumerable.Range(1, (endDate - startDate).Days)
                .Where(d => !salesData.Any(s => s.Date == startDate.AddDays(d - 1).ToString("yyyy-MM-dd")))
                .Select(d => new SalesData
                {
                    Date = startDate.AddDays(d - 1).ToString("yyyy-MM-dd"),
                    Quantity = 0
                }).ToList();

            // Dodanie brakujących dni (jeśli istnieją)
            salesData.AddRange(missingDays);

            return salesData.OrderBy(s => s.Date).ToList();
        }


        public async Task<List<TopProduct>> GetTopProductsForYearAsync(int year)
        {
            return await _context.OrderItems
                .Where(item => item.CreationDate.Year == year)
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

        public async Task<decimal> GetTotalRevenueForYearAsync(int year)
        {
            var totalRevenueForYear = await _context.OrderItems
                .Where(oi => oi.CreationDate.Year == year)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice);

            return totalRevenueForYear;
        }

        public async Task<List<CategorySalesData>> GetCategoryRankingForYearAsync(int year)
        {
            var ranking = await _context.OrderItems
                .Where(o => o.CreationDate.Year == year)
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

        

        public async Task<List<OrderItem>> GetOrderItemsSoldForYearAsync(int year)
        {
            return await _context.OrderItems
                .Where(oi => oi.CreationDate.Year == year)
                .ToListAsync();
        }

        public async Task<List<TopProduct>> GetLeastSoldProductsForYearAsync(int year)
        {
            var leastSoldProducts = await _context.OrderItems
                .Where(o => o.CreationDate.Year == year)
                .GroupBy(o => new { o.ProductId, o.Product.Name })
                .Select(g => new TopProduct
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalQuantity = g.Sum(o => o.Quantity)
                })
                .OrderBy(p => p.TotalQuantity)
                .Take(5)
                .ToListAsync();

            return leastSoldProducts;
        }

        public async Task<List<OrderItem>> GetOrderItemsForYearAsync(int year)
        {
            return await _context.OrderItems
                .Where(o => o.CreationDate.Year == year)
                .ToListAsync();
        }

        public async Task<List<TopBuyer>> GetTopBuyersForYearAsync(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);

            var topBuyersYear = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => o.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalSpent = g.Sum(o => o.TotalAmount),
                    OrdersCount = g.Count()
                })
                .OrderByDescending(g => g.TotalSpent)
                .Take(5)
                .ToListAsync();

            // Pobierz dane użytkowników
            var userIds = topBuyersYear.Select(b => b.UserId).ToList();
            var users = await _context.AspNetUsers
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName, u.Email })
                .ToDictionaryAsync(u => u.Id);

            // Połącz dane
            return topBuyersYear.Select(b => new TopBuyer
            {
                UserName = users[b.UserId]?.UserName ?? "Nieznany użytkownik",
                Email = users[b.UserId]?.Email ?? "Brak e-maila",
                TotalSpent = b.TotalSpent,
                OrdersCount = b.OrdersCount
            }).ToList();
        }

        public async Task<Dictionary<string, List<TopBuyer>>> GetTopBuyersWithQuartersAsync(int year)
        {
            // Pobieramy zamówienia z bazy danych, filtrowane po roku
            var orders = await _context.Orders
                .Where(o => o.OrderDate.Year == year)
                .Include(o => o.UserId)
                .ToListAsync();

            // Grupowanie po kwartale i użytkowniku
            var groupedByQuarter = orders
                .GroupBy(o => new { o.UserId, Quarter = (o.OrderDate.Month - 1) / 3 + 1 }) // Kwartał obliczany na podstawie miesiąca
                .Select(g => new
                {
                    UserId = g.Key.UserId,
                    Quarter = $"Q{g.Key.Quarter}",
                    TotalSpent = g.Sum(o => o.TotalAmount),
                    OrdersCount = g.Count()
                })
                .ToList();

            // Pobieramy dane użytkowników (e-mail, nazwa użytkownika)
            var userIds = groupedByQuarter.Select(g => g.UserId).Distinct().ToList();
            var users = await _context.AspNetUsers
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName, u.Email })
                .ToDictionaryAsync(u => u.Id);

            // Grupowanie danych według kwartalu
            var topBuyersByQuarter = groupedByQuarter
                .GroupBy(g => g.Quarter)
                .ToDictionary(g => g.Key, g =>
                    g.GroupBy(x => x.UserId)
                    .Select(x => new TopBuyer
                    {
                        UserName = users[x.Key]?.UserName ?? "Nieznany użytkownik",
                        Email = users[x.Key]?.Email ?? "Brak e-maila",
                        TotalSpent = x.Sum(s => s.TotalSpent),
                        OrdersCount = x.Sum(s => s.OrdersCount)
                    })
                    .OrderByDescending(b => b.TotalSpent)
                    .Take(5)
                    .ToList());

            // Top 5 za cały rok
            var topBuyersYearly = groupedByQuarter
                .GroupBy(g => g.UserId)
                .Select(g => new TopBuyer
                {
                    UserName = users[g.Key]?.UserName ?? "Nieznany użytkownik",
                    Email = users[g.Key]?.Email ?? "Brak e-maila",
                    TotalSpent = g.Sum(o => o.TotalSpent),
                    OrdersCount = g.Sum(o => o.OrdersCount)
                })
                .OrderByDescending(b => b.TotalSpent)
                .Take(5)
                .ToList();

            // Dodajemy do wyników kwartalnych top 5 za cały rok
            topBuyersByQuarter.Add("Roczny", topBuyersYearly);

            return topBuyersByQuarter;
        }

        public async Task<List<OrderItemWithProduct>> GetOrderItemsWithProductsForYearAsync(int year)
        {
            return await _context.OrderItems
                .Where(o => o.CreationDate.Year == year)
                .Join(_context.Products,
                    orderItem => orderItem.ProductId,
                    product => product.Id,
                    (orderItem, product) => new OrderItemWithProduct
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = orderItem.Quantity,
                        UnitPrice = orderItem.UnitPrice,
                        CreationDate = orderItem.CreationDate
                    })
                .ToListAsync();
        }

        public async Task<List<OrderItemWithCategory>> GetOrderItemsWithCategoriesForYearAsync(int year)
        {
            // Przykład implementacji - dostosuj do Twojej logiki dostępu do bazy danych
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .ThenInclude(p => p.Category)
                .Where(oi => oi.CreationDate.Year == year)
                .Select(oi => new OrderItemWithCategory
                {
                    CategoryName = oi.Product.Category,
                    Quantity = oi.Quantity,
                    CreationDate = oi.CreationDate
                })
                .ToListAsync();
        }

        //public async Task<Dictionary<string, List<CategorySalesData>>> GetCategorySalesForYearAsync(int year)
        //{
        //    // Pobieramy wszystkie dane o sprzedaży w danym roku
        //    var salesData = await _context.OrderItems
        //        .Where(o => o.CreationDate.Year == year)
        //        .Join(_context.Products, oi => oi.ProductId, p => p.Id, (oi, p) => new { oi.Quantity, p.Category, oi.CreationDate })
        //        .ToListAsync();

        //    // Grupowanie danych po kwartale (Q1, Q2, Q3, Q4)
        //    var categorySalesByQuarter = salesData
        //        .GroupBy(x => new { Quarter = $"Q{(x.CreationDate.Month - 1) / 3 + 1}", x.Category })
        //        .Select(g => new CategorySalesData
        //        {
        //            CategoryName = g.Key.Category,
        //            TotalQuantity = g.Sum(x => x.Quantity),
        //            Quarter = g.Key.Quarter
        //        })
        //        .ToList();

        //    // Grupowanie danych rocznych po kategorii
        //    var categorySalesYear = salesData
        //        .GroupBy(x => x.Category)
        //        .Select(g => new CategorySalesData
        //        {
        //            CategoryName = g.Key,
        //            TotalQuantity = g.Sum(x => x.Quantity),
        //            Quarter = "Roczny" // "Roczny" dla całego roku
        //        })
        //        .ToList();

        //    // Łączenie danych kwartalnych i rocznych
        //    var categorySales = categorySalesByQuarter
        //        .GroupBy(c => c.Quarter)
        //        .ToDictionary(g => g.Key, g => g.OrderByDescending(c => c.TotalQuantity).Take(5).ToList());

        //    // Dodanie danych rocznych do słownika
        //    categorySales.Add("Roczny", categorySalesYear.OrderByDescending(c => c.TotalQuantity).Take(5).ToList());

        //    return categorySales;
        //}

        public async Task<Dictionary<string, List<CategorySalesData>>> GetCategorySalesForYearAsync(int year)
        {
            // Pobieramy wszystkie dane o sprzedaży w danym roku
            var salesData = await _context.OrderItems
                .Where(o => o.CreationDate.Year == year)
                .Join(_context.Products, oi => oi.ProductId, p => p.Id, (oi, p) => new { oi.Quantity, p.Category, oi.CreationDate })
                .ToListAsync();

            // Grupowanie danych po kwartale (Q1, Q2, Q3, Q4)
            var categorySalesByQuarter = salesData
                .GroupBy(x => new { Quarter = $"Q{(x.CreationDate.Month - 1) / 3 + 1}", x.Category })
                .Select(g => new CategorySalesData
                {
                    CategoryName = g.Key.Category,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    Quarter = g.Key.Quarter
                })
                .ToList();

            // Grupowanie danych rocznych po kategorii
            var categorySalesYear = salesData
                .GroupBy(x => x.Category)
                .Select(g => new CategorySalesData
                {
                    CategoryName = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    Quarter = "Roczny" // "Roczny" dla całego roku
                })
                .ToList();

            // Grupowanie danych według kwartałów
            var categorySales = categorySalesByQuarter
                .GroupBy(c => c.Quarter)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(c => c.TotalQuantity).Take(1).ToList());

            // Dodanie danych rocznych do słownika (tylko 1 najlepsza kategoria)
            categorySales.Add("Roczny", categorySalesYear.OrderByDescending(c => c.TotalQuantity).Take(1).ToList());

            return categorySales;
        }

    }
}
