using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace tandembooking.Migrations
{
    public partial class added_timeslot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeSlot",
                table: "PilotAvailabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeSlot",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeSlot",
                table: "BookedPilots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeSlot",
                table: "PilotAvailabilities");

            migrationBuilder.DropColumn(
                name: "TimeSlot",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TimeSlot",
                table: "BookedPilots");
        }
    }
}
