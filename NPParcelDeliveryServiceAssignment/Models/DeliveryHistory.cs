namespace NPParcelDeliveryServiceAssignment.Models
{
    public class DeliveryHistory
    {
        public int RecordID { get; set; }

        public int ParcelID { get; set; }

        public DateTime? ReceiveTime { get; set; } 
        public string Description { get; set; }

    }
}
