using MoistureMeterAPI.Core.Models;

namespace MoistureMeterAPI.Core.Repository.Interfaces
{
    public interface IMoistureMeterRepository
    {
        public Task<bool> Insert(MoistureMeterReading reading);
        public Task<PaginationResult<MoistureMeterReading>> GetPaginationResult(int pageSize = 100, MoistureMeterReading? lastRecord = null);
    }
}
