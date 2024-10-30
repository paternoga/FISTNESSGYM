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

        public ProductService(databaseContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            return await _context.ProductCategories
                .Select(c => c.Name)  // Assuming "Name" is the property storing category names
                .ToListAsync();
        }
        public async Task<List<Product>> GetFilteredProductsAsync(string searchTerm, string category, bool sortAscending)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            query = sortAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);

            return await query.ToListAsync();
        }
    }
}
