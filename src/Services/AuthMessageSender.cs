using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TandemBooking.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly INexmoService _nexmo;
        private readonly IMailService _mailService;
        private readonly ContentService _contService;

        public AuthMessageSender(INexmoService nexmo, IMailService mailService, ContentService content)
        {
            _nexmo = nexmo;
            _mailService = mailService;
            _contService = content;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await _mailService.Send(email, subject, message);
        }

        public async Task SendSmsAsync(string number, string message)
        {
            number = await _nexmo.FormatPhoneNumber(number);
            await _nexmo.SendSms(_contService.content.clubname, number, message);
        }
    }
}
