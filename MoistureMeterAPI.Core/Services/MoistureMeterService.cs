using Microsoft.Extensions.Logging;
using MoistureMeterAPI.Core.Models;
using MoistureMeterAPI.Core.Repository.Interfaces;
using MoistureMeterAPI.Core.Services.Interfaces;

namespace MoistureMeterAPI.Core.Services
{
    /// <inheritdoc/>
    public class MoistureMeterService : IMoistureMeterService
    {
        ILogger<MoistureMeterService> _logger;
        IMoistureMeterRepository _moistureMeterRepository;

        public MoistureMeterService(ILogger<MoistureMeterService> logger, IMoistureMeterRepository moistureMeterRepository)
        {
            _logger = logger;
            _moistureMeterRepository = moistureMeterRepository;
        }

        /// <inheritdoc/>
        public async Task<PaginationResult<MoistureMeterReading>> GetPaginationResult(int pageSize = 100, MoistureMeterReading? lastResult = null)
        {
            _logger.LogInformation("GetPaginationResult");

            try
            {
                return await _moistureMeterRepository.GetPaginationResult(pageSize, lastResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPaginationResult failed");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> Insert(MoistureMeterReading reading)
        {
            _logger.LogInformation("Inserting moisture meter reading");

            try
            {
                return await _moistureMeterRepository.Insert(reading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting moisture meter reading");
                throw;
            }
        }
    }
}
