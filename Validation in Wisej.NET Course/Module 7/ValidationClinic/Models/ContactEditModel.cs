using System;
using System.ComponentModel.DataAnnotations;

namespace ValidationClinic.Models
{
    // Each editor and grid receives a working copy; changing it is not a save.
    public class ContactEditModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required."), StringLength(80, ErrorMessage = "Name must be 80 characters or fewer.")]
        public string Name { get; set; } = "";
        [Required(ErrorMessage = "Email is required."), EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = "";
        [Range(0, 120, ErrorMessage = "Enter an age from 0 to 120.")]
        public int? Age { get; set; } = 30;
        public string Phone { get; set; } = "2125550123";
        public string CustomerCode { get; set; } = "CUS-0001";
        public string CreditLimit { get; set; } = "100";
        public DateTime? BirthDate { get; set; } = DateTime.Today.AddYears(-30);
        public DateTime? StartDate { get; set; } = DateTime.Today;
        public DateTime? EndDate { get; set; } = DateTime.Today.AddDays(7);
        public string Status { get; set; } = "Open";
        public DateTime? ClosedDate { get; set; }
        public ContactEditModel Copy() => (ContactEditModel)MemberwiseClone();
    }
}
