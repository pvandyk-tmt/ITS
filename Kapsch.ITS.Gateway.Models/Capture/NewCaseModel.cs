using System.Collections.Generic;

namespace Kapsch.ITS.Gateway.Models.Capture
{
    public class NewCaseModel
    {
        public IList<CaptureTypeModel> CaptureTypes { get; set; }
        public CaseModel Case { get; set; }
    }
}
