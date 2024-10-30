using FISTNESSGYM.Models.database;
using FISTNESSGYM.Models.Database;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace FISTNESSGYM.Services
{
    public class CartService : ICartService
    {
        private readonly string _connectionString;

        public CartService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddToCart(string userId, Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var existingItem = GetCartItems(userId).FirstOrDefault(ci => ci.Product.Id == product.Id);

                if (existingItem != null)
                {
                    existingItem.Quantity++;
                    var sql = "UPDATE CartItems SET Quantity = @Quantity WHERE UserId = @UserId AND ProductId = @ProductId";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Quantity", existingItem.Quantity);
                        command.Parameters.AddWithValue("@UserId", userId); // string zamiast Guid lub int
                        command.Parameters.AddWithValue("@ProductId", product.Id);
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    var sql = "INSERT INTO CartItems (UserId, ProductId, Quantity) VALUES (@UserId, @ProductId, @Quantity)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId); // string zamiast Guid lub int
                        command.Parameters.AddWithValue("@ProductId", product.Id);
                        command.Parameters.AddWithValue("@Quantity", 1);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void RemoveFromCart(string userId, int productId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "DELETE FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId); // string zamiast Guid lub int
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<CartItem> GetCartItems(string userId)
        {
            var cartItems = new List<CartItem>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
            SELECT ci.Id, ci.Quantity, p.Id AS ProductId, p.Name, p.Description, p.Price 
            FROM CartItems ci
            JOIN Product p ON ci.ProductId = p.Id
            WHERE ci.UserId = @UserId";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId); // string zamiast Guid lub int
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cartItem = new CartItem
                            {
                                Id = reader.GetInt32(0),
                                Quantity = reader.GetInt32(1),
                                Product = new Product
                                {
                                    Id = reader.GetInt32(2),
                                    Name = reader.GetString(3),
                                    Description = reader.GetString(4),
                                    Price = reader.GetDecimal(5)
                                }
                            };
                            cartItems.Add(cartItem);
                        }
                    }
                }
            }
            return cartItems;
        }

        public decimal GetTotal(string userId)
        {
            decimal totalAmount = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
            SELECT SUM(p.Price * ci.Quantity) AS TotalAmount
            FROM CartItems ci
            JOIN Product p ON ci.ProductId = p.Id
            WHERE ci.UserId = @UserId";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId); // string zamiast Guid lub int
                    var result = command.ExecuteScalar();
                    totalAmount = result != DBNull.Value ? (decimal)result : 0;
                }
            }
            return totalAmount;
        }

        public void ClearCart(string userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "DELETE FROM CartItems WHERE UserId = @UserId";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId); // string zamiast Guid lub int
                    command.ExecuteNonQuery();
                }
            }
        }
    }


}
