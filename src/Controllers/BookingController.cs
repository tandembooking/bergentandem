using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TandemBooking.Models;
using TandemBooking.Services;
using TandemBooking.ViewModels.Booking;

namespace TandemBooking.Controllers
{

    public class BookingController : Controller
    {
        private dynamic _monthNames;
        private readonly ILogger<BookingController> _logger;
        private readonly INexmoService _nexmo;
        private readonly TandemBookingContext _context;
        private readonly BookingService _bookingService;
        private readonly ContentService _contentService;
        private readonly BookingCoordinatorSettings _bookingCoordinatorSettings;
        private readonly MessageService _messageService;

        public BookingController(INexmoService nexmo, TandemBookingContext context, BookingCoordinatorSettings bookingCoordinatorSettings, BookingService bookingService, MessageService messageService, ContentService contentService, ILogger<BookingController> logger)
        {
            _nexmo = nexmo;
            _context = context;
            _bookingCoordinatorSettings = bookingCoordinatorSettings;
            _bookingService = bookingService;
            _messageService = messageService;
            _logger = logger;
            _contentService = contentService;
            _monthNames = _contentService.content.booking.calender.months;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new BookingViewModel()
            {
                Stage = 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(BookingViewModel input, string Action = "nothing")
        {
           
            try
            {
                if (ModelState.IsValid)
                {

                    var stage = input.Stage;
                    if (Action.Contains("back"))
                    {
                        stage-=2;
                    }
                    
                    if (stage == 1) {
                        
                        if(!ValidatePassengers(input, Action))
                        {
                            input.Stage = 0;
                            return View(input);
                        }
                    }

                    if (stage == 2){
                        bool passengersValidated = ValidatePassengers(input, Action);
                        if (Action.Contains("month"))
                        {
                            input.Stage = 1;
                            return View(input);
                        }
                        if(!passengersValidated)
                        {
                            input.Stage = 0;
                            return View(input);
                        }
                        if (!ValidateTime(input))
                        {
                            input.Stage = 1;
                            return View(input);
                        }
                        
                    }

                    if (stage == 3)
                    {
                        await ValidateContact(input);
                        if (input.Stage == 3)
                        {
                            return await CreateBooking(input);
                        }
                        else
                        {
                            
                            ValidatePassengers(input, Action);
                            return View(input);
                        }
                    }
                    input.Stage = stage;
                }
                else
                {
                    input.Stage--;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, $"Error creating booking, {ex.Message}, {ex}");
                ModelState.AddModelError("", $"An unexpected error occured during booking, please contact the booking coordinator, {_bookingCoordinatorSettings.Name}, on {_bookingCoordinatorSettings.PhoneNumber.AsPhoneNumber()}");
            }
            
            return View(input);
        }
        private bool ValidateTime(BookingViewModel input)
        {
            if (input.Date == null)
            {
                ModelState.AddModelError("Date", "Please select a valid date");
                return false;
            }
            var time = input.TimeSlot ?? -1;
            if (time > 5 || time < 0)
            {
                ModelState.AddModelError("TimeSlot", "Please select a time slot");
                return false;
            }
            return true;
        }
        private async Task<ActionResult> ValidateContact(BookingViewModel input)
        {
            
            //validate phone number
            var phoneNumber =  await _nexmo.FormatPhoneNumber(input.PhoneNumber);
            if (phoneNumber == null)
            {
                ModelState.AddModelError("PhoneNumber", "Please enter a valid phone number");
                input.Stage--;
                return View(input);
            }

            if (string.IsNullOrWhiteSpace(input.Name))
            {
                ModelState.AddModelError("Name", "Please enter your name");
                input.Stage--;
                return View(input);
            }

            return View(input);
        }
        private bool ValidatePassengers(BookingViewModel input, string Action)
        {

            if (input.AdditionalPassengers?.Any(a => string.IsNullOrWhiteSpace(a.Name)) ?? false)
            {
                ModelState.AddModelError("AdditionalPassengers",
                    "Please enter the name of each passenger");
                return false;
            }

            if (input.AdditionalPassengers?.Any(a => (a.Weight ?? -1) == -1) ?? false)
            {
                ModelState.AddModelError("AdditionalPassengers",
                    "Please enter the weight of each passenger");
                return false;
            }

            List<AdditionalPassengerViewModel> passengers;
            if (input.AdditionalPassengers != null)
            {
                passengers = input.AdditionalPassengers
                 ?.Select((a) => new AdditionalPassengerViewModel
                 {
                     Name = a.Name,
                     Weight = a.Weight,
                 })
                 .ToList();
            }
            else
            {
                passengers = new List<AdditionalPassengerViewModel>();
            }
        
            if (Action.Contains("next_month"))
            {

                input.Calender = UpdateBookingCalendar(passengers, input.NextDate ?? DateTime.UtcNow);
                
            }
            else if (Action.Contains("prev_month"))
            {

                input.Calender = UpdateBookingCalendar(passengers, input.PrevDate ?? DateTime.UtcNow);
                
            }
            else
            {
                input.Calender = UpdateBookingCalendar(passengers, input.Date?? DateTime.UtcNow);
            }
            input.NextDate = input.Calender.Next;
            input.PrevDate = input.Calender.Prev;
            return true;

        }
        private async Task<ActionResult> CreateBooking(BookingViewModel input)
        {


            var phoneNumber = await _nexmo.FormatPhoneNumber(input.PhoneNumber);
            var additionalPassengers = input.AdditionalPassengers
                ?.Select((a, i) => new
                {
                    Name = a.Name,
                    Weight = a.Weight,
                    Index = i + 1,
                })
                .ToList();
            if (additionalPassengers.Count == 0)
            {
                ModelState.AddModelError("Passengers", "No passengers found");
                input.Stage = 2;
                return View(input);
            }

            //Create separate bookings for the additional passengers
            var bookings = new List<Booking>();
            foreach(var passenger in additionalPassengers)
            {
                var commentExtra = "Contact person: " + input.Name;
                if (additionalPassengers.Count > 1)
                {
                    commentExtra = $"booking {passenger.Index }/{additionalPassengers.Count()}";
                }
                Booking primaryBooking = null;
                if (passenger.Index > 1)
                {
                    primaryBooking = bookings[0];
                }

                var additionalBooking = new Booking()
                {
                    BookingDate = input.Date.Value.Date,
                    TimeSlot = input.TimeSlot ?? -1,
                    PrimaryBooking = primaryBooking,
                    DateRegistered = DateTime.UtcNow,
                    PassengerEmail = input.Email,
                    PassengerName = passenger.Name,
                    PassengerWeight = passenger.Weight,
                    PassengerPhone = phoneNumber,
                    PassengerFee = _contentService.content.booking.info.price.value,
                    Comment = $"{input.Comment}, {commentExtra}",
                    BookingEvents = new List<BookingEvent>(),
                };
                bookings.Add(additionalBooking);
            }


            //Assign pilots
            await _bookingService.AssignNewPilotAsync(bookings);
            _context.SaveChanges();
            
            await _messageService.SendNewBookingMessage(bookings[0], bookings.Skip(1).ToArray(), true, true);
            return RedirectToAction("Confirm", new { bookingId = bookings[0].Id });
        }
        public ActionResult Confirm(string bookingId)
        {

            return View();
        }
        public BookingCalendarViewModel UpdateBookingCalendar(List<AdditionalPassengerViewModel> passengers, DateTime date)
        {

            var startDate = new DateTime(date.Year, date.Month, 1);
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




            var days = new List<BookingCalendarDayViewModel>();

            for (var dayIndex = 0; startDate.AddDays(dayIndex) <= endDate; dayIndex++)
            {
                var curDate = startDate.AddDays(dayIndex);
                var availableToday = availabilities.Where(a => a.Date.Date == curDate).ToList();

                var bookingsToday = pilotBookings.Where(a => a.Booking.BookingDate.Date == curDate).ToList();

                var timeSlots = new List<BookingCalendarTimeSlotViewModel>();

                for (var timeSlotIndex = 0; timeSlotIndex < 5; timeSlotIndex++)
                {
                    var availableNow = availableToday.Where(a => a.TimeSlot == timeSlotIndex);
                    
                    var bookingsNow = bookingsToday.Where(a => a.Booking.TimeSlot == timeSlotIndex);
                    var freeNow = availableNow.Where(a => bookingsNow.All(b => b.Pilot != a.Pilot));
                   
                    var possibleWithPassengers = PassengerAssigment.DistributePilots(freeNow.ToList(), passengers);

                    timeSlots.Add(new BookingCalendarTimeSlotViewModel()
                    {
                        TimeSlot = timeSlotIndex,
                        AvailablePilots = availableNow.ToList(),
                        FreePilots = availableNow.Where(a => bookingsNow.All(b => b.Pilot != a.Pilot)).ToList(),
                        PossibleNow = possibleWithPassengers,
                        InPast = curDate < DateTime.Now.Date,
                    });
                }
                days.Add(new BookingCalendarDayViewModel()
                {
                    Date = curDate,
                    TimeSlots = timeSlots,
                    InPast = curDate < DateTime.Now.Date,
                });
            }

            return new BookingCalendarViewModel()
            {
                Next = nextMonth,
                Prev = prevMonth,
                MonthName = $"{_monthNames[date.Month - 1]} {date.Year}",
                StartDate = startDate,
                EndDate = endDate,
                Days = days,
                Passengers = passengers
            };
        }


    }
}
