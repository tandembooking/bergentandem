using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TandemBooking.Models;
using TandemBooking.Services;

namespace TandemBooking.ViewModels.BookingAdmin
{
    public class CreateBookingViewModel
    {
        [Display(Name = "Booking Date")]
        [Required(ErrorMessage = "Please select a date")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date")]
        public DateTime? Date { get; set; }

        public int TimeSlot { get; set; }

        [Display(Name = "Pilot")]
        public string PilotId { get; set; }

        [Display(Name = "Passenger Name")]
        [Required(ErrorMessage = "Please enter the passenger name")]
        public string Name { get; set; }

        [Display(Name = "Passenger Weight")]
        public int? Weight { get; set; }

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Display(Name = "Passenger Fee", Description ="The fee the passenger pays for the flight")]
        public int PassengerFee { get; set; }
       
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [Display(Name = "Send notification to passenger")]
        public bool NotifyPassenger { get; set; }

        [Display(Name = "Send notification to pilot")]
        public bool NotifyPilot { get; set; }

        public Guid? PrimaryBookingId { get; set; }
    }


    public class PilotSelectorViewModel
    {
        public string ControlName { get; set; }
        public List<AvailablePilot> AvailablePilots { get; set; }
    }
}
