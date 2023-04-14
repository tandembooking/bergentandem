using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TandemBooking.Controllers;
using TandemBooking.Tests.TestData;
using TandemBooking.Tests.TestSetup;
using TandemBooking.ViewModels;
using TandemBooking.ViewModels.Account;
using Xunit;

namespace TandemBooking.Tests.ControllerTests
{
    [Collection("Pilot Admin Tests")]
    public class PilotAdminTests : IntegrationTestBase
    {
        public PilotAdminTests(IntegrationTestFixture fixture) : base(fixture)
        {
           
            _pilots = new PilotsFixture(Context);
        }

        private readonly PilotsFixture _pilots;


        [Fact]
        public async Task AddAvailabilityTest()
        {
            var loginInfo = new LoginViewModel
            {
                Email = "nils.jordheim@gmail.com",
                Password = "123456",
                RememberMe = false

            };


            var ctrlLogin = GetService<AccountController>();
            var resultLogin = await ctrlLogin.Login(loginInfo);

            var input = new PilotAvailabilityViewModel
            {
                StartDate = new DateTime(2016, 10, 1),
                EndDate = new DateTime(2016, 10, 31),
                Pilot = _pilots.Frode,
                Next = new DateTime(2016, 11, 1),
                Prev = new DateTime(2016, 9, 30),
                MonthName = "October"

            };

            SetPilotAvailabilityViewModel[] inputAvail = new SetPilotAvailabilityViewModel[1];
            var avail1 = new SetPilotAvailabilityViewModel
            {
                Date = new DateTime(2016, 10, 13).ToString(),
                PilotID = _pilots.Frode.Id,
                TimeSlot = 1,
                Available = true
            };
            inputAvail[0] = avail1;
            var ctrl = GetService<PilotAvailabilityController>();
            await ctrl.SetAvailability(inputAvail);
            await ctrl.SetAvailability(inputAvail);

            var result = ctrl.Index(new DateTime(2016, 10, 13), _pilots.Frode.Id) as ViewResult;
            var availibility = (PilotAvailabilityViewModel)result.ViewData.Model;

            //Assert booking is created
            Assert.Single(availibility.Availabilities);

        }

    }
}