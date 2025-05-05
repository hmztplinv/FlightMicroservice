using Flight.API.Models;

namespace Flight.API.Repositories
{
    public interface IFlightRepository
    {
        Task<List<Models.Flight>> GetAllAsync();
        Task<Models.Flight> GetByIdAsync(string id);
        Task<Models.Flight> GetByFlightNumberAsync(string flightNumber);
        Task CreateAsync(Models.Flight flight);
        Task UpdateAsync(string id, Models.Flight flight);
        Task DeleteAsync(string id);
    }
}