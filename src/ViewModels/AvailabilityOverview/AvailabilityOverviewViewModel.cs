using System;
using System.Collections.Generic;
using TandemBooking.Models;

namespace TandemBooking.ViewModels.AvailabilityOverview
{
    public class AvailabilityOverviewViewModel
    {
        public DateTime Next { get; set; }
        public DateTime Prev { get; set; }
        public string MonthName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<AvailabilityOverviewDayViewModel> Days { get; set; }
    }

    public class AvailabilityOverviewDayViewModel
    {
        public DateTime Date { get; set; }
       
        public List<AvailabilityOverviewTimeSlotViewModel> TimeSlots { get; set; }
        public bool InPast { get; set; }
        
    }

    public class AvailabilityOverviewTimeSlotViewModel
    {
        public int TimeSlot { get; set; }
        public List<PilotAvailability> Availabilities { get; set; }
        public List<BookedPilot> PilotBookings { get; set; }
        public List<PilotAvailability> FreePilots { get; set; }

        public List<Models.Booking> UnassignedBookings { get; set; }
    }
}