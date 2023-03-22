using System;
using System.Linq;
using System.Threading.Tasks;
using TandemBooking.Models;
using TandemBooking.Services;
using TandemBooking.Tests.TestData;
using TandemBooking.Tests.TestSetup;
using Xunit;

namespace TandemBooking.Tests.ServiceTests
{
    [Collection("Integration Tests")]
    public class BookingServiceTests : IntegrationTestBase
    {
        public BookingServiceTests(IntegrationTestFixture fixture) : base(fixture)
        {
            _bookingService = GetService<BookingService>();
            _pilots = new PilotsFixture(Context);
        }

        private readonly BookingService _bookingService;
        private readonly PilotsFixture _pilots;

        [Fact]
        public async Task AssignAvailablePilot()
        {
            Context.AddAvailabilityFixture(new DateTime(2016, 11, 1), 1,_pilots.Frode, _pilots.Erik);

            var booking = Context.AddBookingFixture(new DateTime(2016, 11, 1), 1, null);

            var pilot = await _bookingService.AssignNewPilotAsync(booking);
            Context.SaveChanges();

            //assigned pilot is one of available pilots
            Assert.True(pilot == _pilots.Frode || pilot == _pilots.Erik);

            //assigned pilot is both on AssignedPilot and in BookedPilots
            Assert.Equal(pilot, booking.AssignedPilot);
            Assert.Equal(1, booking.BookedPilots.Count);
            Assert.All(booking.BookedPilots, bookedPilot =>
            {
                Assert.Equal(pilot, bookedPilot.Pilot);
                Assert.False(bookedPilot.Canceled);
            });
        }

        [Fact]
        public async Task AssignPilotInWeightRange()
        {
            Context.AddAvailabilityFixture(new DateTime(2016, 11, 1), 1,_pilots.Frode, _pilots.Aasmund, _pilots.Erik);

            var booking = Context.AddBookingFixture(new DateTime(2016, 11, 1),1, null);
            booking.PassengerWeight = 130; //out of frodes weight range

            var pilot = await _bookingService.AssignNewPilotAsync(booking);
            Context.SaveChanges();

            //assigned pilot is the one available pilot in weight range
            Assert.True(pilot == _pilots.Aasmund);
        }

        [Fact]
        public async Task AssignPrioritizedPilot()
        {
            Context.AddAvailabilityFixture(new DateTime(2016, 11, 5), 1,_pilots.Frode, _pilots.Erik);
            Context.AddBookingFixture(new DateTime(2016, 11, 2), 1, _pilots.Frode);
            Context.AddBookingFixture(new DateTime(2016, 11, 3), 1, _pilots.Frode);
            Context.AddBookingFixture(new DateTime(2016, 11, 4), 1, _pilots.Erik);

            var booking = Context.AddBookingFixture(new DateTime(2016, 11, 5), 1, null);

            var pilot = await _bookingService.AssignNewPilotAsync(booking);
            Context.SaveChanges();

            //assigned pilot is pilot with least number of flights
            Assert.True(pilot == _pilots.Erik);
        }

        [Fact]
        public async Task AssignNextPilot()
        {
            Context.AddAvailabilityFixture(new DateTime(2016, 11, 1), 1, _pilots.Frode, _pilots.Erik);
            Context.AddBookingFixture(new DateTime(2016, 11, 2), 1, _pilots.Frode);
            Context.AddBookingFixture(new DateTime(2016, 11, 3), 1, _pilots.Frode);
            Context.AddBookingFixture(new DateTime(2016, 11, 4), 1, _pilots.Erik);

            var booking = Context.AddBookingFixture(new DateTime(2016, 11, 1), 1, null);

            var firstPilot = await _bookingService.AssignNewPilotAsync(booking);
            Context.SaveChanges();

            var secondPilot = await _bookingService.AssignNewPilotAsync(booking);
            Context.SaveChanges();

            // first assigned pilot is pilot with least number of flights
            Assert.Equal(_pilots.Erik, firstPilot);
            Assert.All(booking.BookedPilots.Where(x => x.Pilot == _pilots.Erik), canceledBookedPilot =>
            {
                // pilot should be canceled
                Assert.True(canceledBookedPilot.Canceled);
            });

            // second pilot should be other available pilot
            Assert.Equal(_pilots.Frode, secondPilot);
            Assert.All(booking.BookedPilots.Where(x => x.Pilot == _pilots.Frode), activeBookedPilot =>
            {
                // pilot should be active
                Assert.False(activeBookedPilot.Canceled);
            });
        }


        [Fact]
        public async Task AssignNextPilotUnavailable()
        {
            Context.AddAvailabilityFixture(new DateTime(2016, 11, 1), 1, _pilots.Frode);

            var booking = Context.AddBookingFixture(new DateTime(2016, 11, 1), 1, null);

            var firstPilot = await _bookingService.AssignNewPilotAsync(booking);
            Context.SaveChanges();

            var secondPilot = await _bookingService.AssignNewPilotAsync(booking);
            Context.SaveChanges();

            // first assigned available pilot
            Assert.Equal(_pilots.Frode, firstPilot);

            // then set pilot to null
            Assert.Null(secondPilot);
            Assert.Null(booking.AssignedPilot);
            Assert.Equal(1, booking.BookedPilots.Count);
            Assert.All(booking.BookedPilots, x =>
            {
                Assert.True(x.Canceled);
            });

            // flight should not be canceled, though
            Assert.False(booking.Canceled);
        }

        [Fact]
        public async Task AssignMultipleBookings()
        {
            //setup
            Context.AddAvailabilityFixture(new DateTime(2016, 11, 1), 1, _pilots.Frode);
            Context.AddAvailabilityFixture(new DateTime(2016, 11, 1), 1, _pilots.Erik);
            Context.AddAvailabilityFixture(new DateTime(2016, 11, 1), 1, _pilots.Aasmund);

            var BookingFrode1 = Context.AddBookingFixture(new DateTime(2016, 10, 30), 1, _pilots.Frode);
            var bookingFrode2 = Context.AddBookingFixture(new DateTime(2016, 10, 30), 2, _pilots.Frode);
            var bookingFrode3 = Context.AddBookingFixture(new DateTime(2016, 10, 30), 3, _pilots.Frode);

            Context.AddBookingFixture(new DateTime(2016, 10, 30), 1, _pilots.Erik);
            Context.AddBookingFixture(new DateTime(2016, 10, 30), 2, _pilots.Erik);

            Context.AddBookingFixture(new DateTime(2016, 10, 30), 1, _pilots.Aasmund);

            var booking1 = Context.AddBookingFixture(new DateTime(2016, 11, 1), 1, null);
            var booking2 = Context.AddBookingFixture(new DateTime(2016, 11, 1), 1, null);
            var booking3 = Context.AddBookingFixture(new DateTime(2016, 11, 1), 1, null);
            var bookings = new[] {booking1, booking2, booking3}.ToList();

            //act
            var pilots = await _bookingService.AssignNewPilotAsync(bookings);

            //verify
            Assert.Equal(3, pilots.Count);
            Assert.Equal(_pilots.Aasmund, pilots[0]);
            Assert.Equal(_pilots.Erik, pilots[1]);
            Assert.Equal(_pilots.Frode, pilots[2]);
        }
       


    }
}