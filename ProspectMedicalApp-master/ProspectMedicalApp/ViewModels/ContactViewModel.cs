using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [MinLength(2, ErrorMessage ="Name must be longer then 1 character")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [MinLength(2, ErrorMessage = "Name must be longer then 1 character")]
        public string LastName { get; set; }
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
        ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        public string PhoneNumber { get; set; }
        public string Reason { get; set; }
        public string ReasonInput { get; set; }
        [Required(ErrorMessage = "Message is required")]
        [MaxLength(500, ErrorMessage = "Message must be less than 500 characters")]
        public string Message { get; set; }
    }
}
