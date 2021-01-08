using Microsoft.AspNetCore.Identity;
using ProspectMedicalApp.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.Data.Entities
{
    public class PMAUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
