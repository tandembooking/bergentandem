using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TandemBooking.ViewModels.Manage
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }

        public IList<UserLoginInfo> Logins { get; set; }

        public string PhoneNumber { get; set; }

        public bool TwoFactor { get; set; }

        public bool BrowserRemembered { get; set; }

        public bool SmsNotification { get; set; }
        public bool EmailNotification { get; set; }

        [Display(Name = "Min passenger weight")]
        public int? MinPassengerWeight { get; set; }
        [Display(Name = "Max passenger weight")]
        public int? MaxPassengerWeight { get; set; }
    }
}
