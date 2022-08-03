using CarRentalSystem.Models;

namespace CarRentalSystem.Services
{
    public interface ICarsService
    {
        IEnumerable<Car> Get(DateOnly start, DateOnly end);
    }
}