using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TandemBooking.Services;
using TandemBooking.Attributes;

namespace TandemBooking.Controllers
{
    //[LocalizationAttribute]
    [ServiceFilter(typeof(LocalizationAttribute))]
    public class HomeController : Controller
    {
        //private ContentService _content;
        //public HomeController(ContentService content)
        //{
        //    _content = content;
        //}
        public IActionResult Index(string lang)
        {
            //this takes request parameters only from the query string
            //_content.setLanguage(lang);

            if (User.IsAdmin() || User.IsPilot())
            {
                return RedirectToAction("Index", "Overview");
            }
            return View();
        }

        public IActionResult Faq()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
