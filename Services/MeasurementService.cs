using DocumentFormat.OpenXml.InkML;
using FISTNESSGYM.Components.Pages.Settings.Subscription;
using FISTNESSGYM.Data;
using FISTNESSGYM.Models.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FISTNESSGYM.Services
{
    public class MeasurementService : IMeasurementService
    {
        private readonly databaseContext _context;
        private readonly ILogger<SubscriptionService> _logger;

        public MeasurementService(databaseContext context, ILogger<SubscriptionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Measurement>> GetMeasurementsAsync()
        {
            return await _context.Measurement
                .AsNoTracking()  // Poprawia wydajność przy tylko odczycie
                .ToListAsync();
        }

        public async Task<Measurement> GetMeasurementsByUserIdAsync(string userId)
        {
            try
            {
                var measurement = await _context.Measurement
                   .Where(m => m.UserId == userId)
                   .FirstOrDefaultAsync();

                if (measurement == null)
                {
                    _logger.LogWarning("Brak subskrypcji dla użytkownika: {UserId}", userId);
                }

                return measurement;

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas pobierania subskrypcji dla użytkownika: {UserId}", userId);
                throw;
            }       
        }
        public async Task<(Measurement latest, Measurement? secondLatest)> GetLatestAndSecondLatestMeasurementsByUserIdAsync(string userId)
        {
            try
            {
                var measurements = await _context.Measurement
                    .Where(m => m.UserId == userId)
                    .OrderByDescending(m => m.MeasurementDate)
                    .Take(2)
                    .ToListAsync();

                if (measurements.Count == 0)
                {
                    _logger.LogWarning("Nie znaleziono pomiarów dla: {UserId}", userId);
                    return (null, null);
                }

                Measurement latest = measurements[0];
                Measurement? secondLatest = measurements.Count > 1 ? measurements[1] : null;

                return (latest, secondLatest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas pobierania pomiarów dla użytkownika: {UserId}", userId);
                throw;
            }
        }
    }
}
