using Microsoft.AspNetCore.Mvc;

namespace MoistureMeterAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]        
    public class MoistureMeterController : ControllerBase
    {
        ILogger<MoistureMeterController> _logger;

        public MoistureMeterController(ILogger<MoistureMeterController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Hello")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
