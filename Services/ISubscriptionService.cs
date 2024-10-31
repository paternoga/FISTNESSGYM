using FISTNESSGYM.Models.database;

namespace FISTNESSGYM.Services
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionType>> GetSubscriptionTypesAsync();
        Task<decimal> GetPriceForSubscriptionType(int subscriptionTypeId);
        Task PurchaseSubscriptionAsync(string userId, int subscriptionTypeId);
    }

}
