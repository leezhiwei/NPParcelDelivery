using System.ComponentModel.DataAnnotations;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string UserType { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string LoginID { get; set; }
        public string Location { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Phone Number length is too long, please reduce length or use another phone number")]
        public string TelNo { get; set; }
        [StringLength(5, ErrorMessage = "Salutation length is too long, please reduce length or use another salutation.")]
        public string Salutation { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Birthday Date")]
        public DateTime? BirthDate { get; set; }
        [StringLength(50, ErrorMessage = "City name is too long, please try again.")]
        public string City { get; set; }
        [StringLength(50, ErrorMessage = "Country name is too long, please try again")]
        public string Country { get; set; }
    }
}
