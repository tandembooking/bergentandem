using System;

namespace TandemBooking.Models
{
    public class BookedPilot
    {
        public Guid Id { get; set; }
        public Booking Booking { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool Confirmed { get; set; }
        public bool Canceled { get; set; }

        public string PilotId { get; set; }
        public ApplicationUser Pilot { get; set; }
    }
}