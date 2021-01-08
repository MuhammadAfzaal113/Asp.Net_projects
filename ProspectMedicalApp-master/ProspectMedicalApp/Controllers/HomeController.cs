using Microsoft.AspNetCore.Mvc;
using ProspectMedicalApp.Data;
using ProspectMedicalApp.Services;
using ProspectMedicalApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMailService mailService;

        public HomeController(IMailService mailService)
        {
            this.mailService = mailService;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Home";
            return View();
        }

        [HttpGet("services")]
        public IActionResult Services()
        {
            return View();
        }

        [HttpGet("staff")]
        public IActionResult Doctors()
        {
            return View();
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            ViewBag.UserMessage = "";
            ViewBag.ValidationError = "";
            return View();
        }

        [HttpPost("contact")]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PhoneNumber.Length == 10)
                {
                    ViewBag.UserMessage = "Message Sent!";

                    mailService.SendMessage("test@email.com", model.Reason, model.Message);

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.ValidationError = "Phone number is not valid, format: 1234567890";
                }
            }
            else
            {
                ViewBag.UserMessage = "";
                ViewBag.ValidationError = "";
            }

            return View();
        }

        [HttpGet("appointment")]
        public IActionResult NewAppointment()
        {
            ViewBag.ValidationError = "";
            ViewBag.AppointmentSuccess = "";
            return View();
        }

        [HttpPost("appointment")]
        public IActionResult NewAppointment(AppointmentRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PhoneNumber.Length == 10)
                {
                    ModelState.Clear();
                    ViewBag.AppointmentSuccess = "Your information has been successfully sent! We will be in touch with you shortly";
                }
                else
                {
                    ViewBag.ValidationError = "Phone number is not valid, format: 1234567890";
                }
            }
            else
            {
                ViewBag.AppointmentSuccess = "";
            }

            return View();
        }
    }
}
