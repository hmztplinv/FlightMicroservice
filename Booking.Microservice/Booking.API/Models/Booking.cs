namespace Booking.API.Models
{
    public class Booking
    {
        public string Id { get; set; }
        public string FlightId { get; set; }
        public string PassengerName { get; set; }
        public string PassengerEmail { get; set; }
        public string SeatNumber { get; set; }
        public decimal Price { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; }
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
}