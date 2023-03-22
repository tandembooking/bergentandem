using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TandemBooking.Services;
using TandemBooking.Models;
using TandemBooking.ViewModels.BookingAdmin;

namespace TandemBooking.ViewComponents
{
    public class AvailibilityCalender : ViewComponent
    {
        private readonly BookingService _bookingService;
        private readonly TandemBookingContext _context;

        public AvailibilityCalender(BookingService bookingService, TandemBookingContext context)
        {
            _bookingService = bookingService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string controlName, DateTime date, int timeslot)
        {
            return View(new PilotSelectorViewModel
            {
                AvailablePilots = await _bookingService.FindAvailablePilotsAsync(date, timeslot, true),
                ControlName = controlName,
            });
        }
    }
}
