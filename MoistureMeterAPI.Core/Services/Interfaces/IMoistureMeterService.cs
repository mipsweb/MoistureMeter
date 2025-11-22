using MoistureMeterAPI.Core.Models;

namespace MoistureMeterAPI.Core.Services.Interfaces
{
    /// <summary>
    /// Defines methods for retrieving and storing moisture meter readings with support for pagination.
    /// </summary>
    /// <remarks>Implementations of this interface should provide asynchronous access to moisture meter data,
    /// enabling efficient retrieval of large datasets and insertion of new readings. Methods are designed to support
    /// scalable data access patterns, such as paginated queries for processing historical or real-time sensor
    /// data.</remarks>
    public interface IMoistureMeterService
    {
        /// <summary>
        /// Retrieves a paginated list of moisture meter readings, starting after the specified reading.
        /// </summary>
        /// <param name="pageSize">The maximum number of readings to include in the result. Must be greater than zero. The default value is
        /// 100.</param>
        /// <param name="lastResult">The last reading from the previous page, or <see langword="null"/> to start from the beginning.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see
        /// cref="PaginationResult{MoistureMeterReading}"/> with the requested page of readings.</returns>
        public Task<PaginationResult<MoistureMeterReading>> GetPaginationResult(int pageSize = 100, MoistureMeterReading? lastResult = null);
        
        /// <summary>
        /// Asynchronously inserts a new moisture meter reading into the data store.
        /// </summary>
        /// <param name="reading">The moisture meter reading to insert. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the reading
        /// was successfully inserted; otherwise, <see langword="false"/>.</returns>
        public Task<bool> Insert(MoistureMeterReading reading);
    }
}
