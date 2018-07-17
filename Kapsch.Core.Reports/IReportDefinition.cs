using Kapsch.Core.Reports.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Reports
{
    public interface IReportDefinition
    {
        byte[] Export(ExportType exportType, string[] parameters);
        string RequiredAccessRole { get; }
        string CategoryName { get; }
        string SubCategoryName { get; }
        string ReportName { get; }
        IList<ParameterType> ParameterTypes { get; }
        IList<ExportType> ExportTypes { get; }
    }
}
