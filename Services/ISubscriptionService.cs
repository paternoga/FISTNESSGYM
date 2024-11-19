using FISTNESSGYM.Models.database;

namespace FISTNESSGYM.Services
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionType>> GetSubscriptionTypesAsync();
        Task<decimal> GetPriceForSubscriptionType(int subscriptionTypeId);
        Task<Subscription> GetCurrentSubscriptionAsync(string userId);
        Task PurchaseSubscriptionAsync(string userId, int subscriptionTypeId);
        Task CancelSubscriptionAsync(int subscriptionId);
        Task<List<Subscription>> GetSubscriptionsByUserIdAsync(string userId);
    }

}
