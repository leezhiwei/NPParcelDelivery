namespace NPParcelDeliveryServiceAssignment.Models
{
    public class Parcel
    {
        public int ParcelID { get; set; }

        public string ItemDescription { get; set; }

        public string SenderName { get; set; }

        public string SenderTelNo { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverTelNo { get; set; }

        public string DeliveryAddress { get; set; }

        public string FromCity { get; set; }

        public string FromCountry { get; set; }

        public string ToCity { get; set; }

        public string ToCountry { get; set; }

        public double ParcelWeight { get; set; }

        public decimal DeliveryCharge { get; set; }

        public string Currency { get; set; }

        public DateTime? TargetDeliveryDate { get; set; }

        public string DeliveryStatus { get; set; }

        public int? DeliveryManID { get; set; }

    }
}
