using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TandemBooking.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TandemBooking.Models
{
    public class TandemBookingContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Booking> Bookings { get; set; } 
        public DbSet<PilotAvailability> PilotAvailabilities { get; set; } 
        public DbSet<BookedPilot> BookedPilots { get; set; }
        public DbSet<SentSmsMessage> SentSmsMessages { get; set; }
        public DbSet<SentSmsMessagePart> SentSmsMessageParts { get; set; }


        public TandemBookingContext(DbContextOptions<TandemBookingContext> options) : base(options)
        {
        }
        public TandemBookingContext() : base(
            new DbContextOptionsBuilder<TandemBookingContext>()
                .UseSqlServer("server=.;database=tandembooking;integrated security=true")
                .Options
        )
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var userBuilder = builder.Entity<ApplicationUser>();
            userBuilder.Property(p => p.EmailNotification).HasDefaultValue(true);
            userBuilder.Property(p => p.SmsNotification).HasDefaultValue(true);

            var bookingBuilder = builder.Entity<Booking>();
            bookingBuilder.Property(t => t.AssignedPilotId)
                .HasMaxLength(450);
            bookingBuilder.HasOne(t => t.PrimaryBooking)
                .WithMany(u => u.AdditionalBookings);

            var availabilityBuilder = builder.Entity<PilotAvailability>();
            availabilityBuilder.Property(ab => ab.PilotId)
                .HasMaxLength(450);

            var bookingEventBuilder = builder.Entity<BookingEvent>();
            bookingEventBuilder.Property(t => t.UserId)
                .HasMaxLength(450);

            var bookedPilotBuilder = builder.Entity<BookedPilot>();
            bookedPilotBuilder.Property(t => t.PilotId)
                .HasMaxLength(450);

            builder.Entity<SentSmsMessage>();
            var sentSmsMessagePartBuilder = builder.Entity<SentSmsMessagePart>();
            sentSmsMessagePartBuilder.Property(t => t.GatewayMessageId)
                .HasMaxLength(255);
            sentSmsMessagePartBuilder.HasIndex(t => t.GatewayMessageId);
        }
    }
}
