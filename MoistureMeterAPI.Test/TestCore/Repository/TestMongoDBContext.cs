using MoistureMeterAPI.Core.Repository.Interfaces;
using Mongo2Go;
using MongoDB.Driver;

namespace MoistureMeterAPI.Test.TestCore.Repository
{
    /// <summary>
    /// Provides a test implementation of a MongoDB database context for use in unit tests and development scenarios.
    /// </summary>
    /// <remarks>This context initializes an in-memory MongoDB instance using Mongo2Go, allowing for isolated
    /// and repeatable database operations during testing. The database is created with a fixed name and is intended for
    /// non-production use. The context exposes the MongoDB database instance via the <see cref="MongoDatabase"/>
    /// property.</remarks>
    public class TestMongoDBContext : IDBContext
    {
        internal MongoDbRunner _runner;
        internal string _databaseName = "moisturemeter-test";

        public TestMongoDBContext()
        {
            _runner = MongoDbRunner.StartForDebugging(singleNodeReplSet: false);

            MongoClient mongoClient = new MongoClient(_runner.ConnectionString);
            MongoDatabase = mongoClient.GetDatabase(_databaseName);
        }

        public IMongoDatabase MongoDatabase { get; set; }
    }
}
