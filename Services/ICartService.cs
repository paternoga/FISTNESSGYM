using FISTNESSGYM.Models.database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FISTNESSGYM.Services
{
    public interface ICartService
    {
        // Dodaje przedmiot do koszyka dla danego użytkownika
        Task AddToCart(string userId, Product product, int quantity);

        // Usuwa przedmiot z koszyka dla danego użytkownika
        void RemoveFromCart(string userId, int productId);

        // Zwraca wszystkie przedmioty w koszyku dla danego użytkownika
        List<CartItem> GetCartItems(string userId);

        // Zwraca całkowitą wartość koszyka dla danego użytkownika
        decimal GetTotal(string userId);

        // Czyści koszyk dla danego użytkownika
        void ClearCart(string userId);

        // Pobiera konkretny przedmiot z koszyka na podstawie userId i productId
        Task<CartItem> GetCartItemAsync(string userId, int productId);

        // Dodaje nowy przedmiot do koszyka
        Task AddCartItemAsync(CartItem cartItem);

        // Aktualizuje istniejący przedmiot w koszyku
        Task UpdateCartItemAsync(CartItem cartItem);
        // Metoda do pobierania przedmiotów z koszyka
        Task<List<CartItem>> GetCartItemsAsync(string userId);

        // Metoda do usuwania przedmiotu z koszyka
        Task RemoveFromCartAsync(string userId, int productId);

        // Metoda do czyszczenia koszyka
        Task ClearCartAsync(string userId);

        // Dodaj inne metody, które mogą być potrzebne, np. dodawanie do koszyka
        Task AddToCartAsync(string userId, CartItem item);
    }
}
