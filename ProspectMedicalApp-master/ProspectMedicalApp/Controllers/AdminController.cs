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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> logger;
        private readonly IPMARepository repository;
        private readonly IMapper mapper;
        private readonly UserManager<PMAUser> userManager;

        public AdminController(ILogger<AdminController> logger, IPMARepository repository, 
            IMapper mapper, UserManager<PMAUser> userManager)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet("Main")]
        public IActionResult Main()
        {
            if (!CheckAuthorization()) return AuthorizeSendBack();

            var patients = repository.GetAllPatients();
            return View(mapper.Map<IEnumerable<Patient>, 
                IEnumerable<PatientViewModel>>(patients));
        }

        [HttpPost("Main")]
        public IActionResult Main(PatientViewModel model, string submit)
        {
            Patient patient = null;
            switch (submit)
            {
                case "Find Patient":
                    patient = SortPatientByName(model);
                    break;
                case "Select Patient":
                    patient = repository.GetPatientById(model.Id);
                    break;
            }

            if(patient != null)
            {
                //AdminViewModel aModel = FillAdminModel();
                //aModel.Patient = mapper.Map<Patient, PatientViewModel>(patient);
                //return View("PatientInfo", aModel);
                return RedirectToAction("PatientInfo", new { id = patient.Id });
            }

            TempData["FormError"] = "There was an issue getting the patient data";
            return RedirectToAction("Main");
        }

        [HttpGet("PatientInfo")]
        public IActionResult PatientInfo(int id)
        {
            if (!CheckAuthorization()) return AuthorizeSendBack();

            var patient = repository.GetPatientById(id);
            if(patient != null)
            {
                AdminViewModel aModel = FillAdminModel();
                aModel.Patient = mapper.Map<Patient, PatientViewModel>(patient);
                return View(aModel);
            }

            TempData["FormError"] = "Error getting patient data";
            return RedirectToAction("Main");
        }

        [HttpGet("AddPatient")]
        public IActionResult AddPatient()
        {
            AdminViewModel model = new AdminViewModel();
            model = FillAdminModel();

            return View(model);
        }

        [HttpPost("AddPatient")]
        public async Task<IActionResult> AddPatient(AdminViewModel model, int doctorId)
        {
            if (ModelState.IsValid)
            {
                var patient = mapper.Map<PatientViewModel, Patient>(model.Patient);

                var doctor = repository.GetDoctorById(doctorId);
                if (doctor != null) patient.Doctor = doctor;

                patient.Record = new Record()
                {
                    Appointments = new List<Appointment>()
                };

                var user = new PMAUser()
                {
                    FirstName = model.Patient.FirstName,
                    LastName = model.Patient.LastName,
                    Email = model.Patient.EmailAddress,
                    PhoneNumber = model.Patient.PhoneNumber,
                    UserName = model.Patient.FirstName.Substring(0, 1) + model.Patient.LastName
                };

                //To Do: Add random generation of password, then after successful creation
                //send email to patient informing them of their login info.

                var result = await userManager.CreateAsync(user, "TestP@ss!1");
                if (!result.Succeeded)
                {
                    logger.Log(LogLevel.Error, $"Failed to create user account. {result.Errors.ToString()}");
                }
                else
                {
                    await userManager.AddToRoleAsync(user, "Patient");
                    patient.User = user;
                }

                try
                {
                    repository.AddPatient(patient);
                }
                catch(Exception ex)
                {
                    logger.Log(LogLevel.Error, $"Failed to add patient to db: {ex}");
                }

                try
                {
                    repository.SaveAll();
                }
                catch(Exception ex)
                {
                    logger.Log(LogLevel.Error, $"Failed to save added patient to db: {ex}");
                }

                return RedirectToAction("PatientInfo", new { id = patient.Id });
            }


            TempData["FormError"] = "Patient data is invalid";
            return View(model);
        }

        [HttpPost("DeletePatient")]
        public IActionResult DeletePatient(AdminViewModel model)
        {
            try
            {
                repository.DeletePatient(model.Patient.Id);
            }
            catch(Exception ex)
            {
                logger.Log(LogLevel.Error, $"Failed to delete patient: {ex}");
            }
            try
            {
                repository.SaveAll();
            }
            catch(Exception ex)
            {
                logger.Log(LogLevel.Error, $"Failed to save deleted patient to db: {ex}");
            }

            TempData["FormSuccess"] = "Succesfully deleted patient";
            return RedirectToAction("Main");
        }

        [HttpPost("EditPatient")]
        public IActionResult EditPatient(AdminViewModel model, int doctorId)
        {
            if (ModelState.IsValid)
            {
                var doctor = repository.GetDoctorById(doctorId);
                var patient = mapper.Map<PatientViewModel, Patient>(model.Patient);
                if (doctor != null) patient.Doctor = doctor;

                try
                {
                    repository.UpdatePatient(patient);
                }
                catch(Exception ex)
                {
                    logger.Log(LogLevel.Error, $"Failed to update patient: {ex}");
                }
                try
                {
                    repository.SaveAll();
                }
                catch(Exception ex)
                {
                    logger.Log(LogLevel.Error, $"Failed to save updated patient to db: {ex}");

                }

                var updatedPatient = repository.GetPatientById(patient.Id);

                if (updatedPatient != null)
                { 
                    return RedirectToAction("PatientInfo", new { id = updatedPatient.Id });
                }
            }
            else
            {
                TempData["FormError"] = "Edit form data not filled correctly.";
                
            }

            return RedirectToAction("PatientInfo", new { id = model.Patient.Id });
            
        }

        [HttpPost("AddAppointment")]
        public IActionResult AddAppointment(string appointmentDate, string appointmentTime, int patientId)
        {
            var patient = repository.GetPatientById(patientId);

            if (appointmentTime.Length == 7) appointmentTime = "0" + appointmentTime;
            string concatDate = appointmentDate + " " + appointmentTime;

            DateTime date = DateTime.ParseExact(concatDate, "M/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);

            Appointment newAppointment = new Appointment()
            {
                Date = date,
                Bill = new Bill()
                {
                    DateIssued = date,
                    PaymentDueDate = date.AddDays(30),
                    Procedures = new List<Procedure>()
                }
     
            };

            var initialProcedure = repository.GetProcedureByName("Visit");
            if (initialProcedure != null) newAppointment.Bill.Procedures.Add(initialProcedure);

            var checkDate = patient.Record.Appointments.FirstOrDefault(a => a.Date == date);

            if (checkDate == null)
            {

                try
                {
                    repository.AddAppointment(newAppointment);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, $"Error adding appointment: {ex}");
                }

                patient.Record.Appointments.Add(newAppointment);

                try
                {
                    repository.UpdatePatient(patient);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, $"Error adding appointment to patient: {ex}");
                }

                repository.SaveAll();
            }
            else
            {
                TempData["FormError"] = "Appointment already exists. Try a different date or time";
            }

            AdminViewModel adModel = FillAdminModel();
            adModel.Patient = mapper.Map<Patient, PatientViewModel>(patient);

            return View("PatientInfo", adModel);
        }

        [HttpPost("DeleteAppointment")]
        public IActionResult DeleteAppointment(AdminViewModel model, int appointmentId)
        {
            var patient = repository.GetPatientById(model.Patient.Id);

            if (repository.DeleteAppointment(appointmentId))
            {
                repository.SaveAll();
            }
            else
            {
                TempData["FormError"] = "There was an issue deleting the patient";
            }

            AdminViewModel adModel = FillAdminModel();
            adModel.Patient = mapper.Map<Patient, PatientViewModel>(patient);

            return View("PatientInfo", adModel);
        }

        [HttpGet("PatientAppointment")]
        public IActionResult PatientAppointment(PatientViewModel model)
        {
            var adminModel = new AdminViewModel();
            adminModel.Patient = model;

            var doctors = mapper.Map<IEnumerable<Doctor>, IEnumerable<DoctorViewModel>>(repository.GetAllDoctors());
            var appointments = mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentViewModel>>(repository.GetAllAppointments());

            adminModel.Doctors = doctors;
            adminModel.Appointments = appointments;

            return View(adminModel);
        }

        [HttpGet("UpdateAppointment")]
        public IActionResult UpdateAppointment(int id, int patientId)
        {
            if (!CheckAuthorization()) return AuthorizeSendBack();

            UpdateAppointmentViewModel model = new UpdateAppointmentViewModel();
            var appointment = repository.GetAppointmentById(id);
            if(appointment != null)
            {
                model.Appointment = mapper.Map<Appointment, AppointmentViewModel>(appointment);
            }

            var procedures = repository.GetAllProcedures();
            model.Procedures = mapper.Map<IEnumerable<Procedure>, IEnumerable<ProcedureViewModel>>(procedures).ToList();
            model.PatientId = patientId;

            return View(model);
        }

        [HttpPost("UpdateAppointment")]
        public IActionResult UpdateAppointment(UpdateAppointmentViewModel model, int appointmentId, int procedureId, string AppointmentSubmit)
        {
            switch (AppointmentSubmit)
            {
                case "Add To Appointment":
                    return AddProcedureToAppointment(model, appointmentId, procedureId);
                case "Remove":
                    return RemoveProcedureFromAppointment(model, appointmentId, procedureId);
                case "Back To Patient":
                    var patient = repository.GetPatientById(model.PatientId);
                    if (patient != null)
                    {
                        return RedirectToAction("PatientInfo", new { id = patient.Id });
                    }
                    break;
            }

            return View(new { id = appointmentId, patientId = model.PatientId});
        }

        [HttpPost]
        public IActionResult AppointmentToPatientInfo(int id)
        {

            var patient = repository.GetPatientById(id);
            if(patient != null)
            {
                AdminViewModel model = new AdminViewModel();

                var doctors = mapper.Map<IEnumerable<Doctor>, IEnumerable<DoctorViewModel>>(repository.GetAllDoctors());
                var appointments = mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentViewModel>>(repository.GetAllAppointments());

                model.Patient = mapper.Map<Patient, PatientViewModel>(patient);
                model.Doctors = doctors;
                model.Appointments = appointments;

                return View("PatientInfo", model);
            }


            TempData["FormError"] = "Issue finding patient data";
            return RedirectToAction("Main");
        }

        [HttpGet]
        public IActionResult AppointmentToMain()
        {
            return RedirectToAction("Main");
        }

        Patient SortPatientByName(PatientViewModel model)
        {
            var patient = repository.GetPatientByLastName(model.LastName);
            if (patient.Count() == 0) return null;
            if (patient.Count() == 1) return patient.First();

            var sortedPatient = patient.Where(p => p.FirstName.ToLower() == model.FirstName).ToList();
            if (sortedPatient.Count() == 0) return null;
            if (sortedPatient.Count() == 1) return sortedPatient.First();

            return null;
        }

        AdminViewModel FillAdminModel()
        {
            AdminViewModel model = new AdminViewModel();

            var appointments = repository.GetAllAppointments();
            if (appointments != null)
            {
                model.Appointments = mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentViewModel>>
                                    (appointments);
            }
            else
            {
                model.Appointments = new List<AppointmentViewModel>();
            }

            var doctors = repository.GetAllDoctors();
            if (doctors != null)
            {
                model.Doctors = mapper.Map<IEnumerable<Doctor>, IEnumerable<DoctorViewModel>>
                    (doctors);
            }
            else
            {
                model.Doctors = new List<DoctorViewModel>();
            }

            return model;
        }

        IActionResult AddProcedureToAppointment(UpdateAppointmentViewModel model, int appointmentId, int procedureId)
        {
            UpdateAppointmentViewModel newModel = new UpdateAppointmentViewModel();
            var appointment = repository.GetAppointmentById(appointmentId);
            var procedure = repository.GetProcedureById(procedureId);

            if (appointment == null || procedure == null)
            {
                TempData["FormError"] = "There was an error adding procedure to appointment";
            }
            else
            {
                appointment.Bill.Procedures.Add(procedure);
                try
                {
                    repository.SaveAll();
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, $"Could not save procedure to appointment, {ex}");
                }
            }

            var procedures = repository.GetAllProcedures();

            newModel.Procedures = mapper.Map<IEnumerable<Procedure>, IEnumerable<ProcedureViewModel>>(procedures).ToList();
            newModel.Appointment = mapper.Map<Appointment, AppointmentViewModel>(appointment);
            newModel.PatientId = model.PatientId;

            return View(newModel);
        }
        IActionResult RemoveProcedureFromAppointment(UpdateAppointmentViewModel model, int appointmentId, int procedureId)
        {
            UpdateAppointmentViewModel newModel = new UpdateAppointmentViewModel();
            var appointment = repository.GetAppointmentById(appointmentId);
            var procedure = repository.GetProcedureById(procedureId);

            if (appointment == null || procedure == null)
            {
                TempData["FormError"] = "There was an error removing procedure to appointment";
            }
            else
            {
                appointment.Bill.Procedures.Remove(procedure);
                try
                {
                    repository.SaveAll();
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, $"Could not save removal of procedure from appointment, {ex}");
                }
            }

            var procedures = repository.GetAllProcedures();

            newModel.Procedures = mapper.Map<IEnumerable<Procedure>, IEnumerable<ProcedureViewModel>>(procedures).ToList();
            newModel.Appointment = mapper.Map<Appointment, AppointmentViewModel>(appointment);
            newModel.PatientId = model.PatientId;

            return View(newModel);
        }

        bool CheckAuthorization()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return true;
                }
            }

            return false;
        }
        IActionResult AuthorizeSendBack()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login", "Account");
        }
    }
}
