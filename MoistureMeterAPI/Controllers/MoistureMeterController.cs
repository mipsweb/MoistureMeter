using Microsoft.AspNetCore.Mvc;
using MoistureMeterAPI.Controllers.Models;
using MoistureMeterAPI.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace MoistureMeterAPI.Controllers
{
    [ApiController]
    [Route("api/moisturemeter")]        
    public class MoistureMeterController : ControllerBase
    {
        ILogger<MoistureMeterController> _logger;
        IMoistureMeterService _moistureMeterService;

        public MoistureMeterController(ILogger<MoistureMeterController> logger, IMoistureMeterService moistureMeterService)
        {
            _logger = logger;
            _moistureMeterService = moistureMeterService;
        }

        [HttpGet(Name = "Pagination")]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest request)
        {
            _logger.LogInformation("Pagination");

            try
            {
                var result = await _moistureMeterService.GetPaginationResult(request.PageSize, request.LastId);

                var resultList = new List<MoistureMeterReadingDto>();
               if(result.Result != null)
                {
                    resultList = result.Result.Select(c => new MoistureMeterReadingDto
                    {
                        Id = c.Id.ToString(),
                        Measure = c.Measure,
                        Timestamp = c.Timestamp,
                    }).ToList();
                }

                var moistureMeterPaginationResult = new MoistureMeterPaginationResult
                {
                    Result = resultList,
                    Rows = (int)result.Rows
                };

                return Ok(moistureMeterPaginationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Pagination failed. Request: {JsonConvert.SerializeObject(request)}");

                return BadRequest(ex.Message);
            }
        }
    }
}
