using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TandemBooking.Services;

namespace TandemBooking.ViewModels.BookingAdmin
{
    public class BookingDetailsViewModel
    {
        public string ErrorMessage { get; set; }
        public Models.Booking Booking { get; set; } 
        public List<AvailablePilot> AvailablePilots { get; set; }
        public bool Editable { get; set; }
    }
}
    