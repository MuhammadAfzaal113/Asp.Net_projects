using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProspectMedicalApp.Data;
using ProspectMedicalApp.Data.Entities;
using ProspectMedicalApp.Data.Entity;
using ProspectMedicalApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly IPMARepository repository;
        private readonly SignInManager<PMAUser> signInManager;
        private readonly UserManager<PMAUser> userManager;
        private readonly IMapper mapper;

        public AccountController(ILogger<AccountController> logger, 
            IPMARepository repository, SignInManager<PMAUser> signInManager,
            UserManager<PMAUser> userManager, IMapper mapper)
        {
            this.logger = logger;
            this.repository = repository;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");             
            }

            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Login the user
                var result = await signInManager.PasswordSignInAsync(model.Username, 
                    model.Password, false, false);

                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

            }

            ModelState.AddModelError("", "Failed to login");

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
            
        }

        [HttpGet("Records")]
        public async Task<IActionResult> Record()
        {
            if (User.Identity.IsAuthenticated)
            {
                PatientRecordViewModel model = new PatientRecordViewModel();

                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var patient = repository.GetPatientByUser(user);

                if (patient != null)
                {
                    List<AppointmentViewModel> appointments = new List<AppointmentViewModel>();
                    foreach (var a in patient.Record.Appointments)
                    {
                        appointments.Add(mapper.Map<Appointment, AppointmentViewModel>(repository.GetAppointmentById(a.Id)));
                    }
                    model.Appointments = appointments;
                    model.Patient = mapper.Map<Patient, PatientViewModel>(patient);

                    return View(model);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                TempData["NeedLogin"] = "Please login to see your account info!";
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult Record(string submit, int appointmentId, int patientId)
        {
            switch (submit)
            {
                case "Select Appointment":

                    break;
                case "More Details":
                    var appointment = repository.GetAppointmentById(appointmentId);

                    break;
            }

            return View();
        }

        [HttpPost]
        public IActionResult EditPatient(PatientRecordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var patient = mapper.Map<PatientViewModel, Patient>(model.Patient);

                try
                {
                    repository.UpdatePatient(patient);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, $"Failed to update patient: {ex}");
                }
                try
                {
                    repository.SaveAll();
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, $"Failed to save updated patient to db: {ex}");

                }

                var updatedPatient = repository.GetPatientById(patient.Id);
            }
            else
            {
                TempData["FormError"] = "Input data not valid";
            }

            return RedirectToAction("Record");
        }

        [HttpPost("AppointmentDetails")]
        public IActionResult AppointmentDetails(int appointmentId)
        {
            var appointment = repository.GetAppointmentById(appointmentId);
            if(appointment != null)
            {
                return View(mapper.Map<Appointment, AppointmentViewModel>(appointment));
            }

            TempData["FormError"] = "Appointment not found. Sorry!";
            return RedirectToAction("Record");
        }

        [HttpGet("PayBill")]
        public IActionResult PayBill(int patientId, int appointmentId)
        {
            var patient = repository.GetPatientById(patientId);
            var appointment = repository.GetAppointmentById(appointmentId);

            if (patient != null && appointment != null)
            {
                List<AppointmentViewModel> appointments = new List<AppointmentViewModel>()
                {
                    mapper.Map<Appointment, AppointmentViewModel>(appointment)
                };

                PatientRecordViewModel model = new PatientRecordViewModel()
                {
                    Patient = mapper.Map<Patient, PatientViewModel>(patient),
                    Appointments = appointments
                };

                return View(model);
            }
            else
            {
                TempData["FormError"] = "Error obtaining appointment data";
                return RedirectToAction("Record");
            }
        }

        [HttpPost("PayBill")]
        public IActionResult PayBill(int appointmentId)
        {
            var appointment = repository.GetAppointmentById(appointmentId);
            if(appointment != null)
            {
                appointment.Bill.IsPaid = true;

                try
                {
                    repository.UpdateAppointment(appointment);
                }
                catch(Exception ex)
                {
                    logger.Log(LogLevel.Error, $"Failed to update appointment: {ex}");
                }

                try
                {
                    repository.SaveAll();
                }
                catch(Exception ex)
                {
                    logger.Log(LogLevel.Error, $"Failed to save updated appointment: {ex}");
                }

                return RedirectToAction("Record");
            }

            TempData["FormError"] = "Issue paying bill!";
            return RedirectToAction("Record");
        }
    }
}
