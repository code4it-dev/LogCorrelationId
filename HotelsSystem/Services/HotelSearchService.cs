using HotelsSystem.Models;

namespace HotelsSystem.Services
{
    public class HotelSearchService : IHotelsService
    {
        private readonly ILogger<HotelSearchService> _logger;

        public HotelSearchService(ILogger<HotelSearchService> logger)
        {
            _logger = logger;
            AllHotels = CreateFakeHotels();
            AllAvailableHotels = CreateFakeAvailability(AllHotels);
        }

        public List<Hotel> AllHotels { get; }
        private List<HotelAvailability> AllAvailableHotels { get; }

        public IEnumerable<Hotel> Get(DateOnly start, DateOnly end)
        {
            List<DateOnly> dateRange = CreateDateRange(start, end);

            List<Hotel> hotels = new List<Hotel>();

            _logger.LogInformation($"There are currently {AllHotels.Count} hotels in our system");

            foreach (var availabilty in AllAvailableHotels)
            {
                if (dateRange.All(date => availabilty.Availability.Contains(date)))
                    hotels.Add(availabilty.Hotel);
            }

            if (hotels.Any())
            {
                _logger.LogInformation("We have found " + hotels.Count + " hotels for the date range {StartDate} - {EndDate}", start, end);
            }
            else
            {
                _logger.LogWarning("We haven't found hotels for the specified date range. {StartDate} - {EndDate}", start, end);
            }

            return hotels;
        }

        private static List<DateOnly> CreateDateRange(DateOnly start, DateOnly end)
        {
            var dateRange = new List<DateOnly>();
            DateOnly dtm = start;
            while (dtm <= end)
            {
                dateRange.Add(dtm);
                dtm = dtm.AddDays(1);
            }

            return dateRange;
        }

        private List<HotelAvailability> CreateFakeAvailability(List<Hotel> allHotels)
        {
            Random random = new Random();
            List<HotelAvailability> hotelAvailabilities = new List<HotelAvailability>();

            foreach (var hotel in allHotels)
            {
                var availability = new HotelAvailability(hotel);

                var totalDays = random.Next(20);
                for (int i = 0; i < totalDays; i++)
                {
                    var baseday = DateTime.Now.AddDays(random.Next(-100, 100));

                    var availabilityLength = random.Next(10);

                    for (int j = 0; j < availabilityLength; j++)
                    {
                        availability.AddDay(DateOnly.FromDateTime(baseday.AddDays(j)));
                    }
                }
                hotelAvailabilities.Add(availability);
            }

            return hotelAvailabilities;
        }

        private List<Hotel> CreateFakeHotels()
        {
            return new List<Hotel> {
            new Hotel(1, "Miramare",2),
            new Hotel(2, "Rock College", 3),
            new Hotel(3, "Saint John", 5),
            new Hotel(4, "Mary", 4),
            new Hotel (5, "Il Tugurio", 1),
            new Hotel(6, "Davide's place", 5),
            new Hotel(7,"Glare", 3),
            new Hotel(8, "Most Wanted", 4),
            new Hotel(9, "Roses and guns",3)
            };
        }

        private class HotelAvailability
        {
            private SortedSet<DateOnly> _availability;

            public HotelAvailability(Hotel hotel)
            {
                Hotel = hotel;
                _availability = new SortedSet<DateOnly>();
            }

            public Hotel Hotel { get; }
            public List<DateOnly> Availability => _availability.ToList();

            public void AddDay(DateOnly date) => _availability.Add(date);
        }
    }
}