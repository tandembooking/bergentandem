using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TandemBooking.Models;

namespace tandembooking.Migrations
{
    [DbContext(typeof(TandemBookingContext))]
    [Migration("20160507174114_UserNotificationPreferences")]
    partial class UserNotificationPreferences
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
            {
                b.Property<string>("Id");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken();

                b.Property<string>("Name")
                    .HasAnnotation("MaxLength", 256);

                b.Property<string>("NormalizedName")
                    .HasAnnotation("MaxLength", 256);

                b.HasKey("Id");

                b.HasIndex("NormalizedName")
                    .HasDatabaseName("RoleNameIndex");

                b.ToTable("AspNetRoles");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<string>("ClaimType");

                b.Property<string>("ClaimValue");

                b.Property<string>("RoleId")
                    .IsRequired();

                b.HasKey("Id");

                b.HasIndex("RoleId");

                b.ToTable("AspNetRoleClaims");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<string>("ClaimType");

                b.Property<string>("ClaimValue");

                b.Property<string>("UserId")
                    .IsRequired();

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.ToTable("AspNetUserClaims");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
            {
                b.Property<string>("LoginProvider");

                b.Property<string>("ProviderKey");

                b.Property<string>("ProviderDisplayName");

                b.Property<string>("UserId")
                    .IsRequired();

                b.HasKey("LoginProvider", "ProviderKey");

                b.HasIndex("UserId");

                b.ToTable("AspNetUserLogins");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
            {
                b.Property<string>("UserId");

                b.Property<string>("RoleId");

                b.HasKey("UserId", "RoleId");

                b.HasIndex("RoleId");

                b.HasIndex("UserId");

                b.ToTable("AspNetUserRoles");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
            {
                b.Property<string>("UserId");

                b.Property<string>("LoginProvider");

                b.Property<string>("Name");

                b.Property<string>("Value");

                b.HasKey("UserId", "LoginProvider", "Name");

                b.ToTable("AspNetUserTokens");
            });

            modelBuilder.Entity("TandemBooking.Models.ApplicationUser", b =>
            {
                b.Property<string>("Id");

                b.Property<int>("AccessFailedCount");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken();

                b.Property<string>("Email")
                    .HasAnnotation("MaxLength", 256);

                b.Property<bool>("EmailConfirmed");

                b.Property<bool>("EmailNotification")
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(true);

                b.Property<bool>("IsAdmin");

                b.Property<bool>("IsPilot");

                b.Property<bool>("LockoutEnabled");

                b.Property<DateTimeOffset?>("LockoutEnd");

                b.Property<string>("Name");

                b.Property<string>("NormalizedEmail")
                    .HasAnnotation("MaxLength", 256);

                b.Property<string>("NormalizedUserName")
                    .HasAnnotation("MaxLength", 256);

                b.Property<string>("PasswordHash");

                b.Property<string>("PhoneNumber");

                b.Property<bool>("PhoneNumberConfirmed");

                b.Property<string>("SecurityStamp");

                b.Property<bool>("SmsNotification")
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(true);

                b.Property<bool>("TwoFactorEnabled");

                b.Property<string>("UserName")
                    .HasAnnotation("MaxLength", 256);

                b.HasKey("Id");

                b.HasIndex("NormalizedEmail")
                    .HasDatabaseName("EmailIndex");

                b.HasIndex("NormalizedUserName")
                    .IsUnique()
                    .HasDatabaseName("UserNameIndex");

                b.ToTable("AspNetUsers");
            });

            modelBuilder.Entity("TandemBooking.Models.BookedPilot", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<DateTime>("AssignedDate");

                b.Property<Guid?>("BookingId");

                b.Property<bool>("Canceled");

                b.Property<bool>("Confirmed");

                b.Property<string>("PilotId")
                    .HasMaxLength(450);

                b.HasKey("Id");

                b.HasIndex("BookingId");

                b.HasIndex("PilotId");

                b.ToTable("BookedPilots");
            });

            modelBuilder.Entity("TandemBooking.Models.Booking", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<string>("AssignedPilotId");

                b.Property<DateTime>("BookingDate");

                b.Property<bool>("Canceled");

                b.Property<string>("Comment");

                b.Property<DateTime>("DateRegistered");

                b.Property<string>("PassengerEmail");

                b.Property<string>("PassengerName");

                b.Property<string>("PassengerPhone");

                b.HasKey("Id");

                b.HasIndex("AssignedPilotId");

                b.ToTable("Bookings");
            });

