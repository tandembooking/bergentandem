using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TandemBooking.Models;
using Microsoft.EntityFrameworkCore;
using TandemBooking.Services;
using TandemBooking.ViewModels.AvailabilityOverview;
using TandemBooking.ViewModels.BookingAdmin;

namespace TandemBooking.Controllers
{
    [Authorize]
    public class AvailabilityOverviewController : Controller
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
        
        public AvailabilityOverviewController(TandemBookingContext context)
        {
            _context = context;
        }

        public ActionResult Index(DateTime? date=null)
        { 
            if (!User.IsAdmin() && !User.IsPilot())
            {
                return new UnauthorizedResult();
            }

            if (date == null)
            {
                date = DateTime.UtcNow;
            }

            var startDate = new DateTime(date.Value.Year, date.Value.Month, 1);
            var endDate = startDate.AddMonths(1);

            var nextMonth = startDate.AddMonths(1);
            var prevMonth = startDate.AddMonths(-1);

            int startWeekDay = ((int)startDate.DayOfWeek - 1 + 7) % 7;
            startDate = startDate.AddDays(-startWeekDay);

            int endWeekDay = ((int)endDate.DayOfWeek - 1 + 7) % 7;
            endDate = endDate.AddDays(6 - endWeekDay);

            var pilots = _context.Users.ToList();

            var availabilities = _context.PilotAvailabilities
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .OrderBy(a => a.Date)
                .ToList();

            var pilotBookings = _context.BookedPilots
                .Include(b => b.Booking)
                .Where(b => !b.Canceled && b.Booking.BookingDate >= startDate && b.Booking.BookingDate <= endDate)
                .ToList();

            var unassignedBookings = _context.Bookings
                .Where(b => b.AssignedPilot == null && !b.Canceled && b.BookingDate >= startDate && b.BookingDate <= endDate)
                .ToList();

            var days = new List<AvailabilityOverviewDayViewModel>();

            for (var dayIndex = 0; startDate.AddDays(dayIndex) <= endDate; dayIndex++)
            {
                var curDate = startDate.AddDays(dayIndex);
                var availableToday = availabilities.Where(a => a.Date.Date == curDate).ToList();
                var timeslots = new List<AvailabilityOverviewTimeSlotViewModel>();
                var unassignedToday = unassignedBookings.Where(b => b.BookingDate.Date == curDate);
                var bookingsToday = pilotBookings.Where(a => a.Booking.BookingDate.Date == curDate);

                for (int i = 0; i < 5; i++)
                {
                    var availableAtTimeslot = availableToday.Where(a => a.TimeSlot == i);
                    var unassignedAtTimeslot = unassignedToday.Where(a => a.TimeSlot == i);
                    var bookingsAtTimeslot = bookingsToday.Where(a => a.Booking.TimeSlot == i);
                    timeslots.Add(new AvailabilityOverviewTimeSlotViewModel
                    {
                        TimeSlot = i,
                        Availabilities = availableAtTimeslot.ToList(),
                        PilotBookings = bookingsAtTimeslot.ToList(),
                        UnassignedBookings = unassignedAtTimeslot.ToList(),
                        FreePilots = availableAtTimeslot.Where(a => bookingsAtTimeslot.All(b => b.Pilot != a.Pilot)).ToList(),
                    });
                }
                
                
                
                days.Add(new AvailabilityOverviewDayViewModel()
                {
                    Date = curDate,
                    TimeSlots = timeslots,
                    
                    InPast = curDate < DateTime.Now.Date,
                });                
            }

            return View(new AvailabilityOverviewViewModel()
            {
                Next = nextMonth,
                Prev = prevMonth,
                MonthName = $"{_monthNames[date.Value.Month - 1]} {date.Value.Year}",
                StartDate = startDate,
                EndDate = endDate,
                Days = days,
            });
        }
    }
}