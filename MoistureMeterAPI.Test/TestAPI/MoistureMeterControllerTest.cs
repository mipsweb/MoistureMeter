using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoistureMeterAPI.Controllers;
using MoistureMeterAPI.Controllers.Models;
using MoistureMeterAPI.Core.Models;
using MoistureMeterAPI.Core.Repository.Interfaces;
using MoistureMeterAPI.Core.Services;
using MoistureMeterAPI.Core.Services.Interfaces;
using System.Net;

namespace MoistureMeterAPI.Test;

public class MoistureMeterControllerTest
{
    MoistureMeterController controller;

    [SetUp]
    public void Setup()
    {
        var moistureMeterControllerLogger = Moq.Mock.Of<ILogger<MoistureMeterController>>();

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

        var moistureMeterServiceMock = new Moq.Mock<IMoistureMeterService>();

        moistureMeterServiceMock.Setup(c => c.GetPaginationResult(Moq.It.IsAny<int>(), Moq.It.IsAny<string>()))
            .Returns(Task.FromResult(new Core.Models.PaginationResult<Core.Models.MoistureMeterReading>
            {
                Result = seedDataList.Take(100).ToList(),
                Rows = seedDataList.Count()
            }));

        controller = new MoistureMeterController(moistureMeterControllerLogger, moistureMeterServiceMock.Object);
    }

    [Test]
    public async Task MoistureMeterController_Verify_Pagination_Returns_Correct_Result()
    {
        IActionResult actionResult = await controller.Pagination(new Controllers.Models.PaginationRequest { PageSize = 100 });

        var contentResult = actionResult as ObjectResult;

        Assert.That(contentResult, Is.Not.Null);
        Assert.That(contentResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));

        var contentResultValue = contentResult.Value as MoistureMeterPaginationResult;

        Assert.That(contentResultValue, Is.Not.Null);

        Assert.That(contentResultValue.Result, Is.Not.Null);
        Assert.That(contentResultValue.Rows, Is.GreaterThanOrEqualTo(100));

        var lastRecord = contentResultValue.Result.Last();

        var actionResultNextPage = await controller.Pagination(new Controllers.Models.PaginationRequest { PageSize = 100, LastId = lastRecord.ToString() });

        var contentResultNextPage = actionResult as ObjectResult;

        Assert.That(contentResultNextPage, Is.Not.Null);
        Assert.That(contentResultNextPage.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));

        var contentResultValueNextPage = contentResultNextPage.Value as MoistureMeterPaginationResult;

        Assert.That(contentResultValueNextPage, Is.Not.Null);

        Assert.That(contentResultValueNextPage.Result, Is.Not.Null);
        Assert.That(contentResultValueNextPage.Rows, Is.GreaterThanOrEqualTo(100));
    }
}
