using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tandembooking.Migrations
{
    public partial class Booking_PrimaryBooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryBookingId",
                table: "Bookings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PrimaryBookingId",
                table: "Bookings",
                column: "PrimaryBookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Bookings_PrimaryBookingId",
                table: "Bookings",
                column: "PrimaryBookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Bookings_PrimaryBookingId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PrimaryBookingId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PrimaryBookingId",
                table: "Bookings");
        }
    }
}
