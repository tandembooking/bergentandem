namespace TandemBooking.Services
{
    public static class WeightFormatter
    {
        public static string AsWeight(this int? weight)
        {
            if (weight == null)
                return null;

            return $"{weight}kg";
        }
    }
}