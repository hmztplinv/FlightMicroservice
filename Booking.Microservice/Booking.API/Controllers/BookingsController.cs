
using Booking.API.Models;
using Booking.API.Repositories;
using Booking.Microservice.Messages;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public BookingsController(IBookingRepository bookingRepository, IPublishEndpoint publishEndpoint)
        {
            _bookingRepository = bookingRepository;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Booking>>> GetAll()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Booking>> GetById(string id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        [HttpGet("flight/{flightId}")]
        public async Task<ActionResult<IEnumerable<Models.Booking>>> GetByFlightId(string flightId)
        {
            var bookings = await _bookingRepository.GetByFlightIdAsync(flightId);
            return Ok(bookings);
        }

        [HttpGet("passenger/{email}")]
        public async Task<ActionResult<IEnumerable<Models.Booking>>> GetByPassengerEmail(string email)
        {
            var bookings = await _bookingRepository.GetByPassengerEmailAsync(email);
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Models.Booking booking)
        {
            booking.Status = BookingStatus.Pending;
            await _bookingRepository.CreateAsync(booking);

            // Rezervasyon oluşturulduğunda event yayınla
            await _publishEndpoint.Publish(new BookingCreatedEvent
            {
                BookingId = booking.Id,
                FlightId = booking.FlightId,
                PassengerName = booking.PassengerName,
                PassengerEmail = booking.PassengerEmail,
                Price = booking.Price,
                BookingDate = booking.BookingDate
            });

            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Models.Booking booking)
        {
            var existingBooking = await _bookingRepository.GetByIdAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            var oldStatus = existingBooking.Status;
            var newStatus = booking.Status;

            await _bookingRepository.UpdateAsync(id, booking);

            // Eğer rezervasyon durumu değiştiyse event yayınla
            if (oldStatus != newStatus)
            {
                await _publishEndpoint.Publish(new BookingStatusChangedEvent
                {
                    BookingId = id,
                    OldStatus = oldStatus,
                    NewStatus = newStatus
                });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            await _bookingRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/confirm")]
        public async Task<ActionResult> ConfirmBooking(string id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var oldStatus = booking.Status;
            booking.Status = BookingStatus.Confirmed;
            
            await _bookingRepository.UpdateAsync(id, booking);
            
            await _publishEndpoint.Publish(new BookingStatusChangedEvent
            {
                BookingId = id,
                OldStatus = oldStatus,
                NewStatus = BookingStatus.Confirmed
            });
            
            return Ok(booking);
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelBooking(string id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var oldStatus = booking.Status;
            booking.Status = BookingStatus.Cancelled;
            
            await _bookingRepository.UpdateAsync(id, booking);
            
            await _publishEndpoint.Publish(new BookingStatusChangedEvent
            {
                BookingId = id,
                OldStatus = oldStatus,
                NewStatus = BookingStatus.Cancelled
            });
            
            return Ok(booking);
        }
    }
}