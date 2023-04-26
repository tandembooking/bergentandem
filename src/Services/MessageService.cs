using System.Linq;
using System.Threading.Tasks;
using TandemBooking.Models;
using TandemBooking.ViewModels.BookingAdmin;

namespace TandemBooking.Services
{
    public class MessageService
    {
        private readonly SmsService _smsService;
        private readonly IMailService _mailService;
        private readonly TandemBookingContext _context;
        private readonly BookingService _bookingService;
        private readonly ContentService _contService;
        private readonly BookingCoordinatorSettings _bookingCoordinatorSettings;

        public MessageService(SmsService smsService, TandemBookingContext context, BookingService bookingService, BookingCoordinatorSettings bookingCoordinatorSettings, IMailService mailService, ContentService content)
        {
            _smsService = smsService;
            _context = context;
            _bookingService = bookingService;
            _bookingCoordinatorSettings = bookingCoordinatorSettings;
            _mailService = mailService;
            _contService = content;
        }

        private async Task SendPilotMessage(ApplicationUser user, string message, Booking booking)
        {
            await _smsService.Send(user.PhoneNumber, message, booking);
        }

        public async Task SendNewBookingMessage(Booking primaryBooking, Booking[] additionalBookings, bool notifyPassenger, bool notifyPilot)
        {
            var allBookings = new[] {primaryBooking}.Union(additionalBookings).ToArray();
            var bookingDateString = primaryBooking.BookingDate.ToString("dd.MM.yyyy") + " at " + primaryBooking.TimeSlot.asTime();
            var bookingTimeStamp =
                primaryBooking.BookingDate.ToString("yyyyMMdd") + "T" + primaryBooking.TimeSlot.asTime().Replace(":", "") + "00/" +
                primaryBooking.BookingDate.ToString("yyyyMMdd") + "T" + (primaryBooking.TimeSlot + 1).asTime().Replace(":", "") + "00"
                ;
            //send message to pilot or booking coordinator
            foreach (var booking in allBookings)
            {
                var assignedPilot = booking.AssignedPilot;

                if (assignedPilot != null)
                {
                    _bookingService.AddEvent(booking, null,
                        $"Assigned to {assignedPilot.Name} ({assignedPilot.PhoneNumber.AsPhoneNumber()})");

                    //send message to pilot
                    if (notifyPilot)
                    {
                        //sms to pilot
                        if (assignedPilot.SmsNotification)
                        {
                            var message =
                                $"You have a new flight on {bookingDateString}: {booking.PassengerName}, {booking.PassengerEmail}, {booking.PassengerPhone.AsPhoneNumber()}, {booking.Comment}.";
                            await SendPilotMessage(assignedPilot, message, booking);
                        }
                        //mail to pilot
                        if (assignedPilot.EmailNotification)
                        {
                            var subject = $"New flight on {bookingDateString}";
                            var eventURL = $@"https://www.google.com/calendar/render?action=TEMPLATE&text=Tandem+{System.Web.HttpUtility.UrlEncode(booking.PassengerName)}&dates={bookingTimeStamp}&ctz=Europe/Oslo&details={System.Web.HttpUtility.UrlEncode(booking.PassengerName)}%0A{booking.PassengerPhone}%0A{booking.PassengerEmail}";
                            System.Web.HttpUtility.UrlEncode("c# objects");

                            var details = $@"You have been assigned a new flight:
Date:            {bookingDateString}. 
Passenger Name:  {booking.PassengerName},
Passenger Phone: {booking.PassengerPhone.AsPhoneNumber()},
Passenger Email: {booking.PassengerEmail ?? "not specified"}
Comments:
{booking.Comment}";
                            var message = $@"Hi {assignedPilot.Name},

{details}

Booking Url: http://{_contService.content.url}/BookingAdmin/Details/{booking.Id}

Add to google calender: {eventURL}

fly safe!
Booking Coordinator
";
                            await _mailService.Send(assignedPilot.Email, subject, message);
                        }
                    }

                    //send booking assigned message to booking coordinator
                    var bookingCoordinatorMessage =
                        $"New flight on {bookingDateString} assigned to {assignedPilot.Name}, {booking.Comment}";
                    await _smsService.Send(_bookingCoordinatorSettings.PhoneNumber, bookingCoordinatorMessage, booking);
                }
                else
                {
                   


                    _bookingService.AddEvent(booking, null,
                        $"No pilots available, sent message to tandem coordinator {_bookingCoordinatorSettings.Name} ({_bookingCoordinatorSettings.PhoneNumber.AsPhoneNumber()})");

                    //message all pilots of unassigned booking
                    var messageToPilots =
                        $"Available flight on {bookingDateString} at {booking.TimeSlot.asTime()}: {booking.PassengerName}, {booking.PassengerEmail}, {booking.PassengerPhone.AsPhoneNumber()}, {booking.PassengerWeight} kg, {booking.Comment}";
                    var allPilots = _context.Users.Where(p => p.IsPilot || p.IsAdmin).ToList();

                    foreach (var pilot in allPilots)
                    {
                        await _mailService.Send(pilot.Email, $"Available flight on {bookingDateString}", messageToPilots);
                    }

                    //send no pilots available to booking coordinator
                    var message =
                        $"Please find a pilot on {bookingDateString}: {booking.PassengerName}, {booking.PassengerEmail}, {booking.PassengerPhone.AsPhoneNumber()}, {booking.Comment}";
                    await _smsService.Send(_bookingCoordinatorSettings.PhoneNumber, message, booking);
                }
            }

            //send message to passenger
            if (notifyPassenger)
            {
                string passengerMessage;
                if (allBookings.All(b => b.AssignedPilot != null))
                {
                    var assignedPilot = primaryBooking.AssignedPilot;
                    passengerMessage = additionalBookings.Any() 
                        ? $"Awesome! Your {allBookings.Length} tandem flights on {bookingDateString} are confirmed. You will be contacted by {assignedPilot.Name} ({assignedPilot.PhoneNumber.AsPhoneNumber()}) to coordinate your flights" 
                        : $"Awesome! Your tandem flight on {bookingDateString} is confirmed. You will be contacted by {assignedPilot.Name} ({assignedPilot.PhoneNumber.AsPhoneNumber()}) to coordinate the details.";
                }
                else
                {
                    passengerMessage = additionalBookings.Any()
                        ? $"Awesome! We will try to find {allBookings.Length} pilots who can take you flying on {bookingDateString}. "
                        : $"Awesome! We will try to find a pilot who can take you flying on {bookingDateString}.";
                }

                if (!string.IsNullOrEmpty(primaryBooking.PassengerPhone))
                {
                    await _smsService.Send(primaryBooking.PassengerPhone, passengerMessage, primaryBooking);
                    _bookingService.AddEvent(primaryBooking, null,
                        $"Sent confirmation sms message to {primaryBooking.PassengerName} ({primaryBooking.PassengerPhone.AsPhoneNumber()})");
                }

                if (!string.IsNullOrEmpty(primaryBooking.PassengerEmail))
                {
                    var assignedPilotMessage = "";
                    if (primaryBooking.AssignedPilot != null)
                    {
                        var pilotName = primaryBooking.AssignedPilot.Name;
                        var pilotPhone = primaryBooking.AssignedPilot.PhoneNumber.AsPhoneNumber();
                        var pilotEmail = primaryBooking.AssignedPilot.Email;
                        assignedPilotMessage =
                            $@"
Your assigned instructor is {pilotName} ({pilotPhone}, {pilotEmail}), feel free to 
contact him about any questions you have regarding your flight.
";
                    }

                    var message = $@"Hi {primaryBooking.PassengerName},

Thank you for booking a tandem flight with {_contService.content.clubname} on {bookingDateString}. Your booking 
has been confirmed. One of our instructors will contact you to organize the details 
of when and where you will meet a few days in advance of your flight. 
{assignedPilotMessage}

Kind regards,
{_bookingCoordinatorSettings.Name}
Tandem Coordinator
{_contService.content.clubname}";

                    if (allBookings.Any(b => b.AssignedPilot == null))
                    {
                        message = $@"Hi {primaryBooking.PassengerName},

Thank you for booking a tandem flight with {_contService.content.clubname} on {bookingDateString}. We currently do not have
enough available pilots at the given date. We will try to organize pilots as soon as possible, but we might have to reschedule or
cancel if no pilots become available.

Kind regards,
{_bookingCoordinatorSettings.Name}
Tandem Coordinator
{_contService.content.clubname}";
                    }


                    await _mailService.Send(primaryBooking.PassengerEmail, $"Tandem flight on {bookingDateString}", message);

                    _bookingService.AddEvent(primaryBooking, null,
                        $"Sent confirmation mail to {primaryBooking.PassengerName} ({primaryBooking.PassengerEmail})");
                }
            }

            _context.SaveChanges();
        }

