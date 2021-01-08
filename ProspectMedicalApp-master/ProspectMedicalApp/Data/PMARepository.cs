using Microsoft.EntityFrameworkCore;
using ProspectMedicalApp.Data.Entities;
using ProspectMedicalApp.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.Data
{

    public class PMARepository : IPMARepository
    {
        private readonly PMAContext context;

        public PMARepository(PMAContext context)
        {
            this.context = context;
        }

        public IEnumerable<Doctor> GetAllDoctors()
        {
            return context.Doctors.ToList();
        }

        public IEnumerable<Patient> GetAllPatients()
        {
            return context.Patients
                          .Include(p => p.Doctor)
                          .Include(p => p.Record)
                          .ThenInclude(r => r.Appointments)
                          .OrderBy(p => p.LastName)                      
                          .ToList();
        }

        public Patient GetPatientById(int id)
        {
            return context.Patients
                          .Include(p => p.Doctor)
                          .Include(p => p.Record)
                          .ThenInclude(r => r.Appointments)
                          .FirstOrDefault(p => p.Id == id);
                          

        }

        public IEnumerable<Patient> GetPatientByLastName(string lastName)
        {
            return context.Patients
                          .Include(p=>p.Doctor)
                          .Include(p=>p.Record)
                          .ThenInclude(r => r.Appointments)
                          .Where(p => p.LastName == lastName)
                          .OrderBy(p => p.FirstName)
                          .ToList();
        }

        public Patient GetPatientByUser(PMAUser user)
        {
            return context.Patients
                          .Include(p => p.Doctor)
                          .Include(p => p.Record)
                          .ThenInclude(r => r.Appointments)
                          .FirstOrDefault(p => p.User == user);
        }

        public IEnumerable<Procedure> GetAllProcedures()
        {
            return context.Procedures
                          .OrderBy(p => p.Category)
                          .ToList();
        }

        public Procedure GetProcedureByName(string name)
        {
            return context.Procedures.FirstOrDefault(p => p.Name == name);
        }

        public IEnumerable<Bill> GetAllPatientBills(int patientId)
        {
            var patient = context.Patients.FirstOrDefault(p => p.Id == patientId);

            if (patient != null)
            {
                return patient.Record.Appointments.Select(a => a.Bill).ToList();
            }
            else return null;
        }

        public IEnumerable<Bill> GetAllUnpaidPatientBills(int patientId)
        {
            var patient = context.Patients.FirstOrDefault(p => p.Id == patientId);

            if (patient != null)
            {
                return patient.Record.Appointments
                                    .Select(a => a.Bill)
                                    .Where(b => b.IsPaid == false)
                                    .ToList();
            }
            else return null;
        }

        public Doctor GetDoctorById(int id)
        {
            return context.Doctors.FirstOrDefault(d => d.Id == id);
        }

        public bool SaveAll()
        {
            return context.SaveChanges() > 0;
        }

        public IEnumerable<Appointment> GetAllAppointments()
        {
            return context.Appointments.Include(a => a.Bill).ToList();
        }

        public void AddPatient(Patient patient)
        {
            context.Patients.Add(patient);
        }

        public void AddUser(PMAUser user)
        {
            context.Users.Add(user);
        }

        public void DeletePatient(int id)
        {
            var patient = GetPatientById(id);
            context.Patients.Remove(patient);
        }

        public void UpdatePatient(Patient patient)
        {
            context.Patients.Update(patient);
        }

        public void AddAppointment(Appointment appointment)
        {
            context.Appointments.Add(appointment);

        }

        public Appointment GetAppointmentById(int appointmentId)
        {
            return context.Appointments.Include(a => a.Bill)
                                       .ThenInclude(b => b.Procedures)
                                       .FirstOrDefault(a => a.Id == appointmentId);
                                       
        }

        public bool DeleteAppointment(int id)
        {
            var appointment = GetAppointmentById(id);

            if (appointment != null)
            {
                context.Appointments.Remove(appointment);
                return true;
            }
            return false;
        }

        public Procedure GetProcedureById(int procedureId)
        {
            return context.Procedures.FirstOrDefault(p => p.Id == procedureId);
        }

        public void UpdateAppointment(Appointment appointment)
        {
            context.Update(appointment);
            
        }
    }
}
