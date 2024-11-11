using FISTNESSGYM.Data;
using FISTNESSGYM.Models.database;
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
            // Sprawdź, czy produkt już istnieje w koszyku dla danego użytkownika
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == product.Id);

            if (existingCartItem != null)
            {
                // Jeśli element już istnieje, zaktualizuj jego ilość
                existingCartItem.Quantity += quantity;
                _context.CartItems.Update(existingCartItem); // Oznacz element do aktualizacji
            }
            else
            {
                // Jeśli element nie istnieje, dodaj nowy
                var newCartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = product.Id,
                    Quantity = quantity
                };
                await _context.CartItems.AddAsync(newCartItem); // Dodaj nowy element do kontekstu
            }

            // Zapisz zmiany w bazie danych
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
                .Include(ci => ci.Product) // Załaduj powiązane produkty
                .ToList();
        }

        public decimal GetTotal(string userId)
        {
            return _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Sum(ci => ci.Quantity * ci.Product.Price); // Zakłada, że masz załadowane produkty
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
                                 .Include(ci => ci.Product) // dołącz szczegóły produktu
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
            // Można dodać logikę do dodawania przedmiotu do koszyka, z aktualizacją ilości
            var existingItem = await _context.CartItems
                                              .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity; // aktualizacja ilości
            }
            else
            {
                item.UserId = userId; // ustawienie UserId przed dodaniem
                await _context.CartItems.AddAsync(item);
            }

            await _context.SaveChangesAsync();
        }

        public async Task PlaceOrderAsync(Order order, List<CartItem> cartItems)
        {
            // Dodaj zamówienie do bazy danych
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            // Dodaj pozycje zamówienia
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price,
                    CreationDate = DateTime.Now// zakładam, że price jest pobierane z produktu
                };

                await _context.OrderItems.AddAsync(orderItem);
                
                // Aktualizacja stanu magazynowego
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product != null)
                {
                    // Kontrola stanu magazynowego
                    if (product.StockQuantity >= cartItem.Quantity)
                    {
                        product.StockQuantity -= cartItem.Quantity; // Odejmij ilość zamówioną od stanu magazynowego
                    }
                    else
                    {
                        //throw new Exception("Niewystarczająca ilość produktu w magazynie."); // Obsłuż sytuację, gdy brak wystarczającej ilości
                        throw new InvalidOperationException("Niewystarczająca ilość produktu w magazynie.");
                    }
                }
            }

            // Zapisz zmiany w bazie danych
            await _context.SaveChangesAsync();
        }

        //Analityka sklepu///////
        public async Task<int> GetTotalSoldTodayAsync()
        {
            // Pobieramy dzisiejszą datę bez czasu
            var todayStart = DateTime.Today;
            var todayEnd = todayStart.AddDays(1).AddSeconds(-1); // koniec dnia (23:59:59)

            // Pobieramy wszystkie pozycje OrderItem, które zostały utworzone dzisiaj
            var totalSoldToday = await _context.OrderItems
                .Where(oi => oi.CreationDate >= todayStart && oi.CreationDate <= todayEnd) // Filtrowanie po dacie
                .SumAsync(oi => oi.Quantity); // Sumowanie ilości sprzedanych produktów

            return totalSoldToday;
        }

        public async Task<List<SalesData>> GetSalesDataForLastDaysAsync(int days = 7)
        {
            var todayStart = DateTime.Today;
            var startDate = todayStart.AddDays(-days); // Pobieramy dane z ostatnich 'days' dni

            var salesData = await _context.OrderItems
                .Where(oi => oi.CreationDate >= startDate && oi.CreationDate < todayStart)
                .GroupBy(oi => oi.CreationDate.Date) // Grupowanie według daty
                .Select(g => new SalesData
                {
                    Date = g.Key.ToString("yyyy-MM-dd"), // Formatowanie daty
                    Quantity = g.Sum(oi => oi.Quantity) // Suma ilości sprzedanych przedmiotów w danym dniu
                })
                .OrderBy(s => s.Date) // Posortowanie po dacie
                .ToListAsync();

            return salesData;
        }

        public async Task<List<OrderItem>> GetOrderItemsSoldTodayAsync()
        {
            // Pobierz dane z bazy (np. wszystkie OrderItemy) - używamy LINQ, więc nie potrzebujemy zapytań SQL
            var allOrderItems = await _context.OrderItems.ToListAsync();

            // Filtrujemy dane po dacie (musi to być dzisiaj)
            return allOrderItems.Where(item => item.CreationDate.Date == DateTime.Today).ToList();
        }







    }
}