        public async Task SendMissingPilotMessage(string bookingDateString, Booking booking)
        {
            //send to booking coordinator
            var message = $"Please find a pilot on {bookingDateString}: {booking.PassengerName}, {booking.PassengerEmail}, {booking.PassengerPhone.AsPhoneNumber()}, {booking.Comment}";
            await _smsService.Send(_bookingCoordinatorSettings.PhoneNumber, message, booking);

            //passenger sms 
            if (!string.IsNullOrEmpty(booking.PassengerPhone))
            {
                var passengerMessage = $"We're working on finding an instructor for your flight on {bookingDateString}. You will be contacted with the name and number of the new instructor. If you have any questions, you can contact the tandem booking coordinator, {_bookingCoordinatorSettings.Name} on ({_bookingCoordinatorSettings.PhoneNumber.AsPhoneNumber()})";
                await _smsService.Send(booking.PassengerPhone, passengerMessage, booking);
            }

            //passenger email
            if (!string.IsNullOrEmpty(booking.PassengerEmail))
            {
                var mailMessage = $@"Hi {booking.PassengerName},

Your booking on {bookingDateString} has been updated. 

We're working on finding a new instructor for your flight on {bookingDateString}. 
You will be contacted with the name and number of the new instructor. If you 
have any questions, please contact the tandem booking coordinator on ({_contService.content.contact.phone} or {_contService.content.contact.email})

kind regards,
{_contService.content.contact.person}
Tandem Coordinator
{_contService.content.clubname}
";
                await _mailService.Send(booking.PassengerEmail, $"Tandem flight on {bookingDateString}", mailMessage);
            }


        }

