using FISTNESSGYM.Models.database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FISTNESSGYM.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetFilteredProductsAsync(string searchTerm, string category, bool sortAscending);
        Task<List<string>> GetAllCategoriesAsync();
        Task<int> GetProductStockQuantityAsync(int productId);

    }
}
