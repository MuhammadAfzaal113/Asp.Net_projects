using ProspectMedicalApp.Data.Entities;
using System.Collections.Generic;

namespace ProspectMedicalApp.ViewModels
{
    public class RecordViewModel
    {
        public int Id { get; set; }
        public ICollection<AppointmentViewModel> Appointments { get; set; }
    }
}
