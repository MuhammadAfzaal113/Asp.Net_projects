using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.ViewModels
{
    public class AdminViewModel
    {
        public PatientViewModel Patient { get; set; }
        public IEnumerable<DoctorViewModel> Doctors { get; set; }
        public IEnumerable<AppointmentViewModel> Appointments { get; set; }
    }
}
