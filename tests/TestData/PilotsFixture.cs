using System.Collections.Generic;
using TandemBooking.Models;

namespace TandemBooking.Tests.TestData
{
    public class PilotsFixture
    {
        private readonly TandemBookingContext _context;

        public PilotsFixture(TandemBookingContext context)
        {
            _context = context;
            AddPilotsFixture();
        }

        public ApplicationUser Frode { get; set; }
        public ApplicationUser Erik { get; private set; }
        public ApplicationUser Aasmund { get; private set; }
        public ApplicationUser Tore { get; private set; }

        private void AddPilotsFixture()
        {
            Frode = _context.Users.Add(new ApplicationUser
            {
                Name = "Frode Fester",
                Email = "frode@mail.com",
                PhoneNumber = "4794279974",
                IsAdmin = false,
                IsPilot = true,
                Availabilities = new List<PilotAvailability>(),
                MaxPassengerWeight = 100,
            }).Entity;
            _context.SaveChanges();

            Erik = _context.Users.Add(new ApplicationUser
            {
                Name = "Erik Røthe Klette",
                Email = "heimabrygg.og.smalahove@gmail.com",
                PhoneNumber = "47999694616",
                IsAdmin = false,
                IsPilot = true,
                Availabilities = new List<PilotAvailability>(),
                MinPassengerWeight = 60,
                MaxPassengerWeight = 120,
            }).Entity;
            _context.SaveChanges();

            Aasmund = _context.Users.Add(new ApplicationUser
            {
                Name = "Åsmund Birkeland",
                Email = "aasmund.birkeland@gmail.com",
                PhoneNumber = "4792819387",
                IsAdmin = false,
                IsPilot = true,
                Availabilities = new List<PilotAvailability>(),
                MinPassengerWeight = 40,
            }).Entity;

            Tore = _context.Users.Add(new ApplicationUser
            {
                Name = "Tore Birkeland",
                Email = "tore.birkeland@gmail.com",
                PhoneNumber = "4798463072",
                IsAdmin = true,
                IsPilot = false,
                Availabilities = new List<PilotAvailability>()
            }).Entity;

            _context.SaveChanges();
        }
    }
}