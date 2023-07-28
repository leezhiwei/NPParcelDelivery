namespace NPParcelDeliveryServiceAssignment.Models
{
    public class APIObject
    {
        public string MemberName { get; set; }
        public string MemberSalutation { get; set; }
        public List<Parcel> ParcelsAssignedMember { get; set; }
        public List<CashVoucher> CashVouchersAssigned { get; set; }
    }
}
