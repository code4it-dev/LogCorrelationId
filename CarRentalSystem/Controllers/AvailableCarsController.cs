using CarRentalSystem.Models;
using CarRentalSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AvailableCarsController : ControllerBase
    {
        private readonly ILogger<AvailableCarsController> _logger;
        private readonly ICarsService _carsService;

        public AvailableCarsController(ILogger<AvailableCarsController> logger, ICarsService carsService)
        {
            _logger = logger;
            _carsService = carsService;
        }

        [HttpGet()]
        public ActionResult<IEnumerable<Car>> Get([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                _logger.LogWarning("StartDate cannot be greater than EndDate");
                return BadRequest();
            }
            var cars = _carsService.Get(DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate));

            if (!cars.Any())
            {
                _logger.LogInformation("No cars available for that date range");
            }

            return Ok(cars);
        }
    }
}