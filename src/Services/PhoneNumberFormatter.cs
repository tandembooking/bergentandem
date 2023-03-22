namespace TandemBooking.Services
{
    public static class PhoneNumberFormatter
    {
        public static string AsPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return phoneNumber;

            return $"+{phoneNumber}";
        }
    }
    public static class TimeSlotFormatter
    {
        public static string asTime(this int timeslot)
        {
            string[] times = new string[] { "10:00", "12:00", "14:00", "16:00", "18:00", "20:00" };
            if (timeslot>= 0 && timeslot < times.Length) { return times[timeslot]; }
            return "Timeslot error";
        }
    }
}