namespace Flight.API.Messages
{
    public class FlightCreatedEvent
    {
        public string FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public decimal Price { get; set; }
    }
}