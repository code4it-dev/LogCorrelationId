using HotelsSystem.Models;
using HotelsSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelsSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AvailableHotelsController : ControllerBase
    {
        private readonly ILogger<AvailableHotelsController> _logger;
        private readonly IHotelsService _hotelsService;

        public AvailableHotelsController(ILogger<AvailableHotelsController> logger, IHotelsService hotelsService)
        {
            _logger = logger;
            _hotelsService = hotelsService;
        }

        [HttpGet()]
        public ActionResult<IEnumerable<Hotel>> Get([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                _logger.LogWarning("StartDate cannot be greater than EndDate");
                return BadRequest();
            }
            var cars = _hotelsService.Get(DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate));

            if (!cars.Any())
            {
                _logger.LogInformation("No hotels available for that date range");
            }

            return Ok(cars);
        }
    }
}