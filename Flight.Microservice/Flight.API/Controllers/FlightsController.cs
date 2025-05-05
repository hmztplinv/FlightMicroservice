using Flight.API.Messages;
using Flight.API.Models;
using Flight.API.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Flight.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public FlightsController(IFlightRepository flightRepository, IPublishEndpoint publishEndpoint)
        {
            _flightRepository = flightRepository;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Flight>>> GetAll()
        {
            var flights = await _flightRepository.GetAllAsync();
            return Ok(flights);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Flight>> GetById(string id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            return Ok(flight);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Models.Flight flight)
        {
            await _flightRepository.CreateAsync(flight);

            // Uçuş oluşturulduğunda bir event yayınla
            await _publishEndpoint.Publish(new FlightCreatedEvent
            {
                FlightId = flight.Id,
                FlightNumber = flight.FlightNumber,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                Price = flight.Price
            });

            return CreatedAtAction(nameof(GetById), new { id = flight.Id }, flight);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Models.Flight flight)
        {
            var existingFlight = await _flightRepository.GetByIdAsync(id);
            if (existingFlight == null)
            {
                return NotFound();
            }

            await _flightRepository.UpdateAsync(id, flight);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            await _flightRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}