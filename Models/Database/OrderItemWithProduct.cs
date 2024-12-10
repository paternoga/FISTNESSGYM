namespace FISTNESSGYM.Models.Database
{
    public class OrderItemWithProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreationDate { get; set; }
    }

}
