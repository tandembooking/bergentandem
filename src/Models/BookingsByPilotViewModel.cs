using System.Collections.Generic;

namespace TandemBooking.Models
{
    public class BookingsByPilotViewModel
    {
        public int Year { get; set; }
        public List<BookingsByPilotViewModelItem> PilotStats { get; set; }
    }
}