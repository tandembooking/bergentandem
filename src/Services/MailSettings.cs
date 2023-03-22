using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TandemBooking.Services
{
    public class MailSettings
    {
        public bool Enable { get; set; }

        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPassword { get; set; }

        public string FromName { get; set; }
        public string FromAddress { get; set; }
    }
}
