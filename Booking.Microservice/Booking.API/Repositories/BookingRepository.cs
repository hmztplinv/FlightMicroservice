using Booking.API.Models;
using Booking.API.Settings;
using System.Text.Json;
using StackExchange.Redis;

namespace Booking.API.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly string _keyPrefix;

        public BookingRepository(RedisSettings settings)
        {
            _redis = ConnectionMultiplexer.Connect(settings.ConnectionString);
            _database = _redis.GetDatabase();
            _keyPrefix = settings.InstanceName + ":booking:";
        }

        public async Task<List<Models.Booking>> GetAllAsync()
        {
            var bookings = new List<Models.Booking>();
            var server = _redis.GetServer(_redis.GetEndPoints()[0]);
            
            var keys = server.Keys(pattern: _keyPrefix + "*");
            foreach (var key in keys)
            {
                var value = await _database.StringGetAsync(key);
                if (!value.IsNullOrEmpty)
                {
                    bookings.Add(JsonSerializer.Deserialize<Models.Booking>(value));
                }
            }
            
            return bookings;
        }

        public async Task<Models.Booking> GetByIdAsync(string id)
        {
            var value = await _database.StringGetAsync(_keyPrefix + id);
            if (value.IsNullOrEmpty)
            {
                return null;
            }
            
            return JsonSerializer.Deserialize<Models.Booking>(value);
        }

        public async Task<List<Models.Booking>> GetByFlightIdAsync(string flightId)
        {
            var allBookings = await GetAllAsync();
            return allBookings.Where(b => b.FlightId == flightId).ToList();
        }

        public async Task<List<Models.Booking>> GetByPassengerEmailAsync(string email)
        {
            var allBookings = await GetAllAsync();
            return allBookings.Where(b => b.PassengerEmail == email).ToList();
        }

        public async Task CreateAsync(Models.Booking booking)
        {
            if (string.IsNullOrEmpty(booking.Id))
            {
                booking.Id = Guid.NewGuid().ToString();
            }
            
            booking.BookingDate = DateTime.UtcNow;
            
            var json = JsonSerializer.Serialize(booking);
            await _database.StringSetAsync(_keyPrefix + booking.Id, json);
        }

        public async Task UpdateAsync(string id, Models.Booking booking)
        {
            booking.Id = id;
            var json = JsonSerializer.Serialize(booking);
            await _database.StringSetAsync(_keyPrefix + id, json);
        }

        public async Task DeleteAsync(string id)
        {
            await _database.KeyDeleteAsync(_keyPrefix + id);
        }
    }
}