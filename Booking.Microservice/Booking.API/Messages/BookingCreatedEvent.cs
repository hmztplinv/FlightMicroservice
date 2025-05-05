namespace Booking.Microservice.Messages
{
public class BookingCreatedEvent
    {
        public string BookingId { get; set; }
        public string FlightId { get; set; }
        public string PassengerName { get; set; }
        public string PassengerEmail { get; set; }
        public decimal Price { get; set; }
        public DateTime BookingDate { get; set; }
    }
}