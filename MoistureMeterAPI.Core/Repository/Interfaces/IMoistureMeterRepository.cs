using MoistureMeterAPI.Core.Models;

namespace MoistureMeterAPI.Core.Repository.Interfaces
{
    public interface IMoistureMeterRepository
    {
        public Task<bool> Insert(MoistureMeterReading reading);
    }
}
