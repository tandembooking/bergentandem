using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TandemBooking.Models;

namespace tandembooking.Migrations
{
    [DbContext(typeof(TandemBookingContext))]
    [Migration("20160506205254_AddedSentSmsMessage")]
    partial class AddedSentSmsMessage
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFramework.IdentityRole", b =>
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
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "AspNetUserRoles");
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

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
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
                });

            modelBuilder.Entity("TandemBooking.Models.PilotAvailability", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("PilotId")
                        .HasMaxLength(450);

                    b.HasKey("Id");
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
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("TandemBooking.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("TandemBooking.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("TandemBooking.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TandemBooking.Models.BookedPilot", b =>
                {
                    b.HasOne("TandemBooking.Models.Booking")
                        .WithMany()
                        .HasForeignKey("BookingId");

                    b.HasOne("TandemBooking.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("PilotId");
                });

            modelBuilder.Entity("TandemBooking.Models.Booking", b =>
                {
                    b.HasOne("TandemBooking.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("AssignedPilotId");
                });

            modelBuilder.Entity("TandemBooking.Models.BookingEvent", b =>
                {
                    b.HasOne("TandemBooking.Models.Booking")
                        .WithMany()
                        .HasForeignKey("BookingId");

                    b.HasOne("TandemBooking.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TandemBooking.Models.PilotAvailability", b =>
                {
                    b.HasOne("TandemBooking.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("PilotId");
                });

            modelBuilder.Entity("TandemBooking.Models.SentSmsMessage", b =>
                {
                    b.HasOne("TandemBooking.Models.Booking")
                        .WithMany()
                        .HasForeignKey("BookingId");
                });

            modelBuilder.Entity("TandemBooking.Models.SentSmsMessagePart", b =>
                {
                    b.HasOne("TandemBooking.Models.SentSmsMessage")
                        .WithMany()
                        .HasForeignKey("MessageId");
                });
        }
    }
}
