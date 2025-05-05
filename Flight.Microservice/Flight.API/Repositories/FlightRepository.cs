using Flight.API.Models;
using Flight.API.Settings;
using MongoDB.Driver;

namespace Flight.API.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly IMongoCollection<Models.Flight> _flightsCollection;

        public FlightRepository(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _flightsCollection = database.GetCollection<Models.Flight>(settings.CollectionName);
        }

        public async Task<List<Models.Flight>> GetAllAsync()
        {
            return await _flightsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Models.Flight> GetByIdAsync(string id)
        {
            return await _flightsCollection.Find(f => f.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Models.Flight> GetByFlightNumberAsync(string flightNumber)
        {
            return await _flightsCollection.Find(f => f.FlightNumber == flightNumber).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Models.Flight flight)
        {
            await _flightsCollection.InsertOneAsync(flight);
        }

        public async Task UpdateAsync(string id, Models.Flight flight)
        {
            await _flightsCollection.ReplaceOneAsync(f => f.Id == id, flight);
        }

        public async Task DeleteAsync(string id)
        {
            await _flightsCollection.DeleteOneAsync(f => f.Id == id);
        }
    }
}