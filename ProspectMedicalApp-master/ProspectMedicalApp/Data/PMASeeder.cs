using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProspectMedicalApp.Data.Entities;
using ProspectMedicalApp.Data.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.Data
{
    public class PMASeeder
    {
        private readonly PMAContext context;
        private readonly IHostingEnvironment hosting;
        private readonly UserManager<PMAUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public PMASeeder(PMAContext context, IHostingEnvironment hosting, 
            UserManager<PMAUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.hosting = hosting;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            context.Database.EnsureCreated();

            PMAUser user = await userManager.FindByEmailAsync("testP@email.com");
            PMAUser adminUser = await userManager.FindByEmailAsync("tadminP@email.com");
            if(user == null)
            {
                var roleExist = await roleManager.RoleExistsAsync("Patient");
                if (!roleExist)
                {
                    var role = new IdentityRole();
                    role.Name = "Patient";
                    await roleManager.CreateAsync(role);
                }

                user = new PMAUser()
                {
                    FirstName = "Testy",
                    LastName = "Westy",
                    Email = "testP@email.com",
                    UserName = "TWesty"
                };

                var result = await userManager.CreateAsync(user, "TestP@ss!1");
                if(result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create new user in seeder");
                }
                else
                {
                    await userManager.AddToRoleAsync(user, "Patient");
                }
            }

            if (adminUser == null)
            {
                var roleExist = await roleManager.RoleExistsAsync("Admin");
                if (!roleExist)
                {
                    var role = new IdentityRole();
                    role.Name = "Admin";
                    await roleManager.CreateAsync(role);
                }

                adminUser = new PMAUser()
                {
                    FirstName = "Tad",
                    LastName = "Min",
                    Email = "tadminP@email.com",
                    UserName = "TadMin"
                };

                var result = await userManager.CreateAsync(adminUser, "TestP@ss!1");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create new user in seeder");
                }
                else
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            if (!context.Doctors.Any())
            {
                //Create sample data
                var filepath = Path.Combine(hosting.ContentRootPath, "Data/data.json");
                var json = File.ReadAllText(filepath);
                var data = JArray.Parse(json);
                var doctors = JsonConvert.DeserializeObject<IEnumerable<Doctor>>(data[0].ToString());
                var procedures = JsonConvert.DeserializeObject<IEnumerable<Procedure>>(data[1].ToString());
                var patients = JsonConvert.DeserializeObject<IEnumerable<Patient>>(data[2].ToString());

                var patient = patients.FirstOrDefault(p => p.FirstName == "Testy");
                if (patient != null)
                {
                    patient.User = user;
                    patient.Doctor = doctors.First();
                }

                foreach (var p in patients)
                {
                    p.Doctor = doctors.First();
                    p.Record = new Record()
                    {
                        Appointments = new List<Appointment>()
                       
                    };
                }

                context.Doctors.AddRange(doctors);
                context.Procedures.AddRange(procedures);
                context.Patients.AddRange(patients);

                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                }
            }
        }
    }
}