        public async Task SendNewPilotMessage(string bookingDateString, Booking booking, bool notifyPassenger)
        {
            var bookingTimeStamp =
                booking.BookingDate.ToString("yyyyMMdd") + "T" + booking.TimeSlot.asTime().Replace(":", "") + "00/" +
                booking.BookingDate.ToString("yyyyMMdd") + "T" + (booking.TimeSlot + 1).asTime().Replace(":", "") + "00"
                ;

            //send message to new pilot
            var assignedPilot = booking.AssignedPilot;
            if (assignedPilot.SmsNotification)
            {
                var message = $"You have a new flight on {bookingDateString}: {booking.PassengerName}, {booking.PassengerEmail}, {booking.PassengerPhone.AsPhoneNumber()}, {booking.Comment}.";
                await SendPilotMessage(assignedPilot, message, booking);
            }
            if (assignedPilot.EmailNotification)
            {
                var subject = $"New flight on {bookingDateString}";

                var eventURL = $@"https://www.google.com/calendar/render?action=TEMPLATE&text=Tandem+{System.Web.HttpUtility.UrlEncode(booking.PassengerName)}&dates={bookingTimeStamp}&ctz=Europe/Oslo&details={System.Web.HttpUtility.UrlEncode(booking.PassengerName)}%0A{booking.PassengerPhone}%0A{booking.PassengerEmail}";

                var message = $@"Hi {assignedPilot.Name},

You have been assigned a flight:
Date:            {bookingDateString}. 
Passenger Name:  {booking.PassengerName},
Passenger Phone: {booking.PassengerPhone.AsPhoneNumber()},
Passenger Email: {booking.PassengerEmail ?? "not specified"}
Comments:
{booking.Comment}

Booking Url: http://{_contService.content.url}/BookingAdmin/Details/{booking.Id}

Add to google calender: {eventURL}


fly safe!
Tandem Coordinator
";
                await _mailService.Send(assignedPilot.Email, subject, message);
            }

            //send message to passenger
            if (notifyPassenger)
            {
                //sms
                if (!string.IsNullOrEmpty(booking.PassengerPhone))
                {
                    var passengerMessage = $"Your flight on {bookingDateString} has been assigned a new pilot. You will be contacted by {assignedPilot.Name} ({assignedPilot.PhoneNumber.AsPhoneNumber()}) shortly.";
                    await _smsService.Send(booking.PassengerPhone, passengerMessage, booking);
                }

                //email
                if (!string.IsNullOrEmpty(booking.PassengerEmail))
                {
                    var mailMessage = $@"Hi {booking.PassengerName},

Your booking on {bookingDateString} has been updated. You have been assigned a new instructor.

Your new instructor for {bookingDateString} is {assignedPilot.Name} ({assignedPilot.PhoneNumber.AsPhoneNumber()}, {assignedPilot.Email}). 

kind regards,
{_bookingCoordinatorSettings.Name}
Tandem Coordinator
{_contService.content.clubname}
";
                    await _mailService.Send(booking.PassengerEmail, $"Tandem flight on {bookingDateString}", mailMessage);
                }
            }
        }

