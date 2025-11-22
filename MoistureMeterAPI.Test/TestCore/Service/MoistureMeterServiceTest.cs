using Microsoft.Extensions.Logging;
using MoistureMeterAPI.Core.Models;
using MoistureMeterAPI.Core.Repository.Interfaces;
using MoistureMeterAPI.Core.Services;
using MoistureMeterAPI.Core.Services.Interfaces;
using Moq;

namespace MoistureMeterAPI.Test;

public class MoistureMeterServiceTest
{
    IMoistureMeterService moistureMeterService;

    [SetUp]
    public void Setup()
    {
        var moistureMeterServiceLogger = Moq.Mock.Of<ILogger<MoistureMeterService>>();

        var moistureMeterReposityMock = new Moq.Mock<IMoistureMeterRepository>();
        moistureMeterReposityMock.Setup(c => c.Insert(Moq.It.IsAny<Core.Models.MoistureMeterReading>()))
            .Returns(Task.FromResult(true));

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

        var paginateResult = new PaginationResult<MoistureMeterReading>
        {
            Result = seedDataList
            .Take(100)
            .ToList(),
            Rows = seedDataList.Count
        };

        moistureMeterReposityMock.Setup(c => c.GetPaginationResult(It.IsAny<int>(), It.IsAny<MoistureMeterReading>()))
            .ReturnsAsync(paginateResult);

        moistureMeterService = new MoistureMeterService(moistureMeterServiceLogger, moistureMeterReposityMock.Object);
    }

    [Test]
    public async Task MoistureMeterServiceTest_Verify_Record_Can_Be_Inserted()
    {
        var reading = new MoistureMeterReading
        {
            Measure = 100,
            Timestamp = DateTime.UtcNow,
        };

        bool result = await moistureMeterService.Insert(reading);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task MoistureMeterServiceTest_Verify_Pagination()
    {
        MoistureMeterReading? lastResult = null;
        int pageSize = 100;

        PaginationResult<MoistureMeterReading> paginationResult = await moistureMeterService.GetPaginationResult(pageSize, lastResult);

        Assert.That(paginationResult, Is.Not.Null);
        Assert.That(paginationResult.Result, Is.Not.Null);
        Assert.That(paginationResult.Result.Count, Is.LessThanOrEqualTo(pageSize));
        Assert.That(paginationResult.Rows, Is.GreaterThanOrEqualTo(pageSize));
    }
}
