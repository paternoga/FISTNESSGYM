using FISTNESSGYM.Data;
using FISTNESSGYM.Models.database;
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
    }
}
