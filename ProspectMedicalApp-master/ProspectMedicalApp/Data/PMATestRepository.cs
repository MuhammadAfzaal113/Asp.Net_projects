using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProspectMedicalApp.Data.Entities;
using ProspectMedicalApp.Data.Entity;

namespace ProspectMedicalApp.Data
{
    public class PMATestRepository : IPMARepository
    {
        List<Patient> testPatients;
        List<Doctor> testDoctors;
        List<Procedure> testProcedures;
        List<Appointment> testAppointments;
        List<PMAUser> testUsers;

        public PMATestRepository()
        {
            testPatients = new List<Patient>()
            {
                new Patient()
                {
                      Id = 1,
                      FirstName= "Hoster",
                      LastName= "Westy",
                      EmailAddress= "hostp@email.com",
                      DateOfBirth= new DateTime(1980, 01, 12),
                      Address1= "1234 test rd",
                      Address2= "testPlace",
                      Address3= "teststate",
                      Zipcode= 12345,
                      PhoneNumber= "8888675309"
                },
                new Patient()
                {
                      Id = 2,
                      FirstName= "Testy",
                      LastName= "Westy",
                      EmailAddress= "testp@email.com",
                      DateOfBirth= new DateTime(1984, 23, 06),
                      Address1= "1234 test rd",
                      Address2= "testPlace",
                      Address3= "teststate",
                      Zipcode= 12345,
                      PhoneNumber= "8888555559"
                }
            };
            testDoctors = new List<Doctor>();
            testProcedures = new List<Procedure>();
            testAppointments = new List<Appointment>();
            testUsers = new List<PMAUser>();
        }

        public IEnumerable<Patient> GetAllPatients()
        {
            return testPatients;
        }

        public Patient GetPatientById(int id)
        {
            return testPatients.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Patient> GetPatientByLastName(string lastName)
        {
            return testPatients.Where(p => p.LastName == lastName)
                               .OrderBy(p => p.FirstName)
                               .ToList();
        }

        public Patient GetPatientByUser(PMAUser user)
        {
            return testPatients.FirstOrDefault(p => p.User == user);
        }

        public IEnumerable<Procedure> GetAllProcedures()
        {
            return testProcedures;
        }

        public Procedure GetProcedureByName(string name)
        {
            return testProcedures.FirstOrDefault(p => p.Name == name);
        }

        public IEnumerable<Bill> GetAllPatientBills(int patientId)
        {
            var patient = testPatients.FirstOrDefault(p => p.Id == patientId);

            if (patient != null) return patient.Record.Appointments.Select(a => a.Bill).ToList();
            else return null;
        }

        public IEnumerable<Bill> GetAllUnpaidPatientBills(int patientId)
        {
            var patient = testPatients.FirstOrDefault(p => p.Id == patientId);

            if (patient != null) return patient.Record.Appointments.Select(a => a.Bill)
                                                                   .Where(b => b.IsPaid == false)
                                                                   .OrderBy(b => b.PaymentDueDate)
                                                                   .ToList();
            else return null;
        }

        public IEnumerable<Doctor> GetAllDoctors()
        {
            return testDoctors;
        }

        public Doctor GetDoctorById(int id)
        {
            return testDoctors.FirstOrDefault(d => d.Id == id);
        }

        public bool SaveAll()
        {
            return true;
        }

        public IEnumerable<Appointment> GetAllAppointments()
        {
            return testAppointments;
        }

        public void AddPatient(Patient patient)
        {
            testPatients.Add(patient);
        }

        public void AddUser(PMAUser user)
        {
            testUsers.Add(user);
        }

        public void DeletePatient(int id)
        {
            var patient = GetPatientById(id);
            if(patient != null)testPatients.Remove(patient);
        }

        public void UpdatePatient(Patient patient)
        {
            int index = testPatients.FindIndex(p => p.Id == patient.Id);
            testPatients[index] = patient;
        }

        public void AddAppointment(Appointment appointment)
        {
            testAppointments.Add(appointment);
        }

        public Appointment GetAppointmentById(int appointmentId)
        {
            return testAppointments.FirstOrDefault(a => a.Id == appointmentId);
        }

        public bool DeleteAppointment(int id)
        {
            var appointment = testAppointments.FirstOrDefault(a => a.Id == id);
            if(appointment != null)
            {
                testAppointments.Remove(appointment);
                return true;
            }
            return false;
        }

        public Procedure GetProcedureById(int procedureId)
        {
            return testProcedures.FirstOrDefault(p => p.Id == procedureId);
        }

        public void UpdateAppointment(Appointment appointment)
        {
            int index = testAppointments.FindIndex(a => a.Id == appointment.Id);
            testAppointments[index] = appointment;
        }
    }
}
