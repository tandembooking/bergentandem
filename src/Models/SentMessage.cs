using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TandemBooking.Models
{
    public class SentSmsMessage
    {
        public Guid Id { get; set; }
        public Guid? BookingId { get; set; }

        public string RecipientNumber { get; set; }
        public string MessageText { get; set; }
        public DateTime SentDate { get; set; }
        public int RetryCount { get; set; }
        public DateTime LastRetryDate { get; set; }
        public DateTime NextRetryDate { get; set; }

        public Booking Booking { get; set; }
        public ICollection<SentSmsMessagePart> SmsMessageParts { get; set; }
    }

    public class SentSmsMessagePart
    {
        public Guid Id { get; set; }
        public string GatewayMessageId { get; set; }
        public string StatusCode { get; set; }
        public string StatusText { get; set; }

        public DateTime? DeliveryReportDate { get; set; }
        public string DeliveryReportStatus { get; set; }
        public string DeliveryReportErrorCode { get; set; }

        public SentSmsMessage Message { get; set; }
    }
}
