using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TandemBooking.ViewModels.BookingAdmin
{
    public class CompleteBookingViewModel
    {
        public TandemBooking.Models.Booking Booking { get; set; }
        public int PassengerFee { get; set; }
    }
}
