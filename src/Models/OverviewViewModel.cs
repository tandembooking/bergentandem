using TandemBooking.Models;
using System.Collections.Generic;

namespace TandemBooking.Models
{
    public class OverviewViewModel
    {
        public List<Booking> UpcomingBookings { get; set; }
        public List<Booking> MissingPilotBookings { get; set; }
        public List<Booking> RecentBookings { get; set; }
        public List<Booking> CorrespondingBookings { get; set; }
    }
}