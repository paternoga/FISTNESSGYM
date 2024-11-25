using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FISTNESSGYM.Models.database;
using FISTNESSGYM.Data;

namespace FISTNESSGYM.Services
{
    public class ProductService : IProductService
    {
        private readonly databaseContext _context;

        // Przyjmujemy tylko databaseContext w konstruktorze
        public ProductService(databaseContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            // Using AsNoTracking for read-only operations to improve performance
            return await _context.Products.AsNoTracking().ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            // Handle potential null case if product not found
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            return await _context.ProductCategories
                .Select(c => c.Name) // Assuming "Name" is the property storing category names
                .Distinct() // Ensure unique category names
                .ToListAsync();
        }

        public async Task<List<Product>> GetFilteredProductsAsync(string searchTerm, string category, bool sortAscending)
        {
            var query = _context.Products.AsQueryable();

            // Filter by search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            // Sort products by price
            query = sortAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);

            return await query.AsNoTracking().ToListAsync(); // Use AsNoTracking for read-only scenarios
        }

        public async Task<int> GetProductStockQuantityAsync(int productId)
        {
            // Używamy Entity Framework do uzyskania ilości zapasów
            var product = await _context.Products.FindAsync(productId);
            return product?.StockQuantity ?? 0; // Zwróć ilość zapasów lub 0, jeśli produkt nie został znaleziony
        }

        public async Task<int> GetOutOfStockCountAsync()
        {
            return await _context.Products
                .Where(p => p.StockQuantity == 0)
                .CountAsync();
        }

    }
}
