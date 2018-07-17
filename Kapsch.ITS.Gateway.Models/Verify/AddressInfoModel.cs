using System;

namespace Kapsch.ITS.Gateway.Models.Verify
{
    public class AddressInfoModel
    {
        public int Key { get; set; }
        public string Street { get; set; }
        public string Suburb { get; set; }
        public string Town { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public string POBox { get; set; }
        public string Residual { get; set; }
        public int ResidualScore { get; set; }
        public int PersonID { get; set; }
        public string IDNumber { get; set; }
        public string CompanyName { get; set; }
        public string AddressDate { get; set; }
        public string UserDetails { get; set; }
        public string Source { get; set; }
        public int AddressTypeID { get; set; }
    }
}
