using Kapsch.Core.Reports.Enums;
using System.Collections.Generic;

namespace Kapsch.Core.Reports.Models
{
    public class ReportSubCategoryModel
    {
        public ReportSubCategoryModel()
        {
            ReportDefinitions = new List<ReportDefinitionModel>();
        }

        public string SubCategoryName { get; set; }
        public IList<ReportDefinitionModel> ReportDefinitions { get; set; }
    }
}
