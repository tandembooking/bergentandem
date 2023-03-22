namespace TandemBooking.Models
{
    public class BookingsByPilotViewModelItem
    {
        public string PilotId { get; set; }
        public string PilotName { get; set; }
        public int CompletedFlights { get; set; }
        public int FlightsMissingStatus { get; set; }
    }
}