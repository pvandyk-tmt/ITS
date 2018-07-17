using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Capture
{
    public class AcceptCaseModel
    {
        public SessionModel Session { get; set; }
        public int OffenceSetID { get; set; }
        public int FileNumber { get; set; }
        public int NextFileNumber { get; set; }
        public int OfficerID { get; set; }
        public string SheetNumber { get; set; }
        public bool HasSheetNumberChanged { get; set; }
        public CaseModel CaseInfo { get; set; }
        public CaptureTypeModel CaptureType { get; set; }
        public int PrintImageID { get; set; }
        public string ComputerName { get; set; }
    }
}
