using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class Member
    {
        [Display(Name = "Member ID")]
        public int MemberID { get; set; }
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Salutation")]
		[Required]
		public string Salutation { get; set; }

        [Display(Name = "Phone Number")]
		[Required]
		public string TelNo { get; set; }
        [Display(Name = "Email Address")]
        [RegisterValidation]
        [RegexStringValidator("^\\\\S+@\\\\S+\\\\.\\\\S+$")]
		[Required]
		public string EmailAddr { get; set; }
        [Display(Name = "Password")]
		[Required]
		[DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "BirthDate")]
		[Required]
		public DateTime? BirthDate { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "Country")]
		[Required]
		public string Country { get; set; }

    }
}
