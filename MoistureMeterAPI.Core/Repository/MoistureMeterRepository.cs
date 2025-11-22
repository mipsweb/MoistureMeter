using Microsoft.Extensions.Logging;
using MoistureMeterAPI.Core.Models;
using MoistureMeterAPI.Core.Repository.Interfaces;
using MongoDB.Driver;

namespace MoistureMeterAPI.Core.Repository
{
    /// <summary>
    /// Provides methods for storing and managing moisture meter readings in a MongoDB database.
    /// </summary>
    /// <remarks>This repository encapsulates data access logic for moisture meter readings, allowing for
    /// insertion and retrieval operations. It is intended to be used as a dependency in services that require
    /// persistent storage of moisture data. Thread safety and connection management are handled internally.</remarks>
    public class MoistureMeterRepository : IMoistureMeterRepository
    {
        ILogger<MoistureMeterRepository> _logger;
        IMongoCollection<MoistureMeterReading> _moistureMeterCollection;

        public MoistureMeterRepository(ILogger<MoistureMeterRepository> logger, IDBContext dbContext)
        {
            _logger = logger;

            var collectionExists = dbContext.MongoDatabase.ListCollectionNames().ToList().Contains("reading");
            if (!collectionExists)
            {
                dbContext.MongoDatabase.CreateCollection("reading");
            }

            _moistureMeterCollection = dbContext.MongoDatabase.GetCollection<MoistureMeterReading>("reading", new MongoCollectionSettings
            {
                AssignIdOnInsert = true
            });

            _moistureMeterCollection.Indexes.CreateOne(new CreateIndexModel<MoistureMeterReading>(Builders<MoistureMeterReading>.IndexKeys.Descending(m => m.Timestamp)));
        }

        public async Task<PaginationResult<MoistureMeterReading>> GetPaginationResult(int pageSize = 100, MoistureMeterReading? lastRecord = null)
        {
            _logger.LogInformation("GetPaginationResult");

            try
            {
                var compareDateTime = DateTimeOffset.Now;
                if (lastRecord != null)
                {
                    compareDateTime = lastRecord.Timestamp;
                }

                var filter = Builders<MoistureMeterReading>.Filter.Lt(x => x.Timestamp, compareDateTime);

                var aggregate = _moistureMeterCollection.Aggregate()
                    .Match(filter)
                    .SortByDescending(u => u.Timestamp)
                    .Limit(pageSize);

                var rowsTotal = await _moistureMeterCollection.EstimatedDocumentCountAsync();

                var result = new PaginationResult<MoistureMeterReading>
                {
                    Result = aggregate.ToList(),
                    Rows = rowsTotal
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPaginationResult failed");
                throw;
            }
        }

        /// <summary>
        /// Asynchronously inserts a new moisture meter reading into the data store.
        /// </summary>
        /// <param name="reading">The moisture meter reading to insert. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous insert operation.</returns>
        public async Task<bool> Insert(MoistureMeterReading reading)
        {
            _logger.LogInformation("Inserting new moisture meter reading");

            try
            {
                await _moistureMeterCollection.InsertOneAsync(reading);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting moisture meter reading");
                throw;
            }

        }

        /**/
    }
}
