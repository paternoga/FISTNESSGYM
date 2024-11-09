using FISTNESSGYM.Models.database;

namespace FISTNESSGYM.Services
{
    public interface IMeasurementService
    {
        Task<Measurement> GetMeasurementsByUserIdAsync(string userId);

    }
}
