namespace FISTNESSGYM.Models.Database
{
    public class LastPurchasedProduct
    {
        public string ProductName { get; set; } 
        public int Quantity { get; set; }      
        public decimal UnitPrice { get; set; } 
        public DateTime CreationDate { get; set; } 
    }

}
