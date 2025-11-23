using Microsoft.Extensions.Logging;
using MoistureMeterAPI.Core.Models;
using MoistureMeterAPI.Core.Repository.Interfaces;
using MoistureMeterAPI.Core.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.IO;

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
        public async Task<PaginationResult<MoistureMeterReading>> GetPaginationResult(int pageSize = 100, string? lastId = null)
        {
            _logger.LogInformation("GetPaginationResult");

            try
            {
                MoistureMeterReading? lastRecord = null;

                if (lastId != null) {

                    lastRecord = await _moistureMeterRepository.GetById(new ObjectId(lastId));
                }                

                return await _moistureMeterRepository.GetPaginationResult(pageSize, lastRecord);
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
