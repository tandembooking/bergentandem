using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TandemBooking.Models
{
    public class CompletedBookingsViewModel
    {
        public int Year { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Booking> Bookings { get; set; }
        public string PilotId { get; set; }
        public string PilotName { get; set; }
    }
}
