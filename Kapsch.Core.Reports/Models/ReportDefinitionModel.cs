using Kapsch.Core.Reports.Enums;
using System.Collections.Generic;

namespace Kapsch.Core.Reports.Models
{
    public class ReportDefinitionModel
    {
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string ReportName { get; set; }
        public string RequiredAccessRole { get; set; }
        public IList<ParameterType> ParameterTypes { get; set; }
        public IList<ExportType> ExportTypes { get; set; }
    }
}
