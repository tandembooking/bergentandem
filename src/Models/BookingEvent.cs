using System;

namespace TandemBooking.Models {
    public class BookingEvent
    {
        public Guid Id { get; set; }
        public Booking Booking { get; set; }
        public DateTime EventDate { get; set; }
        public string EventMessage { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }

}