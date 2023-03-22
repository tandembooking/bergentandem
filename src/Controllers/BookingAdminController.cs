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
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1;
using TandemBooking.Services;
using TandemBooking.ViewModels.BookingAdmin;

namespace TandemBooking.Controllers
{
    [Authorize(Policy = "IsValidated")]
    public class BookingAdminController : Controller
    {
        private readonly ILogger<BookingAdminController> _logger;
        private readonly TandemBookingContext _context;
        private readonly BookingService _bookingService;
        private readonly MessageService _messageService;
        private readonly BookingCoordinatorSettings _bookingCoordinatorSettings;
        private readonly UserManager _userManager;
        private readonly INexmoService _nexmo;

        public BookingAdminController(TandemBookingContext context, INexmoService nexmo, BookingService bookingService,
            MessageService messageService, BookingCoordinatorSettings bookingCoordinatorSettings,
            UserManager userManager, ILogger<BookingAdminController> logger)
        {
            _context = context;
            _bookingService = bookingService;
            _messageService = messageService;
            _bookingCoordinatorSettings = bookingCoordinatorSettings;
            _userManager = userManager;
            _logger = logger;
            _nexmo = nexmo;
        }

        #region List Bookings

        public ActionResult Index(string userId = null)
        {
            var bookingQuery = _context.Bookings
                .Include(b => b.AssignedPilot)
                .Where(u => u.BookingDate >= DateTime.UtcNow.AddDays(-30))
                .OrderByDescending(u => u.BookingDate).ThenBy(u => u.DateRegistered)
                .AsQueryable();

            //non-admin users can only see their own bookings
            if (userId != null)
            {
                bookingQuery = bookingQuery.Where(b => b.AssignedPilot.Id == userId);
            }

            //get at least 30 bookings and all bookings from the last 30days
            var bookings = bookingQuery
                .AsEnumerable()
                .TakeWhile((booking, index) => booking.BookingDate > DateTime.Now.AddDays(-30) || index < 30)
                .ToList();

            return View(bookings);
        }

        #endregion

        #region Create New Booking

        [HttpGet]
        public ActionResult Create(Guid? cloneBookingId = null)
        {
            CreateBookingViewModel vm;
            if (cloneBookingId.HasValue)
            {
                var originalBooking = _context.Bookings
                    .FirstOrDefault(b => b.Id == cloneBookingId);
                if (originalBooking == null)
                {
                    return NotFound();
                }

                vm = new CreateBookingViewModel
                {
                    Date = originalBooking.BookingDate,
                    TimeSlot = originalBooking.TimeSlot,
                    Email = originalBooking.PassengerEmail,
                    PhoneNumber = originalBooking.PassengerPhone.AsPhoneNumber(),
                    Name = originalBooking.PassengerName + " +1",
                    Weight = originalBooking.PassengerWeight,
                    Comment = originalBooking.Comment,
                    PrimaryBookingId = originalBooking.Id,
                    NotifyPassenger = false,
                    NotifyPilot = true,
                    PassengerFee = (int)originalBooking.PassengerFee,
                };
            }
            else
            {
                vm = new CreateBookingViewModel
                {
                    Date = DateTime.Today,
                };
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateBookingViewModel input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (input.Date == null)
                    {
                        ModelState.AddModelError("Date", "Please select a date");
                        return View(input);
                    }

                    //if phone number is provided, make sure it is valid. 
                    //if no phone number is provided, that's ok as well
                    string phoneNumber = null;
                    if (!string.IsNullOrEmpty(input.PhoneNumber))
                    {
                        phoneNumber = await _nexmo.FormatPhoneNumber(input.PhoneNumber);
                        if (phoneNumber == null)
                        {
                            ModelState.AddModelError("PhoneNumber", "Please enter a valid phone number");
                            return View(input);
                        }
                    }

                    //Fetch primary booking if this is a clone
                    Booking primaryBooking = null;
                    if (input.PrimaryBookingId != null)
                    {
                        primaryBooking = _context.Bookings
                            .FirstOrDefault(b => b.Id == input.PrimaryBookingId);
                    }

                    //create booking
                    var booking = new Booking()
                    {
                        BookingDate = input.Date.Value.Date,
                        TimeSlot = input.TimeSlot,
                        DateRegistered = DateTime.UtcNow,
                        PassengerEmail = input.Email,
                        PassengerName = input.Name,
                        PassengerWeight = input.Weight,
                        PassengerPhone = phoneNumber,
                        Comment = input.Comment,
                        BookingEvents = new List<BookingEvent>(),
                        PrimaryBooking = primaryBooking,
                        PassengerFee = input.PassengerFee,
                    };
                    _context.Add(booking);

                    //assign pilot
                    if (string.IsNullOrEmpty(input.PilotId))
                    {
                        //if no pilot is selected, find a new one
                        await _bookingService.AssignNewPilotAsync(booking);
                    }
                    else
                    {
                        //or use pilot as provided
                        var pilot = await _userManager.FindByIdAsync(input.PilotId);
                        _bookingService.AssignNewPilot(booking, pilot);
                    }
                    _context.SaveChanges();

                    await _messageService.SendNewBookingMessage(booking, new Booking[] {}, input.NotifyPassenger, input.NotifyPilot);

                    //redirect to edit action
                    return RedirectToAction("Details", new {id = booking.Id});
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, $"Error creating booking, {ex.Message}, {ex}");
                ModelState.AddModelError("", $"An unexpected error occured during booking, please contact the booking coordinator, {_bookingCoordinatorSettings.Name}, on {_bookingCoordinatorSettings.PhoneNumber}");
            }

