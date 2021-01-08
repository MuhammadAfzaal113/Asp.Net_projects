using ProspectMedicalApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.Data.Entity
{
    public class Record
    {
        public int Id { get; set; }
        public ICollection<Appointment> Appointments { get; set; }

    }
}
