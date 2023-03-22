using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TandemBooking.ViewModels.BookingAdmin
{
    public class AssignPilotPostbackModel
    {
        public string NewPilotMessage { get; set; }
        public string NewPilotUserId { get; set; }
        public bool NewPilotNotifyPassenger { get; set; }
    }
}
