using ProspectMedicalApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.Data.Entity
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public int Zipcode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public Doctor Doctor { get; set; }
        public Record Record { get; set; }
        public PMAUser User { get; set; }
    }
}
