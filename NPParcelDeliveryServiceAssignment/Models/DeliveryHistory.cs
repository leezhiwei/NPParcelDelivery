using System.ComponentModel.DataAnnotations;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class DeliveryHistory
    {
        [Display(Name = "Record ID")]
        public int RecordID { get; set; }
        [Display(Name = "Parcel ID")]
        public int ParcelID { get; set; }
        public string Description { get; set; }

    }
}
