using Microsoft.AspNetCore.Mvc.Rendering;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class DeliveryFailure
    {
        public int ReportID { get; set; }

        public int ParcelID { get; set; }

        public int DeliveryManID { get; set; }

        public string FailureType { get; set; }

        public string Description { get; set; }

        public int? StationMgrID { get; set; }

        public string FollowUpAction { get; set; }

        public DateTime DateCreated { get; set; }

    }
}