            return View(input);
        }

        public ActionResult PilotSelector(string controlName, DateTime date)
        {
            return ViewComponent("PilotSelector", new {controlName, date});
        }

        #endregion

        #region Edit Booking

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var booking = _context.Bookings
                .FirstOrDefault(b => b.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            var vm = new EditBookingViewModel
            {
                Id = booking.Id,
                BookingDate = booking.BookingDate,
                TimeSlot = booking.TimeSlot,
                PassengerName = booking.PassengerName,
                PassengerWeight = booking.PassengerWeight,
                PassengerEmail = booking.PassengerEmail,
                PassengerPhone = booking.PassengerPhone.AsPhoneNumber(),
                PassengerFee = (int)booking.PassengerFee,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Guid id, EditBookingViewModel input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //if phone number is provided, make sure it is valid. 
                    //if no phone number is provided, that's ok as well
                    string phoneNumber = null;
                    if (!string.IsNullOrEmpty(input.PassengerPhone))
                    {
                        phoneNumber = await _nexmo.FormatPhoneNumber(input.PassengerPhone);
                        if (phoneNumber == null)
                        {
                            ModelState.AddModelError("PassengerPhone", "Please enter a valid phone number");
                            return View(input);
                        }
                    }

                    var booking = _context.Bookings
                        .Include(b => b.AssignedPilot)
                        .FirstOrDefault(b => b.Id == id);
                    if (booking == null)
                    {
                        return NotFound();
                    }

                    var userId = _userManager.GetUserId(User);
                    if (!User.IsAdmin() && booking.AssignedPilot.Id != userId)
                    {
                        return RedirectToAction("Details", new { id = id, errorMessage = "Only admin or currently assigned pilot can edit booking" });
                    }

                    //create booking
                    booking.BookingDate = input.BookingDate;
                    booking.TimeSlot = input.TimeSlot;
                    booking.PassengerName = input.PassengerName;
                    booking.PassengerWeight = input.PassengerWeight;
                    booking.PassengerPhone = phoneNumber;
                    booking.PassengerEmail = input.PassengerEmail;
                    booking.PassengerFee = input.PassengerFee;
                    _context.SaveChanges();

                    //redirect to edit action
                    return RedirectToAction("Details", new { id = booking.Id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, $"Error editing booking {id}, {ex.Message}, {ex}");
                ModelState.AddModelError("", $"An unexpected error occured during editing");
            }

            return View(input);
        }

        #endregion

        #region Booking Details

        [HttpGet]
        public async Task<ActionResult> Details(Guid id, string errorMessage = null)
        {
            var booking = _context.Bookings
                .Include(b => b.BookedPilots).ThenInclude(bp => bp.Pilot)
                .Include(b => b.BookingEvents).ThenInclude(be => be.User)
                .Include(b => b.AssignedPilot)
                .Include(b => b.AdditionalBookings).ThenInclude(b => b.AssignedPilot)
                .Include(b => b.PrimaryBooking).ThenInclude(b => b.AssignedPilot)
                .Include(b => b.PrimaryBooking).ThenInclude(b => b.AdditionalBookings)
                .FirstOrDefault(b => b.Id == id);

            var vm = new BookingDetailsViewModel
            {
                ErrorMessage = errorMessage,
                Booking = booking,
                Editable = User.IsAdmin() || booking.AssignedPilot?.Id == _userManager.GetUserId(User),
                AvailablePilots = await _bookingService.FindAvailablePilotsAsync(booking.BookingDate, booking.TimeSlot, true),
            };

            ViewBag.ErrorMessage = errorMessage;
            return View(vm);
        }

        #endregion

        #region Cancel

        [HttpGet]
        public async Task<ActionResult> Cancel(Guid id)
        {
            var booking = await _context.Bookings
                .Include(b => b.AssignedPilot)
                .FirstOrDefaultAsync(b => b.Id == id);

            var userId = _userManager.GetUserId(User);
            if (!User.IsAdmin() && booking.AssignedPilot.Id != userId)
            {
                return RedirectToAction("Details", new { id = id, errorMessage = "Only admin or currently assigned pilot can cancel booking" });
            }

            var vm = new BookingCancelViewModel
            {
                Booking = booking,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Cancel(Guid id, CancelBookingPostbackModel input)
        {
            var booking = await _context.Bookings
                .Include(b => b.AssignedPilot)
                .Include(b => b.BookingEvents)
                .FirstOrDefaultAsync(b => b.Id == id);

            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            if (!User.IsAdmin() && booking.AssignedPilot?.Id != userId)
            {
                return RedirectToAction("Details", new { id = id, errorMessage = "Only admin or currently assigned pilot can cancel booking" });
            }

            var cancelMessage = input.CancelMessage;
            if (string.IsNullOrWhiteSpace(cancelMessage))
            {
                cancelMessage = "- no cancel message";
            }

            booking.Canceled = true;
            booking.Completed = false;

            if (input.NotifyPassenger)
            {
                await _messageService.SendCancelMessage(cancelMessage, booking, user);
                _bookingService.AddEvent(booking, User, $"Canceled due to {cancelMessage} (notified passenger)");
            }
            else
            {
                _bookingService.AddEvent(booking, User, $"Canceled due to {cancelMessage} (did not notify passenger)");
            }

            foreach (var bookedPilot in _context.BookedPilots.Where(b => b.Booking.Id == booking.Id))
            {
                bookedPilot.Canceled = true;
            }
            _context.SaveChanges();

            return RedirectToAction("Details", new { Id = booking.Id });
        }

        #endregion

        #region Assign Pilot

        [HttpGet]
        public async Task<ActionResult> AssignPilot(Guid id, string errorMessage = null)
        {
            var booking = _context.Bookings
                .Include(b => b.AssignedPilot)
                .Include(b => b.BookingEvents)
                .FirstOrDefault(b => b.Id == id);

            var userId = _userManager.GetUserId(User);
            if (!User.IsAdmin() && booking.AssignedPilot.Id != userId)
            {
                return RedirectToAction("Details", new { id = id, errorMessage = "Only admin or currently assigned pilot can assign new pilot" });
            }

            var vm = new BookingAssignPilotViewModel
            {
                ErrorMessage = errorMessage,
                Booking = booking,
                AvailablePilots = await _bookingService.FindAvailablePilotsAsync(booking.BookingDate, booking.TimeSlot, true),
            };

            ViewBag.ErrorMessage = errorMessage;
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> AssignPilot(Guid id, AssignPilotPostbackModel newPilot)
        {
            var booking = _context.Bookings
                .Include(b => b.AssignedPilot)
                .Include(b => b.BookingEvents)
                .Include(b => b.BookedPilots).ThenInclude(bp => bp.Pilot)
                .FirstOrDefault(b => b.Id == id);

            var userId = _userManager.GetUserId(User);
            if (!User.IsAdmin() && booking.AssignedPilot.Id != userId)
            {
                return RedirectToAction("Details", new { id = id, errorMessage = "Only admin or currently assigned pilot can assign new pilot" });
            }

            //get optional pilot id from input
            var newPilotId = newPilot.NewPilotUserId;

            var originalPilot = booking.AssignedPilot;
            string errorMessage = null;

            //unassign the previous pilot
            if (booking.AssignedPilot != null)
            {
                var removedMessage = $"Removed assigned pilot {booking.AssignedPilot} ({booking.AssignedPilot.PhoneNumber.AsPhoneNumber()})";
                if (!string.IsNullOrEmpty(newPilot.NewPilotMessage))
                {
                    removedMessage = removedMessage + $" due to {newPilot.NewPilotMessage}";
                }
                _bookingService.AddEvent(booking, User, removedMessage);
            }

            ApplicationUser assignedPilot;
            if (string.IsNullOrEmpty(newPilotId))
            {
                //try to find a new pilot (if none are available, set no pilot assigned)
                assignedPilot = await _bookingService.AssignNewPilotAsync(booking);
            }
            else
            {
                assignedPilot = _context.Users.Single(u => u.Id == newPilotId);
                _bookingService.AssignNewPilot(booking, assignedPilot);
            }
            await _context.SaveChangesAsync();

            //notify pilot and passenger of the newly assigned pilot
            var bookingDateString = booking.BookingDate.ToString("dd.MM.yyyy") + " at " + booking.TimeSlot.asTime();
            if (assignedPilot != null)
            {
                await _messageService.SendNewPilotMessage(bookingDateString, booking, originalPilot, newPilot.NewPilotNotifyPassenger);
                if (originalPilot != null)
                {
                    await _messageService.SendPilotUnassignedMessage(booking, originalPilot);
                }

                if (string.IsNullOrEmpty(newPilotId))
                {
                    _bookingService.AddEvent(booking, User, $"Assigned booking to {assignedPilot.Name} ({assignedPilot.PhoneNumber.AsPhoneNumber()})");
                }
                else
                {
                    _bookingService.AddEvent(booking, User, $"Forced booking to {assignedPilot.Name} ({assignedPilot.PhoneNumber.AsPhoneNumber()})");
                }

                if (newPilot.NewPilotNotifyPassenger)
                {
                    _bookingService.AddEvent(booking, User, $"Sent status update to passenger, {booking.PassengerName} ({booking.PassengerPhone.AsPhoneNumber()})");
                }
            }
            else if (originalPilot != null)
            {
                await _messageService.SendMissingPilotMessage(bookingDateString, booking);
                await _messageService.SendPilotUnassignedMessage(booking, originalPilot);
                _bookingService.AddEvent(booking, User, $"No pilots available, sent message to tandem coordinator {_bookingCoordinatorSettings.Name} ({_bookingCoordinatorSettings.PhoneNumber.AsPhoneNumber()})");
                _bookingService.AddEvent(booking, User, $"Sent status update to passenger, {booking.PassengerName} ({booking.PassengerPhone.AsPhoneNumber()})");
            }
            else
            {
                errorMessage = "No pilots available. Please select pilot manually";
                return RedirectToAction("AssignPilot", new { Id = booking.Id, errorMessage });
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { Id = booking.Id, errorMessage });
        }

        [HttpPost]
        public async Task<ActionResult> AddMessage(Guid id, SendMessageViewModel input)
        {
            var booking = _context.Bookings
                .Include(b => b.AssignedPilot)
                .Include(b => b.BookingEvents)
                .FirstOrDefault(b => b.Id == id);

            if (string.IsNullOrWhiteSpace(input.EventMessage))
            {
                return RedirectToAction("Details", new { Id = booking.Id });
            }

            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            if (!User.IsAdmin() && booking.AssignedPilot.Id != userId)
            {
                return RedirectToAction("Index");
            }

            var message = input.EventMessage;
            if (input.SendToPassenger)
            {
                message += " (sent to passenger)";
            }

            _bookingService.AddEvent(booking, User, message);
            _context.SaveChanges();

            await _messageService.SendBookingInformationMessage(input, booking, user);

            return RedirectToAction("Details", new {Id = booking.Id});
        }

        #endregion

        [HttpGet]
        public async Task<ActionResult> Complete(Guid id)
        {
            var booking = await _context.Bookings
                .Include(b => b.AssignedPilot)
                .Include(b => b.BookingEvents)
                .FirstOrDefaultAsync(b => b.Id == id);

            var userId = _userManager.GetUserId(User);
            if (!User.IsAdmin() && booking.AssignedPilot.Id != userId)
            {
                return RedirectToAction("Details", new { Id = booking.Id, errorMessage = "Only admin or currently assigned pilot can confirm booking" });
            }

            return View(new CompleteBookingViewModel()
            {
                Booking = booking,
                PassengerFee = (int)booking.PassengerFee,
            });
        }

        [HttpPost]
        public async Task<ActionResult> Complete(Guid id, CompleteBookingViewModel input)
        {
            var booking = await _context.Bookings
                .Include(b => b.AssignedPilot)
                .Include(b => b.BookingEvents)
                .FirstOrDefaultAsync(b => b.Id == id);

            var userId = _userManager.GetUserId(User);
            if (!User.IsAdmin() && booking.AssignedPilot.Id != userId)
            {
                return RedirectToAction("Details", new { Id = booking.Id, errorMessage = "Only admin or currently assigned pilot can confirm booking" });
            }

            //update passenger fee and completed status
            booking.PassengerFee = input.PassengerFee;
            booking.Canceled = false;
            booking.Completed = true;
            await _context.SaveChangesAsync();

            _bookingService.AddEvent(booking, User, "booking confirmed");
            _context.SaveChanges();

            return RedirectToAction("Details", new { Id = booking.Id });
        }
    }
}