            modelBuilder.Entity("TandemBooking.Models.BookingEvent", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<Guid?>("BookingId");

                b.Property<DateTime>("EventDate");

                b.Property<string>("EventMessage");

                b.Property<string>("UserId");

                b.HasKey("Id");

                b.HasIndex("BookingId");

                b.HasIndex("UserId");

                b.ToTable("BookingEvent");
            });

            modelBuilder.Entity("TandemBooking.Models.PilotAvailability", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<DateTime>("Date");

                b.Property<string>("PilotId")
                    .HasMaxLength(450);

                b.HasKey("Id");

                b.HasIndex("PilotId");

                b.ToTable("PilotAvailabilities");
            });

            modelBuilder.Entity("TandemBooking.Models.SentSmsMessage", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<Guid?>("BookingId");

                b.Property<DateTime>("LastRetryDate");

                b.Property<string>("MessageText");

                b.Property<DateTime>("NextRetryDate");

                b.Property<string>("RecipientNumber");

                b.Property<int>("RetryCount");

                b.Property<DateTime>("SentDate");

                b.HasKey("Id");

                b.HasIndex("BookingId");

                b.ToTable("SentSmsMessages");
            });

            modelBuilder.Entity("TandemBooking.Models.SentSmsMessagePart", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<DateTime?>("DeliveryReportDate");

                b.Property<string>("DeliveryReportErrorCode");

                b.Property<string>("DeliveryReportStatus");

                b.Property<string>("GatewayMessageId")
                    .HasAnnotation("MaxLength", 255);

                b.Property<Guid?>("MessageId");

                b.Property<string>("StatusCode");

                b.Property<string>("StatusText");

                b.HasKey("Id");

                b.HasIndex("GatewayMessageId");

                b.HasIndex("MessageId");

                b.ToTable("SentSmsMessageParts");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
            {
                b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                    .WithMany("Claims")
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
            {
                b.HasOne("TandemBooking.Models.ApplicationUser")
                    .WithMany("Claims")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
            {
                b.HasOne("TandemBooking.Models.ApplicationUser")
                    .WithMany("Logins")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
            {
                b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                    .WithMany("Users")
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("TandemBooking.Models.ApplicationUser")
                    .WithMany("Roles")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("TandemBooking.Models.BookedPilot", b =>
            {
                b.HasOne("TandemBooking.Models.Booking", "Booking")
                    .WithMany("BookedPilots")
                    .HasForeignKey("BookingId");

                b.HasOne("TandemBooking.Models.ApplicationUser", "Pilot")
                    .WithMany("Bookings")
                    .HasForeignKey("PilotId");
            });

            modelBuilder.Entity("TandemBooking.Models.Booking", b =>
            {
                b.HasOne("TandemBooking.Models.ApplicationUser", "AssignedPilot")
                    .WithMany()
                    .HasForeignKey("AssignedPilotId");
            });

            modelBuilder.Entity("TandemBooking.Models.BookingEvent", b =>
            {
                b.HasOne("TandemBooking.Models.Booking", "Booking")
                    .WithMany("BookingEvents")
                    .HasForeignKey("BookingId");

                b.HasOne("TandemBooking.Models.ApplicationUser", "User")
                    .WithMany()
                    .HasForeignKey("UserId");
            });

            modelBuilder.Entity("TandemBooking.Models.PilotAvailability", b =>
            {
                b.HasOne("TandemBooking.Models.ApplicationUser", "Pilot")
                    .WithMany("Availabilities")
                    .HasForeignKey("PilotId");
            });

            modelBuilder.Entity("TandemBooking.Models.SentSmsMessage", b =>
            {
                b.HasOne("TandemBooking.Models.Booking", "Booking")
                    .WithMany()
                    .HasForeignKey("BookingId");
            });

            modelBuilder.Entity("TandemBooking.Models.SentSmsMessagePart", b =>
            {
                b.HasOne("TandemBooking.Models.SentSmsMessage", "Message")
                    .WithMany("SmsMessageParts")
                    .HasForeignKey("MessageId");
            });
        }
    }
}
