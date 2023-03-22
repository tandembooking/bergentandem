using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tandembooking.Migrations
{
    public partial class FixedNamesForRTM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedPilot_Booking_BookingId",
                table: "BookedPilot");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedPilot_ApplicationUser_PilotId",
                table: "BookedPilot");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_ApplicationUser_AssignedPilotId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingEvent_Booking_BookingId",
                table: "BookingEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingEvent_ApplicationUser_UserId",
                table: "BookingEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_PilotAvailability_ApplicationUser_PilotId",
                table: "PilotAvailability");

            migrationBuilder.DropForeignKey(
                name: "FK_SentSmsMessage_Booking_BookingId",
                table: "SentSmsMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_SentSmsMessagePart_SentSmsMessage_MessageId",
                table: "SentSmsMessagePart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SentSmsMessagePart",
                table: "SentSmsMessagePart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SentSmsMessage",
                table: "SentSmsMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PilotAvailability",
                table: "PilotAvailability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Booking",
                table: "Booking");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookedPilot",
                table: "BookedPilot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUser",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityUserRole<string>",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityUserLogin<string>",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityUserClaim<string>",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityRoleClaim<string>",
                table: "AspNetRoleClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityRole",
                table: "AspNetRoles");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SentSmsMessageParts",
                table: "SentSmsMessagePart",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SentSmsMessageParts_GatewayMessageId",
                table: "SentSmsMessagePart",
                column: "GatewayMessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SentSmsMessages",
                table: "SentSmsMessage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PilotAvailabilities",
                table: "PilotAvailability",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookings",
                table: "Booking",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookedPilots",
                table: "BookedPilot",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookedPilots_Bookings_BookingId",
                table: "BookedPilot",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookedPilots_AspNetUsers_PilotId",
                table: "BookedPilot",
                column: "PilotId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_AssignedPilotId",
                table: "Booking",
                column: "AssignedPilotId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingEvent_Bookings_BookingId",
                table: "BookingEvent",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingEvent_AspNetUsers_UserId",
                table: "BookingEvent",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PilotAvailabilities_AspNetUsers_PilotId",
                table: "PilotAvailability",
                column: "PilotId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SentSmsMessages_Bookings_BookingId",
                table: "SentSmsMessage",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SentSmsMessageParts_SentSmsMessages_MessageId",
                table: "SentSmsMessagePart",
                column: "MessageId",
                principalTable: "SentSmsMessage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.CreateIndex(
                column: "MessageId",
                table: "SentSmsMessagePart",
                name: "IX_SentSmsMessageParts_MessageId");

            migrationBuilder.CreateIndex(
                column: "BookingId",
                table: "SentSmsMessage",
                name: "IX_SentSmsMessages_BookingId");

            migrationBuilder.CreateIndex(
                column: "PilotId",
                table: "PilotAvailability",
                name: "IX_PilotAvailabilities_PilotId");

            migrationBuilder.CreateIndex(
                column: "AssignedPilotId",
                table: "Booking",
                name: "IX_Bookings_AssignedPilotId");

            migrationBuilder.CreateIndex(
                column: "PilotId",
                table: "BookedPilot",
                name: "IX_BookedPilots_PilotId");

            migrationBuilder.CreateIndex(
                column: "BookingId",
                table: "BookedPilot",
                name: "IX_BookedPilots_BookingId");

            migrationBuilder.RenameTable(
                name: "SentSmsMessagePart",
                newName: "SentSmsMessageParts");

            migrationBuilder.RenameTable(
                name: "SentSmsMessage",
                newName: "SentSmsMessages");

            migrationBuilder.RenameTable(
                name: "PilotAvailability",
                newName: "PilotAvailabilities");

            migrationBuilder.RenameTable(
                name: "Booking",
                newName: "Bookings");

            migrationBuilder.RenameTable(
                name: "BookedPilot",
                newName: "BookedPilots");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedPilots_Bookings_BookingId",
                table: "BookedPilots");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedPilots_AspNetUsers_PilotId",
                table: "BookedPilots");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_AssignedPilotId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingEvent_Bookings_BookingId",
                table: "BookingEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingEvent_AspNetUsers_UserId",
                table: "BookingEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_PilotAvailabilities_AspNetUsers_PilotId",
                table: "PilotAvailabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_SentSmsMessages_Bookings_BookingId",
                table: "SentSmsMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_SentSmsMessageParts_SentSmsMessages_MessageId",
                table: "SentSmsMessageParts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SentSmsMessageParts",
                table: "SentSmsMessageParts");

            migrationBuilder.DropIndex(
                name: "IX_SentSmsMessageParts_GatewayMessageId",
                table: "SentSmsMessageParts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SentSmsMessages",
                table: "SentSmsMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PilotAvailabilities",
                table: "PilotAvailabilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookedPilots",
                table: "BookedPilots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SentSmsMessagePart",
                table: "SentSmsMessageParts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SentSmsMessage",
                table: "SentSmsMessages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PilotAvailability",
                table: "PilotAvailabilities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Booking",
                table: "Bookings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookedPilot",
                table: "BookedPilots",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUser",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "UserName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityUserRole<string>",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityUserLogin<string>",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityUserClaim<string>",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityRoleClaim<string>",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityRole",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookedPilot_Booking_BookingId",
                table: "BookedPilots",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookedPilot_ApplicationUser_PilotId",
                table: "BookedPilots",
                column: "PilotId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_ApplicationUser_AssignedPilotId",
                table: "Bookings",
                column: "AssignedPilotId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingEvent_Booking_BookingId",
                table: "BookingEvent",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingEvent_ApplicationUser_UserId",
                table: "BookingEvent",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PilotAvailability_ApplicationUser_PilotId",
                table: "PilotAvailabilities",
                column: "PilotId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SentSmsMessage_Booking_BookingId",
                table: "SentSmsMessages",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SentSmsMessagePart_SentSmsMessage_MessageId",
                table: "SentSmsMessageParts",
                column: "MessageId",
                principalTable: "SentSmsMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.RenameIndex(
                name: "IX_SentSmsMessageParts_MessageId",
                table: "SentSmsMessageParts",
                newName: "IX_SentSmsMessagePart_MessageId");

            migrationBuilder.RenameIndex(
                name: "IX_SentSmsMessages_BookingId",
                table: "SentSmsMessages",
                newName: "IX_SentSmsMessage_BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_PilotAvailabilities_PilotId",
                table: "PilotAvailabilities",
                newName: "IX_PilotAvailability_PilotId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_AssignedPilotId",
                table: "Bookings",
                newName: "IX_Booking_AssignedPilotId");

            migrationBuilder.RenameIndex(
                name: "IX_BookedPilots_PilotId",
                table: "BookedPilots",
                newName: "IX_BookedPilot_PilotId");

            migrationBuilder.RenameIndex(
                name: "IX_BookedPilots_BookingId",
                table: "BookedPilots",
                newName: "IX_BookedPilot_BookingId");

            migrationBuilder.RenameTable(
                name: "SentSmsMessageParts",
                newName: "SentSmsMessagePart");

            migrationBuilder.RenameTable(
                name: "SentSmsMessages",
                newName: "SentSmsMessage");

            migrationBuilder.RenameTable(
                name: "PilotAvailabilities",
                newName: "PilotAvailability");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "Booking");

            migrationBuilder.RenameTable(
                name: "BookedPilots",
                newName: "BookedPilot");
        }
    }
}
