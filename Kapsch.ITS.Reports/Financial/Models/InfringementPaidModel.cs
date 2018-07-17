namespace Kapsch.ITS.Reports.Financial.Models
{
    public class InfringementPaidModel
    {
        public string DistrictName { get; set; }
        public long DistrictID { get; set; }
        public int Count { get; set; }
        public decimal Value { get; set; }
    }
}
