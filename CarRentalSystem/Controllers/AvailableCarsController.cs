using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AvailableCarsController : ControllerBase
    {
        private static readonly Car[] AllCars = new[]
        {
        new Car("FIAT", "red"),
        new Car("Tesla", "white"),
        new Car("BMW", "black"),
        new Car("Ferrari", "red"),
        new Car("Toyota", "purple"),
        new Car("FIAT", "purple"),
        new Car("Tesla", "black"),
        new Car("Ferrari", "red"),
        new Car("FIAT", "black"),
        new Car("BMW", ""),
        new Car("Toyota", "red"),
    };

        private readonly ILogger<AvailableCarsController> _logger;

        public AvailableCarsController(ILogger<AvailableCarsController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public ActionResult<IEnumerable<Car>> Get([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                _logger.LogWarning("StartDate cannot be greater than EndDate");
                return BadRequest();
            }
            var daysDiff = (endDate.DayOfYear - startDate.DayOfYear);
            var requiredCarsNumber = Math.Max(1, AllCars.Length - daysDiff);

            Random rd = new Random();

            var cars = AllCars.OrderBy(_ => rd.Next()).Take(requiredCarsNumber);

            if (!cars.Any())
            {
                _logger.LogInformation("No cars available for that date range");
            }

            return Ok(cars);
        }
    }

    public record Car(string Name, string Color);
}