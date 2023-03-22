using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TandemBooking.Services;

namespace TandemBooking.Tests.TestSetup
{
    public class MockMailMessage
    {
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class MockMailService : IMailService
    {
        public List<MockMailMessage> Messages { get; set; } = new List<MockMailMessage>();

        public Task Send(string recipient, string subject, string body)
        {
            Messages.Add(new MockMailMessage()
            {
                Recipient = recipient,
                Subject = subject,
                Body = body,
            });

            return Task.CompletedTask;
        }
    }
}