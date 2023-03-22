using System;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TandemBooking.Models;
using Microsoft.EntityFrameworkCore;
using TandemBooking.Services;
using TandemBooking.ViewModels.BookingAdmin;

namespace TandemBooking.Controllers
{
    public class DeliveryReportController : Controller
    {
        private readonly SmsService _smsService;

        public DeliveryReportController(SmsService smsService)
        {
            _smsService = smsService;
        }

        public async Task<ActionResult> Index()
        {
            var deliveryReport = new NexmoDeliveryReport()
            {
                ClientRef = Request.Query["client-ref"],
                ErrCode = Request.Query["err-code"],
                Msisdn = Request.Query["msisdn"],
                MessageId = Request.Query["messageId"],
                NetworkCode = Request.Query["network-code"],
                Status = Request.Query["status"],
                To = Request.Query["to"],
            };
            await _smsService.HandleDeliveryReport(deliveryReport);

            return new OkResult();
        }
    }
}