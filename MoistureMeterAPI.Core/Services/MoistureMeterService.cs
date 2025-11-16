using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MoistureMeterAPI.Core.Models;
using MoistureMeterAPI.Core.Repository.Interfaces;
using MoistureMeterAPI.Core.Services.Interfaces;

namespace MoistureMeterAPI.Core.Services
{
    public class MoistureMeterService : IMoistureMeterService
    {
        ILogger<MoistureMeterService> _logger;
        IMoistureMeterRepository _moistureMeterRepository;

        public MoistureMeterService(ILogger<MoistureMeterService> logger, IMoistureMeterRepository moistureMeterRepository)
        {
            _logger = logger;
            _moistureMeterRepository = moistureMeterRepository;
        }

        public async Task Insert(MoistureMeterReading reading)
        {
            _logger.LogInformation("Inserting moisture meter reading");

            try
            {
                await _moistureMeterRepository.Insert(reading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting moisture meter reading");
                throw;
            }
        }
    }
}