        public async Task SendPilotUnassignedMessage(Booking booking, ApplicationUser previousPilot)
        {
            var bookingDateString = booking.BookingDate.ToString("dd.MM.yyyy") + " at " + booking.TimeSlot.asTime(); 

            if (previousPilot.SmsNotification)
            {
                var message = $"Your booking on {bookingDateString} has been reassigned to another pilot";
                await SendPilotMessage(previousPilot, message, booking);
            }
            if (previousPilot.EmailNotification)
            {
                var message =
                    $@"Hi {previousPilot.Name},

Your flight on {bookingDateString} has been assigned to another pilot.

Booking Url: http://{_contService.content.url}/BookingAdmin/Details/{booking.Id}

fly safe!
Tandem Coordinator
";
                await _mailService.Send(previousPilot.Email, $"Booking on {bookingDateString} reassigned", message);
            }
        }

        public async Task SendCancelMessage(string cancelMessage, Booking booking, ApplicationUser sender)
        {
            var bookingDate = $"{booking.BookingDate:dd.MM.yyyy}" + " at " + booking.TimeSlot.asTime(); ;
            if (!string.IsNullOrEmpty(booking.PassengerPhone))
            {
                var message = $"Unfortunately, your flight on {bookingDate} has been canceled due to {cancelMessage}";
                await _smsService.Send(booking.PassengerPhone, message, booking);
            }

            if (!string.IsNullOrEmpty(booking.PassengerEmail))
            {
                var senderText = $"{sender.Name} ({sender.PhoneNumber.AsPhoneNumber()}, {sender.Email})";
                var mailMessage = $@"Hi {booking.PassengerName},

Your booking on {bookingDate} has been updated. {senderText} has sent you a new message:

Unfortunately, your flight on {bookingDate} has been canceled due to {cancelMessage}

kind regards,
{_bookingCoordinatorSettings.Name}
Tandem Coordinator
{_contService.content.clubname}
";
                await _mailService.Send(booking.PassengerEmail, $"Tandem flight on {bookingDate}", mailMessage);
            }
        }

        public async Task SendBookingInformationMessage(SendMessageViewModel input, Booking booking, ApplicationUser sender)
        {
            if (input.SendToPassenger)
            {
                if (!string.IsNullOrEmpty(booking.PassengerPhone))
                {
                    await _smsService.Send(booking.PassengerPhone, input.EventMessage, booking);
                }

                if (!string.IsNullOrEmpty(booking.PassengerEmail))
                {
                    var senderText = $"{sender.Name} ({sender.PhoneNumber.AsPhoneNumber()}, {sender.Email})";
                    var bookingDate = $"{booking.BookingDate:dd.MM.yyyy}" + " at " + booking.TimeSlot.asTime(); 
                    var mailMessage = $@"Hi {booking.PassengerName},

Your booking on {bookingDate} has been updated. {senderText} has sent you a new message:

{input.EventMessage}

kind regards,
{_contService.content.contact.person}
Tandem Coordinator
{_contService.content.clubname}
";
                    await _mailService.Send(booking.PassengerEmail, $"Tandem flight on {bookingDate}", mailMessage);
                }
            }
        }

    }
}
