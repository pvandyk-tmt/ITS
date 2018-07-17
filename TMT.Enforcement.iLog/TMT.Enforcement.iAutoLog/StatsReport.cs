using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TMT.Core.Camera.Base;
using Microsoft.Reporting.WinForms;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace TMT.Enforcement.iAutoLog
{
    public partial class StatsReport : Form
    {
        private List<ReportSource> _rs = new List<ReportSource>();

        public StatsReport()
        {
            InitializeComponent();
        }

        public void BindFormData(cFilm film)
        {
            string statsFileName;

            if (film.pHasEncryptedPictureFiles)
            {
                film.applyStats();

                //*****First Record is the Operator and Stats
                ReportSource rsMain = new ReportSource 
                {
                    AverageSpeed = film.pAverageSpeed, 
                    CountOfInfringements = film.pInfringements, 
                    ErrorsCount = film.pCaptureErrors, 
                    HighestSpeed = film.pHighestSpeed, 
                    JammerCount = film.pJammerCount, 
                    TestPhotoCount = film.pTestPhotos, 
                    VehiclesChecked = film.pVehiclesChecked
                };

                statsFileName = film.pStatsFileName == string.Empty ? "None" : film.pStatsFileName;

                if (film.pStartDate != null && film.pEndDate != null)
                {
                    rsMain.Time = film.pStartDate.Value.ToString("HH:mm:ss tt") + "-" + film.pEndDate.Value.ToString("HH:mm:ss tt");
                }

                _rs.Add(rsMain);

                int locationCounter = 0;

                var groupByDate = from f in film.getPicturesFiles()
                                      where f.pHasError == false
                                      group f by f.pOffenceDateStringDD_MM_YYYY into g
                                      select g;
                
                List<string> lst = new List<string>();
                List<string> camId = new List<string>();
                foreach (IGrouping<string, cPictureFile> pictureFilesDate in groupByDate)
                {
                    var groupByLocation = from f in pictureFilesDate
                                          where f.pHasError == false
                                          group f by f.pLocationCode into g
                                          select g;

                    foreach (IGrouping<string, cPictureFile> pictureFilesLocation in groupByLocation)
                    {
                        locationCounter++;
                        ReportSource rs = new ReportSource { CamDate = pictureFilesLocation.First().pOffenceDateStringDD_MM_YYYY, Session = "Actual", LocationCode = pictureFilesLocation.First().pLocationCode };

                        foreach (cPictureFile pictureFile in pictureFilesLocation)
                        {
                            if (!lst.Contains(pictureFile.pOperatorId))
                            {
                                lst.Add(pictureFile.pOperatorId);
                            }

                            if (pictureFile.pIsTest)
                            {
                                rs.TestPhotoCount++;
                            }
                            else if (pictureFile.pIsJammer)
                            {
                                rs.JammerCount++;
                            }
                            else if (pictureFile.pHasError)
                            {
                                rs.ErrorsCount++;
                            }
                            else
                            {
                                rs.CountOfInfringements++;
                            }

                            if (!camId.Contains(pictureFile.pMachineId))
                            {
                                camId.Add(pictureFile.pMachineId);
                            }  
                        }

                        DateTime? start = pictureFilesLocation.First().pOffenceDate;
                        DateTime? end = pictureFilesLocation.Last().pOffenceDate;
                        if (start != null && end != null)
                        {
                            rs.Time = start.Value.ToString("HH:mm:ss tt") + "-" + end.Value.ToString("HH:mm:ss tt");
                        }

                        _rs.Add(rs);
                    }
                }
                
                StringBuilder sb = new StringBuilder();
                foreach (string s in lst)
                {
                    sb.Append(s);
                    sb.Append(",");
                }

                rsMain.Operators = sb.ToString().TrimEnd(',');

                cPictureFile pic = film.getFirstValidPictureFile();

                if (pic != null)
                {
                    rsMain.LocationCode = locationCounter > 1 ? "*VARIOUS" : pic.pLocationCode;
                    rsMain.CamDate = pic.pOffenceDateStringDD_MM_YYYY;
                    rsMain.Session = pic.pFormattedSession;
                }

                sb.Clear();
                foreach (string c in camId)
                {
                    sb.Append(c);
                    sb.Append(",");
                }

                rsMain.CamId = sb.ToString().TrimEnd(',');

                rsMain.StatsFileName = string.IsNullOrWhiteSpace(statsFileName) ? "No  File" : statsFileName;
            }
        }

        private void StatsReport_Load(object sender, EventArgs e)
        {
            try
            {
                // Set the processing mode for the ReportViewer to Local
                reportViewerMain.ProcessingMode = ProcessingMode.Local;

                LocalReport localReport = reportViewerMain.LocalReport;

                localReport.ReportPath = @"Reports\AutoLogStatsReport.rdlc";

                localReport.DataSources[0].Value = _rs;

                // Refresh the report
                reportViewerMain.LocalReport.Refresh();

                reportViewerMain.RefreshReport();
                
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        public void Save(string fileName)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;
            string deviceInf = "";
            //byte[] bytes;
            //string extension;

            string directoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            string exe = Process.GetCurrentProcess().MainModule.FileName;
            string path = Path.GetDirectoryName(exe); 

            reportViewerMain.ProcessingMode = ProcessingMode.Local;

            LocalReport localReport = reportViewerMain.LocalReport;

            localReport.ReportPath = string.Format("{0}\\{1}", path, "Reports\\AutoLogStatsReport.rdlc");
            localReport.Refresh();
            localReport.DataSources[0].Value = _rs;

            byte[] pdfReport = localReport.Render("pdf", deviceInf, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                fs.Write(pdfReport, 0, pdfReport.Length);
            }

        }
    }
}