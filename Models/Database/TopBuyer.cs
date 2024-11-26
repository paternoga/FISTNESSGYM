namespace FISTNESSGYM.Models.Database
{
    public class TopBuyer
    {
        public string UserName { get; set; }  
        public string Email { get; set; }    
        public decimal TotalSpent { get; set; } 
        public int OrdersCount { get; set; }    
    }


}
