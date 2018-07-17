using Microsoft.Reporting.WebForms;

namespace Kapsch.EVR.Reports
{
    public class ReportViewerBase
    {
        protected byte[] StreamPdfReport(ReportViewer reportViewer)
        {
            Warning[] Warnings;
            string[] strStreamIds;
            string strMimeType;
            string strEncoding;
            string strFileNameExtension;

            return reportViewer.LocalReport.Render("PDF", null, out strMimeType, out strEncoding, out strFileNameExtension, out strStreamIds, out Warnings);
        }

        protected byte[] StreamExcelReport(ReportViewer reportViewer)
        {
            Warning[] Warnings;
            string[] strStreamIds;
            string strMimeType;
            string strEncoding;
            string strFileNameExtension;

            return reportViewer.LocalReport.Render("Excel", null, out strMimeType, out strEncoding, out strFileNameExtension, out strStreamIds, out Warnings);
        }
    }
}
