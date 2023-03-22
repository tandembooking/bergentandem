using System.Collections.Generic;
using TandemBooking.Models;
using TandemBooking.Services;

namespace TandemBooking.ViewModels.BookingAdmin
{
    public class BookingAssignPilotViewModel
    {
        public string ErrorMessage { get; set; }
        public TandemBooking.Models.Booking Booking { get; set; }
        public List<AvailablePilot> AvailablePilots { get; set; }
    }
}