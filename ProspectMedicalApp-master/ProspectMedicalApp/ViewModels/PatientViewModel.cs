using System;
using System.ComponentModel.DataAnnotations;

namespace ProspectMedicalApp.ViewModels
{
    public class PatientViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "First name is required")]
        [MinLength(2,ErrorMessage = "First name should be longer than 2 characters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        [MinLength(2, ErrorMessage = "Last name should be longer than 2 characters")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",
        ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Street address is required")]
        public string Address1 { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string Address2 { get; set; }
        [Required(ErrorMessage = "State is required")]
        public string Address3 { get; set; }
        [Required]
        public int Zipcode { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        public string PhoneNumber { get; set; }
        public RecordViewModel Record { get; set; }
        public DoctorViewModel Doctor { get; set; }

    }
}
