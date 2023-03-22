using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Cms;
using TandemBooking.Services;

namespace TandemBooking.Tests.TestSetup
{
    public class MockNexmoMessage
    {
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public string Body { get; set; }
    }

    public class MockNexmoService : INexmoService
    {
        public List<MockNexmoMessage> Messages { get; set; } = new List<MockNexmoMessage>();

        public Task<string> FormatPhoneNumber(string phoneNumber, string countryCode = "NO")
        {
            if (phoneNumber?.Length == 8)
            {
                return Task.FromResult("47" + phoneNumber);
            }
            else
            {
                return Task.FromResult((string)null);
            }
        }

        public Task<JObject> LookupPhoneNumber(string phoneNumber, string countryCode = "NO")
        {
            return null;
        }

        public Task<NexmoSmsResult> SendSms(string @from, string to, string text)
        {
            Messages.Add(new MockNexmoMessage()
            {
                Sender = from,
                Recipient = to,
                Body = text,
            });

            return Task.FromResult(new NexmoSmsResult()
            {
                MessageCount = 1,
                Messages = new List<NexmoSmsResultMessage>()
                {
                    new NexmoSmsResultMessage()
                    {
                        MessageId = new Guid().ToString(),
                        ErrorText = "",
                        Status = "OK",
                    }
                }
            });
        }
    }
}