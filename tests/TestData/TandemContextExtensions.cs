using System;
using System.Collections.Generic;
using TandemBooking.Models;

namespace TandemBooking.Tests.TestData
{
    public static class TandemContextExtensions
    {
        public static void AddAvailabilityFixture(this TandemBookingContext context, DateTime date,int timeslot,
            params ApplicationUser[] pilots)
        {
            foreach (var pilot in pilots)
                pilot.Availabilities.Add(new PilotAvailability
                {
                    Date = date,
                    TimeSlot = timeslot
                    
                });
            context.SaveChanges();
        }

        public static Booking AddBookingFixture(this TandemBookingContext context, DateTime date, int timeslot, ApplicationUser pilot, int passengerFee=1700)
        {
            var booking = context.Bookings.Add(new Booking
            {
                AssignedPilot = pilot,
                DateRegistered = new DateTime(2016, 1, 1),
                BookingDate = date,
                TimeSlot = timeslot,
                Canceled = false,
                PassengerFee = passengerFee,
                BookedPilots = pilot != null 
                    ? new List<BookedPilot> {new BookedPilot {Pilot = pilot}} 
                    : new List<BookedPilot>()
            }).Entity;

            context.SaveChanges();

            return booking;
        }
    }
}