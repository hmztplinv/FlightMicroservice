using Booking.API.Models;

public class BookingStatusChangedEvent
    {
        public string BookingId { get; set; }
        public BookingStatus OldStatus { get; set; }
        public BookingStatus NewStatus { get; set; }
    }
// BookingStatus



