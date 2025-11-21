using Microsoft.Extensions.Logging;
using MoistureMeterAPI.Core.Repository;
using MoistureMeterAPI.Core.Repository.Interfaces;
using MoistureMeterAPI.Test.TestCore.Repository;

namespace MoistureMeterAPI.Test;

public class MoistureMeterRepositoryTest
{
    IMoistureMeterRepository _repository; 

    [SetUp]
    public void Setup()
    {
        var accountRepositoryLogger = Moq.Mock.Of<ILogger<MoistureMeterRepository>>();

        var mongoDatabaseContext = new TestMongoDBContext();

        _repository = new MoistureMeterRepository(accountRepositoryLogger, mongoDatabaseContext);
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
}
