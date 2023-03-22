using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TandemBooking.Controllers;
using TandemBooking.Models;

namespace TandemBooking.Services
{
    public class BookingServiceDb
    {
        private readonly TandemBookingContext _context;
        private readonly UserManager _userManager;

        public BookingServiceDb(TandemBookingContext context, UserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<AvailablePilot>> GetAvailablePilotsAsync(DateTime date, int timeslot)
        {
            var availablePilots = new List<AvailablePilot>();
            var conn = (SqlConnection) _context.Database.GetDbConnection();
            await _context.Database.OpenConnectionAsync();

            var sql = @"
                DECLARE @Date date = CONVERT(date, @DateParam)
                
                SELECT Pilots.Id PilotId, COALESCE(AvailabilityCount, 0) AvailabilityCount, COALESCE(Bookings.BookingCount, 0) BookingCount, COALESCE(BookingsToday.BookingCount, 0) BookingCountToday, COALESCE(BookingsTimeslot.BookingCount, 0) BookingCountTimeslot
	                FROM AspNetUsers Pilots
	                LEFT OUTER JOIN (
		                SELECT PilotId, COUNT(Id) AvailabilityCount FROM PilotAvailabilities
		                WHERE
			                CONVERT(date, PilotAvailabilities.[Date]) = @Date
                            AND PilotAvailabilities.[TimeSlot] = @timeslot
		                GROUP BY PilotId 
	                ) Availabilities ON Pilots.Id = Availabilities.PilotId
	                LEFT OUTER JOIN (
		                SELECT PilotId, COUNT(BookedPilots.Id) BookingCount
			                FROM BookedPilots 
			                INNER JOIN Bookings ON BookedPilots.BookingId = Bookings.Id
			                WHERE
                                COALESCE(PassengerFee, 0) > 0
				                AND BookedPilots.Canceled = 0
				                AND Bookings.Canceled = 0
				                AND Bookings.BookingDate BETWEEN DATEADD(day, -30, @Date) AND DATEADD(day, 14, @Date)
                                AND not (Bookings.BookingDate = @Date AND Bookings.TimeSlot = @timeslot)
			                GROUP BY PilotId
	                ) Bookings ON Pilots.Id = Bookings.PilotId
	                LEFT OUTER JOIN (
		                SELECT PilotId, COUNT(BookedPilots.Id) BookingCount
			                FROM BookedPilots 
			                INNER JOIN Bookings ON BookedPilots.BookingId = Bookings.Id
			                WHERE
                                COALESCE(PassengerFee, 0) > 0
				                AND BookedPilots.Canceled = 0
				                AND Bookings.Canceled = 0
				                AND CONVERT(date, Bookings.BookingDate) = @Date
                                
			                GROUP BY PilotId
	                ) BookingsToday ON Pilots.Id = BookingsToday.PilotId
                    LEFT OUTER JOIN (
		                SELECT PilotId, COUNT(BookedPilots.Id) BookingCount
			                FROM BookedPilots 
			                INNER JOIN Bookings ON BookedPilots.BookingId = Bookings.Id
			                WHERE
                                COALESCE(PassengerFee, 0) > 0
				                AND BookedPilots.Canceled = 0
				                AND Bookings.Canceled = 0
				                AND CONVERT(date, Bookings.BookingDate) = @Date
                                AND Bookings.TimeSlot = @timeslot
			                GROUP BY PilotId
	                ) BookingsTimeslot ON Pilots.Id = BookingsTimeslot.PilotId

	                WHERE
		                Pilots.IsPilot = 1
            ";

            var users = _context.Users.ToList();

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = _context.Database.GetExistingTransaction()?.GetDbTransaction() as SqlTransaction;
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("DateParam", date);
                cmd.Parameters.AddWithValue("timeslot", timeslot);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var pilotId = rd.GetString(rd.GetOrdinal("PilotId"));
                        var availabilities = rd.GetInt32(rd.GetOrdinal("AvailabilityCount"));
                        var bookings = rd.GetInt32(rd.GetOrdinal("BookingCount"));
                        var bookingsToday = rd.GetInt32(rd.GetOrdinal("BookingCountToday"));
                        var bookingsTimeslot = rd.GetInt32(rd.GetOrdinal("BookingCountTimeslot"));

                        var availablePilot = new AvailablePilot()
                        {
                            Pilot = users.Single(u => u.Id == pilotId),
                            Available = availabilities > 0 && bookingsTimeslot == 0,
                            Booked =  bookingsTimeslot > 0,
                            Priority = bookings
                                        + 1000*bookingsToday,
                            // a booking the same day is weighted more heavily to avoid pilots getting too many flights a day
                        };
                        availablePilots.Add(availablePilot);
                    }
                }
            }

            return availablePilots;
        }
    }
}
