using MongoDB.Driver;

namespace MoistureMeterAPI.Core.Repository.Interfaces
{
    public interface IDBContext
    {
        public IMongoDatabase MongoDatabase { get; set; }
    }
}
