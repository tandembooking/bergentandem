using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TandemBooking.Models;
using TandemBooking.ViewModels;
using TandemBooking.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TandemBooking.Controllers
{
    [Authorize]
    public class PilotAvailabilityController : Controller
    {
        private static string[] _monthNames = new[]
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };

        private readonly TandemBookingContext _context;
        private readonly UserManager _userManager;
        private readonly BookingService _bookingService;

        public PilotAvailabilityController(TandemBookingContext context, UserManager userManager, BookingService bookingService)
        {
            _context = context;
            _userManager = userManager;
            _bookingService = bookingService;
        }

        public ActionResult Index(DateTime? date = null, string userId=null)
        {
            if (date == null)
            {
                date = DateTime.UtcNow;
            }

            if (userId == null ||  (User != null && !User.IsAdmin()))
            {
                userId = _userManager.GetUserId(User);
            }
            var user = _context.Users.Single(u => u.Id == userId);

            var startDate = new DateTime(date.Value.Year, date.Value.Month, 1);
            var endDate = startDate.AddMonths(1);

            var nextMonth = startDate.AddMonths(1);
            var prevMonth = startDate.AddMonths(-1);

            int startWeekDay = ((int) startDate.DayOfWeek - 1 + 7)%7;
            startDate = startDate.AddDays(-startWeekDay);

            int endWeekDay = ((int) endDate.DayOfWeek - 1 + 7)%7;
            endDate = endDate.AddDays(6 - endWeekDay);

            var availabilities = _context.PilotAvailabilities
                .Where(a => a.Pilot.Id == userId)
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .OrderBy(a => a.Date).ThenBy(a => a.TimeSlot)
                .ToList();

            var pilotBookings = _context.BookedPilots
                .Include(b => b.Booking)
                .Where(a => a.Pilot.Id == userId)
                .Where(b => !b.Canceled && b.Booking.BookingDate >= startDate && b.Booking.BookingDate <= endDate)
                .ToList();

            return View(new PilotAvailabilityViewModel()
            {
                Next = nextMonth,
                Prev = prevMonth,
                MonthName = $"{_monthNames[date.Value.Month - 1]} {date.Value.Year}",
                StartDate = startDate,
                EndDate = endDate,
                Availabilities = availabilities,
                PilotBookings = pilotBookings,
                Pilot = user,
            });
        }

        
         [HttpPost]
        async public Task<ActionResult> SetAvailability([FromBody]  SetPilotAvailabilityViewModel[] availabilities)
        {

            for (int i = 0; i < availabilities.Length; i++)
            {
                DateTime currentDate = Convert.ToDateTime(availabilities[i].Date);
                var existingAvailabilities = _context.PilotAvailabilities
                .Where(a => a.Pilot.Id == availabilities[i].PilotID)
                .Where(a => a.Date == currentDate)
                .Where(a => a.TimeSlot == availabilities[i].TimeSlot);

                
                if (availabilities[i].Available && existingAvailabilities.Count() ==0)
                {

                    var pilotAvailability = new PilotAvailability()
                    {
                        Date = currentDate,
                        TimeSlot = availabilities[i].TimeSlot,
                        Pilot = _context.Users.Single(u => u.Id == availabilities[i].PilotID)
                    };
                    _context.PilotAvailabilities.Add(pilotAvailability);

                    var unassignedBookings = _context.Bookings
                    .Where(b => b.AssignedPilot == null && !b.Canceled && b.BookingDate == currentDate && b.TimeSlot == availabilities[i].TimeSlot).ToList();
                    if(unassignedBookings.Count() > 0)
                    {
                        _context.SaveChanges();
                        await _bookingService.AssignNewPilotAsync(unassignedBookings[0]);
                    }

                }
                else if(availabilities[i].Available == false)
                {
                    

                    foreach (var pilotAvailability in existingAvailabilities)
                    {
                        _context.PilotAvailabilities.Remove(pilotAvailability);
                    }
                }
            }

            _context.SaveChanges();

            return Json(new { result = "ok" });
        }


        
    }

}