using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tandembooking.Migrations
{
    public partial class PassengerWeight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PassengerWeight",
                table: "Bookings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxPassengerWeight",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinPassengerWeight",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassengerWeight",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "MaxPassengerWeight",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MinPassengerWeight",
                table: "AspNetUsers");
        }
    }
}
