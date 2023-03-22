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
    [Collection("TimeSlot Tests")]
    public class BookingServiceTimeSlotTests : IntegrationTestBase
    {
        public BookingServiceTimeSlotTests(IntegrationTestFixture fixture) : base(fixture)
        {
            _bookingService = GetService<BookingService>();
            _pilots = new PilotsFixture(Context);
        }

        private readonly BookingService _bookingService;
        private readonly PilotsFixture _pilots;


        [Fact]
        public async Task AssignBookingToBookedTimeslot()
        {
            //setup
            Context.AddAvailabilityFixture(new DateTime(2016, 11, 1), 1, _pilots.Aasmund);

            Context.AddBookingFixture(new DateTime(2016, 11, 1), 1, _pilots.Aasmund);
            
            var booking1 = Context.AddBookingFixture(new DateTime(2016, 11, 1), 1, null);
            //act
            var pilot = await _bookingService.AssignNewPilotAsync(booking1);

            //verify
            Assert.Equal(null, pilot);
        }
        
    }
}