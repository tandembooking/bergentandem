using System;

namespace TandemBooking.Models
{
    public class PilotAvailability {
        public Guid Id {get;set;}
        public DateTime Date {get; set;}

        public string PilotId { get; set; }
        public ApplicationUser Pilot {get;set;}

        public int TimeSlot { get; set; }
    }
}