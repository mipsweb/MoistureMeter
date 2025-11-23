using MoistureMeterAPI.Core.Models;
using MongoDB.Bson;

namespace MoistureMeterAPI.Core.Repository.Interfaces
{
    /// <summary>
    /// Defines methods for storing and retrieving moisture meter readings with support for paginated queries.
    /// </summary>
    /// <remarks>Implementations of this interface should provide asynchronous operations for inserting new
    /// readings and retrieving paginated results. Thread safety and data consistency are dependent on the specific
    /// implementation.</remarks>
    public interface IMoistureMeterRepository
    {
        /// <summary>
        /// Asynchronously inserts a new moisture meter reading into the data store.
        /// </summary>
        /// <param name="reading">The moisture meter reading to insert. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the reading
        /// was successfully inserted; otherwise, <see langword="false"/>.</returns>
        public Task<bool> Insert(MoistureMeterReading reading);
        
        /// <summary>
        /// Retrieves a paginated set of moisture meter readings starting after the specified record.
        /// </summary>
        /// <param name="pageSize">The maximum number of readings to include in the result. Must be greater than zero. The default is 100.</param>
        /// <param name="lastRecord">The last moisture meter reading from the previous page, or <see langword="null"/> to start from the
        /// beginning.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see
        /// cref="PaginationResult{MoistureMeterReading}"/> with the requested page of readings.</returns>
        public Task<PaginationResult<MoistureMeterReading>> GetPaginationResult(int pageSize = 100, MoistureMeterReading? lastRecord = null);
        
        /// <summary>
        /// Retrieves a moisture meter reading by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the moisture meter reading to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the moisture meter reading
        /// associated with the specified identifier, or null if no matching reading is found.</returns>
        public Task<MoistureMeterReading> GetById(ObjectId id);
    }
}
