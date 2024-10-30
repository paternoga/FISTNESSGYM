using FISTNESSGYM.Models.database;

namespace FISTNESSGYM.Models.Database
{
    public class CartItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }         
        public Product Product { get; set; }     
        public int Quantity { get; set; } 
    }

}
