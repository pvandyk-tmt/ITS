using Kapsch.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("LK_DISTRICT", Schema = "ITS")]
    public class District
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("PROVINCE")]
        public string Province {get;set;}

        [Column("TOWN")]
        public string Town {get;set;}

        [Column("SUBURB")]
        public string Suburb {get;set;}

        [Column("STREET")]
        public string Street {get;set;}

        [Column("CODE")]
        public string Code{get;set;}

        [Column("PO_BOX")]
        public string POBox {get;set;}

        [Column("POSTAL_CODE")]
        public string PostalCode {get;set;}

        [Column("POSTAL_STREET")]
        public string PostalStreet {get;set;}

        [Column("POSTAL_SUBURB")]
        public string PostalSuburb {get;set;}

        [Column("POSTAL_TOWN")]
        public string PostalTown {get;set;}

        [Column("TICKET_PRE")]
        public string TicketPre {get;set;}

        [Column("TICKET_POST")]
        public string TicketPost {get;set;}

        [Column("TICKET_SEQUENCE_NAME")]
        public string TicketSequenceName {get;set;}

        [Column("CASE_SEQUENCE_NAME")]
        public string CaseSequenceName {get;set;}

        [Column("SECTION56_TICKET_PRE")]
        public string Section56TicketPre { get; set; }

        [Column("SECTION56_TICKET_POST")]
        public string Section56TicketPost { get; set; }

        [Column("TRAFFIC_TICKET_PRE")]
        public string TrafficTicketPre {get;set;}

        [Column("TRAFFIC_TICKET_POST")]
        public string TrafficTicketPost {get;set;}

        [Column("DEPARTMENT_NAME")]
        public string DepartmantName {get;set;}

        [Column("TELEPHONE")]
        public string Telephone {get;set;}

        [Column("FAKS")]
        public string Faks {get;set;}

        [Column("CASE_NO_PRE")]
        public string CaseNoPre {get;set;}

        [Column("CASE_NO_POST")]
        public string CaseNoPost {get;set;}

        [Column("DISTRICT_ALL")]
        public string DistrictAll {get;set;}

        [Column("REGION_ID")]
        public long RegionID {get;set;}

        [Column("DISTRICT_TYPE_ID")]
        public long DistrictTypeID {get;set;}

        [Column("ACC_NUMBER")]
        public string AccountNo {get;set;}

        [Column("BANK")]
        public string Bank {get;set;}

        [Column("BRANCH")]
        public string Branch {get;set;}

        [Column("BRANCH_CODE")]
        public string BranchCode {get;set;}

        [Column("ACCOUNT_TYPE")]
        public string AccountType {get;set;}

        [Column("BANK_DETAILS")]
        public string BankDetails {get;set;}

        [Column("EXT_TICKET_POST")]
        public long? ExtTicketPost {get;set;}

        [Column("IA_CODE")]
        public string IACode {get;set;}

        [Column("NATIS_NUMBER")]
        public long? NatisNumber {get;set;}

        [Column("NATIS_SEQUENCE_NAME")]
        public string NatisSequenceName {get;set;}

        [Column("OFFICE_HOURS")]
        public string OfficeHours {get;set;}

        [Column("SECTION67_PRE")]
        public string Sectiontion67Pre { get; set; }

        [Column("SECTION67_POST")]
        public string Section67Post { get; set; }

        [Column("J175_PRINTING")]
        public long? J175Printing {get;set;}

        [Column("CENTRAL_CAPTURE_INDICATOR")]
        public long? CentralCaptureIndicator {get;set;}

        [Column("ACTIVE_INDICATOR")]
        public long ActiveIndicator {get;set;}

        [Column("EMAIL_ADDRESS")]
        public string EmailAddress {get;set;}

        [Column("I_TICKET_PRE_56")]
        public string ITicketPre56 {get;set;}

        [Column("I_TICKET_SEQ_NAME_56")]
        public string ITicketSeqName56 {get;set;}

        [Column("I_TICKET_PRE_341")]
        public string ITicketPre341 {get;set;}

        [Column("I_TICKET_SEQ_NAME_341")]
        public string ITicketSeqName341 {get;set;}

        [Column("BRANCH_NAME")]
        public string BranchName {get;set;}

        [Column("PAYMENT_OPTIONS")]
        public string PaymentOptions { get; set; }

        [Column("SITE_LOGO")]
        public Byte[] SiteLogo { get; set; }

        [ForeignKey("RegionID")]
        public virtual Region Region { get; set; }
    }
}
