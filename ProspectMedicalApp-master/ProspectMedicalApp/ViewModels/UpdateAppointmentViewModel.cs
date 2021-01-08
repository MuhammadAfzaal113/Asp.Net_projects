using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.ViewModels
{
    public class UpdateAppointmentViewModel
    {
        public int PatientId { get; set; }
        public AppointmentViewModel Appointment { get; set; }
        public List<ProcedureViewModel> Procedures { get; set; }
    }
}
