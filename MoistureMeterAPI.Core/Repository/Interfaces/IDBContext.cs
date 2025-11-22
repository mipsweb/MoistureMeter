using MongoDB.Driver;

namespace MoistureMeterAPI.Core.Repository.Interfaces
{
    /// <summary>
    /// Gets or sets the MongoDB database instance associated with the context.
    /// </summary>
    /// <remarks>Use this property to access or assign the underlying <see cref="IMongoDatabase"/> for
    /// database operations. Changing the value may affect subsequent queries and commands executed through this
    /// context.</remarks>
    public interface IDBContext
    {
        public IMongoDatabase MongoDatabase { get; set; }
    }
}
