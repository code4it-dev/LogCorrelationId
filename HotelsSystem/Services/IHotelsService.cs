using HotelsSystem.Models;

namespace HotelsSystem.Services
{
    public interface IHotelsService
    {
        IEnumerable<Hotel> Get(DateOnly start, DateOnly end);
    }
}