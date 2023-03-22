using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TandemBooking.Services
{
    public class BookingCoordinatorSettings
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int DefaultPassengerFee { get; set; }
    }
}
