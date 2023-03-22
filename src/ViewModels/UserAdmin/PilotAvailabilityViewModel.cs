using System;
using System.Collections.Generic;
using TandemBooking.Models;

namespace TandemBooking.ViewModels
{
    public class PilotAvailabilityViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<PilotAvailability> Availabilities { get; set; }
        public List<BookedPilot> PilotBookings { get; set; }

        public DateTime Next { get; set; }
        public DateTime Prev { get; set; }
        public string MonthName { get; set; }
        public ApplicationUser Pilot { get; set; }
    }

    public class SetPilotAvailabilityViewModel
    {
        public string Date { get; set; }

        public string PilotID { get; set; }

        public int TimeSlot { get; set; }
        public bool Available { get; set; }
    }   
}

