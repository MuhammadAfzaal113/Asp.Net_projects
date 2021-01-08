using System.Collections.Generic;
using ProspectMedicalApp.Data.Entities;
using ProspectMedicalApp.Data.Entity;

namespace ProspectMedicalApp.Data
{
    public interface IPMARepository
    {
        IEnumerable<Patient> GetAllPatients();
        Patient GetPatientByUser(PMAUser user);
        Patient GetPatientById(int id);
        IEnumerable<Patient> GetPatientByLastName(string lastName);
        void AddPatient(Patient patient);
        void DeletePatient(int id);
        void UpdatePatient(Patient patient);

        IEnumerable<Bill> GetAllPatientBills(int patientId);
        IEnumerable<Bill> GetAllUnpaidPatientBills(int patientId);

        IEnumerable<Procedure> GetAllProcedures();
        Procedure GetProcedureByName(string name);
        Procedure GetProcedureById(int procedureId);

        IEnumerable<Doctor> GetAllDoctors();
        Doctor GetDoctorById(int id);

        IEnumerable<Appointment> GetAllAppointments();
        Appointment GetAppointmentById(int appointmentId);
        void AddAppointment(Appointment appointment);
        bool DeleteAppointment(int id);
        void UpdateAppointment(Appointment appointment);


        void AddUser(PMAUser user);

        bool SaveAll();

    }
}