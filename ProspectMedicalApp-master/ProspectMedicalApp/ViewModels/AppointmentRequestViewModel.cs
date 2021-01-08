using System;
using System.ComponentModel.DataAnnotations;

namespace ProspectMedicalApp.ViewModels
{
    public class AppointmentRequestViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [MinLength(2)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [MinLength(2)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Phone number needs to be in the format 1234567890")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please select a date for the appointment")]
        [DataType(DataType.Date)]
        public DateTime? AppointmentDate { get; set; }
        public string AppointmentType { get; set; }
        [Required(ErrorMessage = "Message is required")]
        [MaxLength(500)]
        public string Message { get; set; }
    }
}
