using CarRentalSystem.Models;

namespace CarRentalSystem.Services
{
    public class CarsService : ICarsService
    {
        private readonly ILogger<CarsService> _logger;

        public CarsService(ILogger<CarsService> logger)
        {
            _logger = logger;
            AllCars = CreateFakeCars();
            AllAvailableCars = CreateFakeAvailability(AllCars);
        }

        public List<Car> AllCars { get; }
        private List<CarAvailability> AllAvailableCars { get; }

        public IEnumerable<Car> Get(DateOnly start, DateOnly end)
        {
            List<DateOnly> dateRange = CreateDateRange(start, end);

            List<Car> cars = new List<Car>();

            _logger.LogInformation($"There are currently {AllCars.Count} cars in our system");

            foreach (var availabilty in AllAvailableCars)
            {
                if (dateRange.All(date => availabilty.Availability.Contains(date)))
                    cars.Add(availabilty.Car);
            }

            if (cars.Any())
            {
                _logger.LogInformation("We have found " + cars.Count + " cars for the date range {StartDate} - {EndDate}", start, end);
            }
            else
            {
                _logger.LogWarning("We haven't found cars for the specified date range. {StartDate} - {EndDate}", start, end);
            }

            return cars;
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

        private List<CarAvailability> CreateFakeAvailability(List<Car> allCars)
        {
            Random random = new Random();
            List<CarAvailability> carAvailabilities = new List<CarAvailability>();

            foreach (var car in allCars)
            {
                var availability = new CarAvailability(car);

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
                carAvailabilities.Add(availability);
            }

            return carAvailabilities;
        }

        private List<Car> CreateFakeCars()
        {
            return new List<Car>  {
                new Car(1,"FIAT", "red"),
                new Car(2,"Tesla", "white"),
                new Car(3,"BMW", "black"),
                new Car(4, "Ferrari", "red"),
                new Car(5, "Toyota", "purple"),
                new Car(6,"FIAT", "purple"),
                new Car(7,"Tesla", "black"),
                new Car(8,"Ferrari", "red"),
                new Car(9,"FIAT", "black"),
            };
        }

        private class CarAvailability
        {
            private SortedSet<DateOnly> _availability;

            public CarAvailability(Car car)
            {
                Car = car;
                _availability = new SortedSet<DateOnly>();
            }

            public Car Car { get; }
            public List<DateOnly> Availability => _availability.ToList();

            public void AddDay(DateOnly date) => _availability.Add(date);
        }
    }
}