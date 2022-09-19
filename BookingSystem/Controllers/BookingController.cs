using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public BookingController(ILogger<BookingController> logger, IHttpClientFactory httpClientFactory, IServiceProvider services)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<Booking>> FindBooking([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate.Date > endDate.Date)
            {
                _logger.LogWarning("StartDate {StartDate} cannot be later than EndDate {EndDate}", startDate, endDate);
                return BadRequest();
            }

            IEnumerable<Car> cars = await FindCars(startDate, endDate);
            IEnumerable<Hotel> hotels = await FindHotels(startDate, endDate);

            var booking = new Booking(cars, hotels);

            return Ok(booking);
        }

        private async Task<IEnumerable<Car>> FindCars(DateTime startDate, DateTime endDate)
        {
            var httpClient = _httpClientFactory.CreateClient("cars_system");
            try
            {
                string requestUri = $"/AvailableCars?startDate={startDate.ToString("yyyy-MM-dd")}&endDate={endDate.ToString("yyyy-MM-dd")}";
                var cars = await httpClient.GetFromJsonAsync<IEnumerable<Car>>(requestUri);
                return cars;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while contacting the external service");
                return Enumerable.Empty<Car>();
            }
        }

        private async Task<IEnumerable<Hotel>> FindHotels(DateTime startDate, DateTime endDate)
        {
            var httpClient = _httpClientFactory.CreateClient("hotels_system");
            try
            {
                string requestUri = $"/AvailableHotels?startDate={startDate.ToString("yyyy-MM-dd")}&endDate={endDate.ToString("yyyy-MM-dd")}";
                var hotels = await httpClient.GetFromJsonAsync<IEnumerable<Hotel>>(requestUri);
                return hotels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while contacting the external service");
                return Enumerable.Empty<Hotel>();
            }
        }
    }

    public record Booking(IEnumerable<Car> Cars, IEnumerable<Hotel> hotels);

    public record Car(string Name, string Color);
    public record Hotel(string Name, short Stars);
}