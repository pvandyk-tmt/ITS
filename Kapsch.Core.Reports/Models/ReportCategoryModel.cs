using System.Collections.Generic;

namespace Kapsch.Core.Reports.Models
{
    public class ReportCategoryModel
    {
        public ReportCategoryModel()
        {
            ReportSubCategories = new List<ReportSubCategoryModel>();
        }

        public string CategoryName { get; set; }
        public IList<ReportSubCategoryModel> ReportSubCategories { get; set; }
    }
}
