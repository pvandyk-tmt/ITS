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
    [Table("COURT_DETAILS", Schema = "COURT")]
    public class Court
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("COURT_NAME")]
        public string CourtName {get;set;}

        [Column("ADDRESS_INFO_ID")]
        public long? AddressInfoID {get;set;}

        [Column("PERSON_INFO_ID")]
        public long? PersonInfoID {get;set;}

        [Column("CONTEMPT_AMOUNT")]
        public long? ContemptAmount { get; set; }

        [Column("CONTEMPT_DAYS")]
        public long? ContemptDays { get; set; }

        [Column("BANKING_INFO_ID")]
        public long? BankingInfoID { get; set; }

        [Column("DISTRICT_ID")]
        public long DistrictID {get;set;}

        [Column("CASE_PRE")]
        public string CasePre {get;set;}

        [Column("CASE_POST")]
        public string CasePost {get;set;}

        [Column("SEQUENCE_NAME")]
        public string SequenceName {get;set;}

        [Column("STATUS_ID")]
        public Status Status {get;set;}

        [Column("WARRANT_PRE")]
        public string WarrantPre {get;set;}

        [Column("WARRANT_POST")]
        public string WarrantPost {get;set;}

        [Column("CAPTURE_DATE")]
        public DateTime CaptureDate {get;set;}

        [Column("TYPE_OF_SERVICE_ALLOWED")]
        public long? TypeOfServiceAllowed {get;set;}

        [Column("WARRANT_LETTER_GRACE")]
        public long? WarrantLetterGrace { get; set; }

        [Column("WARRANT_EXPIRE_DAYS")]
        public long? WarrantExpireDays { get; set; }

        [Column("SUMMONS_EXPIRE_DAYS")]
        public long? SummonsExpireDays { get; set; }

        [Column("CREDENTIAL_ID")]
        public long? CredentialID {get;set;}

        [Column("COURT_TIME")]
        public string CourtTime {get;set;}

        [Column("DAYS_TO_COURT_DATE")]
        public long? DaysToCourtDate { get; set; }

        [Column("OVER_ALLOCATION")]
        public long? OverAllocation { get; set; }

        [Column("RE_ISSUE_INVALID_SERVING")]
        public long ReIssueInvalidServing { get; set;}

        [ForeignKey("DistrictID")]
        public virtual District District { get; set; }

        [ForeignKey("AddressInfoID")]
        public virtual AddressInfo AddressInfo { get; set; }

        public virtual IList<CourtRoom> CourtRooms { get; set; }
        public virtual IList<CourtDate> CourtDates { get; set; }
    }
}
