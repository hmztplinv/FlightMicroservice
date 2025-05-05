using Booking.API.Models;
namespace Booking.Microservice.Messages
{
public class BookingStatusChangedEvent
    {
        public string BookingId { get; set; }
        public BookingStatus OldStatus { get; set; }
        public BookingStatus NewStatus { get; set; }
    }
}
// BookingStatus



