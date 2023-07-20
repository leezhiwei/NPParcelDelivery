using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class Member
    {
        [Display(Name = "Member ID")]
        public int MemberID { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Salutation")]
		public string Salutation { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
		public string TelNo { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [RegisterValidation]
        [RegexStringValidator("^\\\\S+@\\\\S+\\\\.\\\\S+$")]
		public string EmailAddr { get; set; }

        [Required]
        [Display(Name = "Password")]
		[DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Display(Name = "BirthDate")]
		public DateTime? BirthDate { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Country")]
		public string Country { get; set; }

    }
}
