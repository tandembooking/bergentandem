using System;

namespace TandemBooking.Tests.TestSetup
{
    public class LocalDbException: Exception
    {
        public LocalDbException(string message) : base(message)
        {
        }

        public LocalDbException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}