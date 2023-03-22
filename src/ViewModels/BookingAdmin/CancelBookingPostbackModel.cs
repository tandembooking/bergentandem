using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TandemBooking.ViewModels.BookingAdmin
{
    public class CancelBookingPostbackModel
    {
        public string CancelMessage { get; set; }
        public bool NotifyPassenger { get; set; }
    }
}
