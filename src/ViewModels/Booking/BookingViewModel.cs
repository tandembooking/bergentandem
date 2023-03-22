using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace TandemBooking.ViewModels.Booking
{
    public class AdditionalPassengerViewModel
    {
        [Required(ErrorMessage = "Please enter the passengers name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter the passengers weight")]
        public int? Weight { get; set; }
    }

    public class BookingViewModel
    {
        public int Stage { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date")]
        public DateTime? Date { get; set; }

        public int? TimeSlot { get; set; }
        public AdditionalPassengerViewModel[] AdditionalPassengers { get; set; }
        public string Name { get; set; }

        [Phone(ErrorMessage = "We need a phone number to contact you about your flight. Please enter valid phone number.")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Please enter a valid number of passengers or 1 if you're the only one flying")]   
        //public int Passengers { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        public string Action { get; set; }
        public DateTime? NextDate { get; set; }
        public DateTime? PrevDate { get; set; }

        public BookingCalendarViewModel Calender { get; set; }

       
    }
}