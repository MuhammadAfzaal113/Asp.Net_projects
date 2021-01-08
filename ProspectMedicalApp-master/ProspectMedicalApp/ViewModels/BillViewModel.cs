using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.ViewModels
{
    public class BillViewModel
    {
        public bool IsPaid { get; set; }
        public DateTime DateIssued { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public ICollection<ProcedureViewModel> Procedures { get; set; }
    }
}
