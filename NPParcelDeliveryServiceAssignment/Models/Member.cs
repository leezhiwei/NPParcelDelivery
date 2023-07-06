using System.ComponentModel.DataAnnotations;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class Member
    {
        [Display(Name = "Member ID")]
        public int MemberID { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Salutation")]
        public string Salutation { get; set; }

        [Display(Name = "Phone Number")]
        public string TelNo { get; set; }
        [Display(Name = "Email Address")]
        public string EmailAddr { get; set; }
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "BirthDate")]
        public DateTime? BirthDate { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "Country")]
        public string Country { get; set; }

    }
}
