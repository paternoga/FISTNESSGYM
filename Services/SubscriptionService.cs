using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FISTNESSGYM.Data;
using FISTNESSGYM.Models;
using FISTNESSGYM.Models.database;
using FISTNESSGYM.Models.Database.ModelsDTO;
using FISTNESSGYM.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FISTNESSGYM.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly databaseContext _context;
        private readonly ILogger<SubscriptionService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DbLogger _dbLogger;

        public SubscriptionService(databaseContext context, ILogger<SubscriptionService> logger, UserManager<ApplicationUser> userManager, DbLogger dbLogger)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _dbLogger = dbLogger;
        }

        public async Task<List<SubscriptionType>> GetSubscriptionTypesAsync()
        {
            try
            {
                return await _context.SubscriptionTypes.ToListAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("There was a problem retrieving subscription types from the database.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving subscription types.", ex);
            }
        }

        public async Task<decimal> GetPriceForSubscriptionType(int subscriptionTypeId)
        {
            try
            {
                return subscriptionTypeId switch
                {
                    1 => 99.00m, // miesieczna
                    2 => 950.00m, // roczna
                    3 => 1.00m, // probna
                    _ => throw new ArgumentException("Invalid subscription type.")
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the price for the subscription type.", ex);
            }
        }

        public async Task<Subscription> GetCurrentSubscriptionAsync(string userId)
        {
            try
            {
                var subscription = await _context.Subscriptions
                    .Include(s => s.SubscriptionType)
                    .FirstOrDefaultAsync(s => s.UserId == userId && s.SubscriptionStatusId == 1); 

                if (subscription == null)
                {
                    _logger.LogWarning("Brak subskrypcji dla użytkownika: {UserId}", userId);
                }

                return subscription;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas pobierania subskrypcji dla użytkownika: {UserId}", userId);
                throw;
            }
        }


        public async Task PurchaseSubscriptionAsync(string userId, int subscriptionTypeId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.");
            }

            var userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new Exception("The specified user does not exist.");
            }

            try
            {
                var subscriptionType = await _context.SubscriptionTypes.FindAsync(subscriptionTypeId);
                if (subscriptionType == null)
                {
                    throw new Exception("Invalid subscription type.");
                }

                decimal price = await GetPriceForSubscriptionType(subscriptionTypeId);

                var subscription = new Subscription
                {
                    UserId = userId,
                    SubscriptionTypeId = subscriptionTypeId,
                    StartDate = DateTime.UtcNow,
                    EndDate = subscriptionTypeId == 1
                        ? DateTime.UtcNow.AddMonths(1)
                        : DateTime.UtcNow.AddYears(1),
                    SubscriptionStatusId = 1,
                    Price = price
                };

                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Użytkownik"))
                    {
                        await _userManager.RemoveFromRoleAsync(user, "Użytkownik");
                    }

                    if (!await _userManager.IsInRoleAsync(user, "Klient"))
                    {
                        await _userManager.AddToRoleAsync(user, "Klient");
                    }
                }

                var logMessage = $"Użytkownik {subscription.UserId} zakupił subskrypcję {subscription.SubscriptionType.TypeName} w dniu {subscription.StartDate}.";
                _logger.LogInformation(logMessage);  

                await _dbLogger.LogToDatabaseAsync(LogLevel.Information, logMessage);

            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("There was a problem saving the subscription to the database.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while purchasing the subscription.", ex);
            }
        }

        public async Task CancelSubscriptionAsync(int subscriptionId)
        {
            var subscription = await _context.Subscriptions.FindAsync(subscriptionId);

            if (subscription == null)
            {
                throw new Exception("Subskrypcja nie została znaleziona.");
            }

            subscription.SubscriptionStatusId = 2;

            try
            {
                await _context.SaveChangesAsync();

                var user = await _userManager.FindByIdAsync(subscription.UserId);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Klient"))
                    {
                        await _userManager.RemoveFromRoleAsync(user, "Klient");
                        await _userManager.AddToRoleAsync(user, "Użytkownik");
                    }

                }
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Wystąpił problem podczas anulowania subskrypcji.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Wystąpił błąd podczas anulowania subskrypcji.", ex);
            }

            var logMessage = $"Użytkownik {subscription.UserId} anulował subskrypcję {subscription.SubscriptionType.TypeName} w dniu {subscription.StartDate}.";
            _logger.LogInformation(logMessage);

            await _dbLogger.LogToDatabaseAsync(LogLevel.Information, logMessage);
        }


        public async Task<List<Subscription>> GetSubscriptionsByUserIdAsync(string userId)
        {
            return await _context.Subscriptions
                .Include(s => s.SubscriptionType) 
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }


    }
}
