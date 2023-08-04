using System.ComponentModel.DataAnnotations;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class Staff
    {
        [Display(Name = "Staff ID")]
        public int StaffID { get; set; }
        [Display(Name = "Staff Name")]
        public string StaffName { get; set; }
        [Display(Name = "Login ID")]
        public string LoginID { get; set; }
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Appointment")]
        public string Appointment { get; set; }
        [Display(Name = "Office Telephone Number")]
        public string OfficeTelNo { get; set; }
        [Display(Name = "Location")]
        public string Location { get; set; }
    }
}

