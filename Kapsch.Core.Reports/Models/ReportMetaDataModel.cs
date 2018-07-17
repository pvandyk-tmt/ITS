using Kapsch.Core.Reports.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Kapsch.Core.Reports.Models
{
    public class ReportMetaDataModel
    {
        public ReportMetaDataModel()
        {
            ReportCategories = new List<ReportCategoryModel>();
        }

        // TODO Can not be the most efficient way
        public void ApplyFilterAndOrder(IList<string> accessRoles)
        {

            foreach (var reportCategory in ReportCategories)
            {
                foreach (var reportSubCategory in reportCategory.ReportSubCategories)
                {
                    reportSubCategory.ReportDefinitions = 
                        reportSubCategory.ReportDefinitions
                            .Where(f => accessRoles.Contains(f.RequiredAccessRole))
                            .ToList();
                }

                reportCategory.ReportSubCategories = 
                    reportCategory.ReportSubCategories
                        .Where(f => f.ReportDefinitions.Count != 0)
                        .OrderBy(f => f.SubCategoryName)
                        .ToList();
            }

            ReportCategories = 
                ReportCategories
                    .Where(f => f.ReportSubCategories.Count != 0)
                    .OrderBy(f => f.CategoryName)
                    .ToList();
        }

        public IList<ReportCategoryModel> ReportCategories { get; set; }       
    } 
}
