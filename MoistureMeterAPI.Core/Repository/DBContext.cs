using Microsoft.Extensions.Options;
using MoistureMeterAPI.Core.Options;
using MoistureMeterAPI.Core.Repository.Interfaces;
using MongoDB.Driver;

namespace MoistureMeterAPI.Core.Repository
{
    public class DBContext : IDBContext
    {
        public IMongoDatabase MongoDatabase { get; set; }

        public DBContext(IOptions<MongoDBOptions> options)
        {
            MongoClient _mongoClient = new MongoClient(options.Value.GetConnectionString());

            MongoDatabase = _mongoClient.GetDatabase(options.Value.DatabaseName);
        }        
    }
}
