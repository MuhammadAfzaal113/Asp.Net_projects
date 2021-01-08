using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.Data.Entity
{
    public class Bill
    {
        public int Id { get; set; }
        public bool IsPaid { get; set; }
        public DateTime DateIssued { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public ICollection<Procedure> Procedures { get; set; }
    }
}
