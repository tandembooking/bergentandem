using System.Collections.Generic;
using TandemBooking.Models;
using TandemBooking.Services;

namespace TandemBooking.ViewModels.BookingAdmin
{
    public class BookingCancelViewModel
    {
        public string ErrorMessage { get; set; }
        public TandemBooking.Models.Booking Booking { get; set; }
    }
}