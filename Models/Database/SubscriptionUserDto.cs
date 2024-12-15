namespace FISTNESSGYM.Models.Database
{
    public class SubscriptionUserDto
    {
        public string Email { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}
