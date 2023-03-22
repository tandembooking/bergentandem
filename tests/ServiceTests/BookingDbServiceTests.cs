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
    public class BookingDbServiceTests : IntegrationTestBase
    {
        public BookingDbServiceTests(IntegrationTestFixture fixture) : base(fixture)
        {
            _db = GetService<BookingServiceDb>();
            _pilots = new PilotsFixture(Context);
        }

        private readonly BookingServiceDb _db;
        private readonly PilotsFixture _pilots;

        [Fact]
        public async Task Availability()
        {
            Context.AddAvailabilityFixture(new DateTime(2016, 11, 1), 1, _pilots.Frode, _pilots.Erik);

            var avail = await _db.GetAvailablePilotsAsync(new DateTime(2016, 11, 1), 1);

            Assert.Equal(3, avail.Count); // only pilots
            Assert.Equal(2, avail.Count(x => x.Available)); // frode and erik are available
            Assert.Equal("Åsmund Birkeland", avail.Single(x => !x.Available).Pilot.Name); // åsmund is not available
            Assert.All(avail, x => Assert.Equal(0, x.Priority));
        }

        [Fact]
        public async Task PriorityWithBookings()
        {
            //future bookings
            Context.AddBookingFixture(new DateTime(2016, 11, 2), 1, _pilots.Frode);
            Context.AddBookingFixture(new DateTime(2016, 11, 3), 1, _pilots.Frode);
            Context.AddBookingFixture(new DateTime(2016, 11, 4), 1, _pilots.Frode);
            Context.AddBookingFixture(new DateTime(2016, 11, 4), 1, _pilots.Frode, 0);          //should not count (zero fee)
            Context.AddBookingFixture(new DateTime(2016, 11, 1).AddDays(20), 1, _pilots.Frode); //should not count

            //past bookings
            Context.AddBookingFixture(new DateTime(2016, 10, 29), 1, _pilots.Erik);
            Context.AddBookingFixture(new DateTime(2016, 11, 1).AddDays(-20), 1, _pilots.Erik);
            Context.AddBookingFixture(new DateTime(2016, 11, 1).AddDays(-40), 1, _pilots.Erik); //should not count

            //today
            Context.AddBookingFixture(new DateTime(2016, 11, 1), 1, _pilots.Aasmund);
            Context.AddBookingFixture(new DateTime(2016, 11, 2), 1, _pilots.Aasmund);
            Context.AddBookingFixture(new DateTime(2016, 10, 29), 1, _pilots.Aasmund);

            //Canceled booking should not count
            var b1 = Context.AddBookingFixture(new DateTime(2016, 10, 28), 1, _pilots.Aasmund);
            b1.Canceled = true;

            //Reassigned booking (should count on frode, but not Aasmund)
            var b2 = Context.AddBookingFixture(new DateTime(2016, 11, 5), 1, _pilots.Aasmund);
            b2.AssignedPilot = _pilots.Frode;
            b2.BookedPilots.Single().Canceled = true;
            b2.BookedPilots.Add(new BookedPilot {Pilot = _pilots.Frode});

            Context.SaveChanges();

            //act
            var avail = await _db.GetAvailablePilotsAsync(new DateTime(2016, 11, 1),1);

            //assert
            Assert.All(avail, x => Assert.Equal(false, x.Available));

            Assert.Equal(4, avail.Single(x => x.Pilot == _pilots.Frode).Priority);
            Assert.Equal(2, avail.Single(x => x.Pilot == _pilots.Erik).Priority);
            Assert.Equal(1002, avail.Single(x => x.Pilot == _pilots.Aasmund).Priority);
        }
    }
}