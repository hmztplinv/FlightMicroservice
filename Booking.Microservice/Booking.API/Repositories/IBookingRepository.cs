using Booking.API.Models;

namespace Booking.API.Repositories
{
    public interface IBookingRepository
    {
        Task<List<Models.Booking>> GetAllAsync();
        Task<Models.Booking> GetByIdAsync(string id);
        Task<List<Models.Booking>> GetByFlightIdAsync(string flightId);
        Task<List<Models.Booking>> GetByPassengerEmailAsync(string email);
        Task CreateAsync(Models.Booking booking);
        Task UpdateAsync(string id, Models.Booking booking);
        Task DeleteAsync(string id);
    }
}