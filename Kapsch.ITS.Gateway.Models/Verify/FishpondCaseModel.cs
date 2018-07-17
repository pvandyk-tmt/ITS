using System.Collections.Generic;

namespace Kapsch.ITS.Gateway.Models.Verify
{
    public class FishpondCaseModel
    {
        public IList<CaptureTypeModel> CaptureTypes { get; set; }
        public CaseModel Case { get; set; }
    }
}
