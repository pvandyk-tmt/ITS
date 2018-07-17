using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.Configuration
{
    public class DistrictModel
    {
        public long ID { get; set; }
        public string Province { get; set; }
        public string Town { get; set; }
        public string Suburb { get; set; }
        public string Street { get; set; }
        public string Code { get; set; }
        public string POBox { get; set; }
        public string PostalCode { get; set; }
        public string PostalStreet { get; set; }
        public string PostalSuburb { get; set; }
        public string PostalTown { get; set; }
        public string TicketPre { get; set; }
        public string TicketPost { get; set; }
        public string TicketSequenceName { get; set; }
        public string CaseSequenceName { get; set; }
        public string Section56TicketPre { get; set; }
        public string Section56TicketPost { get; set; }
        public string TrafficTicketPre { get; set; }
        public string TrafficTicketPost { get; set; }
        public string DepartmantName { get; set; }
        public string Telephone { get; set; }
        public string Faks { get; set; }
        public string CaseNoPre { get; set; }
        public string CaseNoPost { get; set; }
        public string DistrictAll { get; set; }
        public long RegionID { get; set; }
        public long DistrictTypeID { get; set; }
        public string AccountNo { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
        public string BranchCode { get; set; }
        public string AccountType { get; set; }
        public string BankDetails { get; set; }
        public long? ExtTicketPost { get; set; }
        public string IACode { get; set; }
        public long? NatisNumber { get; set; }
        public string NatisSequenceName { get; set; }
        public string OfficeHours { get; set; }
        public string Sectiontion67Pre { get; set; }
        public string Section67Post { get; set; }
        public long? J175Printing { get; set; }
        public long? CentralCaptureIndicator { get; set; }
        public long ActiveIndicator { get; set; }
        public string EmailAddress { get; set; }
        public string ITicketPre56 { get; set; }
        public string ITicketSeqName56 { get; set; }
        public string ITicketPre341 { get; set; }
        public string ITicketSeqName341 { get; set; }
        public string BranchName { get; set; }
        public byte[] SiteLogo { get; set; }
        public string PaymentOptions { get; set; }
    }
}
