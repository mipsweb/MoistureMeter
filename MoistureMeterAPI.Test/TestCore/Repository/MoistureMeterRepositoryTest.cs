using Microsoft.Extensions.Logging;
using MoistureMeterAPI.Core.Repository;
using MoistureMeterAPI.Core.Repository.Interfaces;
using MoistureMeterAPI.Test.TestCore.Repository;

namespace MoistureMeterAPI.Test;

public class MoistureMeterRepositoryTest
{
    IMoistureMeterRepository _repository;

    [SetUp]
    public async Task Setup()
    {
        var accountRepositoryLogger = Moq.Mock.Of<ILogger<MoistureMeterRepository>>();

        var mongoDatabaseContext = new TestMongoDBContext();

        _repository = new MoistureMeterRepository(accountRepositoryLogger, mongoDatabaseContext);

        await mongoDatabaseContext.MongoDatabase.DropCollectionAsync("reading");

        var readingCollection = mongoDatabaseContext.MongoDatabase.GetCollection<Core.Models.MoistureMeterReading>("reading");

        var seedDataList = new List<Core.Models.MoistureMeterReading>();

        var nextDate = DateTime.Now.AddHours(-12);
        var randomReading = new Random();

        do
        {
            var reading = randomReading.Next(0, 100);

            seedDataList.Add(new Core.Models.MoistureMeterReading
            {
                Timestamp = nextDate,
                Measure = reading
            });

            nextDate = nextDate.AddMinutes(2);

        } while (nextDate < DateTime.Now);


        await readingCollection.InsertManyAsync(seedDataList);
    }

    /// <summary>
    /// Verifies that a moisture meter reading can be successfully inserted into the repository.
    /// </summary>
    /// <remarks>This test ensures that the repository's insert operation returns <see langword="true"/> when
    /// a valid moisture meter reading is provided. The test is asynchronous and uses the NUnit framework.</remarks>
    /// <returns></returns>
    [Test]
    public async Task MoistureMeterRepositoryTest_Verify_MoistureMeter_Can_Be_Inserted()
    {
        var result = await _repository.Insert(new Core.Models.MoistureMeterReading
        {
            Measure = 1,
            Timestamp = DateTime.Now,
        });

        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Verifies that the moisture meter repository can load a list of moisture meter records with pagination and that
    /// subsequent pages are correctly retrieved.
    /// </summary>
    /// <remarks>This test ensures that the repository returns non-null results with more than 100 rows and
    /// that pagination yields the expected ordering of records. It validates both initial and subsequent page
    /// retrievals using the repository's pagination mechanism.</remarks>
    /// <returns></returns>
    [Test]
    public async Task MoistureMeterRepositoryTest_Verify_MoistureMeter_List_Can_Be_Loaded()
    {
        var result = await _repository.GetPaginationResult(100);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Rows, Is.GreaterThan(100));

        var lastResult = result.Result.Last();

        var nextResult = await _repository.GetPaginationResult(100, lastResult);

        Assert.That(nextResult, Is.Not.Null);
        Assert.That(nextResult.Result, Is.Not.Null);
        Assert.That(nextResult.Rows, Is.GreaterThan(100));

        Assert.That(nextResult.Result.First().Timestamp, Is.LessThan(lastResult.Timestamp));
    }
}
