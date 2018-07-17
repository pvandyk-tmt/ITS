
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using TMT.Enforcement.iLog.Persistence;
using TMT.Core.Camera.RedRoom;
using TMT.Enforcement.ErrorWriting;
using TMT.Core.Camera.Base;
using System.IO;
using System.Windows.Forms;
using System.Net.Mail;
using TMT.Enforcement.iAutoLog.Properties;
using System.Threading;
using System.Diagnostics;

namespace TMT.Enforcement.iAutoLog
{
    public class ProcessJob : IJob
    {
        private string mImagePath;
        private string SavedStatsFilePath;
        private cPhysicalStudio mPhysicalStudio;
        private int mUserId;
        private ErrorLogging errorWriting;
        private cDataAccess dataAccess;
        private DateTime serverDateTime;
        private int retryCounter = 0;

        public void Execute(IJobExecutionContext context)
        {
            errorWriting = new ErrorLogging();
            try
            {
                retryCounter += 1;
                errorWriting.WriteErrorLog("iAutoLog Job has started on machine " + Environment.MachineName);
                var configurationCore = new TMT.Core.Camera.Interfaces.Configuration();

                var redRoomConfiguration = new cConfiguration();

                List<cCamera> cameras = redRoomConfiguration.GetCameras(Path.Combine(DefaultPath(), "Config"), "CameraConfig.xml");
                mPhysicalStudio = new cPhysicalStudio();
                
                // mPhysicalStudio.evFilmAdded += mPhysicalStudioFilmAdded;
                foreach (cCamera abstractCamera in cameras)
                {
                    mPhysicalStudio.Register(abstractCamera);
                }

                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["CoreContext"];

                dataAccess = new cDataAccess(setting.ConnectionString);
                errorWriting.WriteErrorLog("Get Server Time.");
                serverDateTime = dataAccess.GetDatabaseDate();

                dataAccess.evNewCaseLoggedError += data_NewCaseLoggedError;

                bool success = dataAccess.getImagePath(Environment.MachineName, out mImagePath);
                if (!success)
                {
                    errorWriting.WriteErrorLog("Invalid Image path. Please check the computer name!");
                }
            
                errorWriting.WriteErrorLog("StartProcessing");
                StartProcessing();
            }
            catch(Exception ex)
            {
                errorWriting.WriteErrorLog(ex.Message);
                if (retryCounter <= 2) 
                {
                    Thread.Sleep(60000);
                    Execute(context);
                }
            }
        }

        private void StartProcessing()
        {

            ListView lvUnloggedDirectoryList = new ListView();
            lvUnloggedDirectoryList = GetUnLoggedDirectoryList();
            try
            {
                if (lvUnloggedDirectoryList != null && lvUnloggedDirectoryList.Items.Count > 0)
                {
                    errorWriting.WriteErrorLog("Directory count to be processed: " + lvUnloggedDirectoryList.Items.Count.ToString());
                    foreach (ListViewItem lsvItem in lvUnloggedDirectoryList.Items)
                    {
                        errorWriting.WriteErrorLog(string.Format("Processing file Location : {0}", lsvItem.SubItems[1].Text));
                        ProcessInfringementList(lsvItem);
                    }
                }
                else
                {
                    errorWriting.WriteErrorLog("No infringements found to process");
                }
            }
            finally
            {
                errorWriting.WriteErrorLog(String.Format("The AutoiLog has completed for the day on machine {0}. \r\n The stats reports can be found in {1} on machine {0} .",
                    Environment.MachineName, SavedStatsFilePath, System.DateTime.Now.Date.ToString("yyyyMMdd")));
                
                SendMail("AutoiLog log files", String.Format("The AutoiLog has completed for the day on machine {0}. \r\n The stats reports can be found in {1} on machine {0} .", 
                    Environment.MachineName, SavedStatsFilePath, System.DateTime.Now.Date.ToString("yyyyMMdd")));
                dataAccess.disconnect();
            }
        }

        private ListView GetUnLoggedDirectoryList()
        {
            string CameraFilePath = ConfigurationManager.AppSettings["CameraFilePath"];

            if (CameraFilePath.Length <= 0)
            {
                errorWriting.WriteErrorLog("Camera File path is not properly configured in config file");
                return null;
            }

            var dirSelected = new DirectoryInfo(CameraFilePath);
            if (!dirSelected.Exists)
            {
                errorWriting.WriteErrorLog(CameraFilePath + ": This Camera File path does not exist.");
                return null;
            }

            ListView unloggedList = new ListView();
            try
            {
                mPhysicalStudio.Path = CameraFilePath;

                List<cFilm> films = mPhysicalStudio.GetDevelopableFilms(null);
                if (films.Count == 0)
                {
                    var sb = new StringBuilder();
                    sb.Append("The system could not decrypt files in the selected path.");
                    sb.Append(Environment.NewLine);
                    sb.Append("Possible Reasons:");
                    sb.Append(Environment.NewLine);
                    sb.Append("1. No Files could be found, check the filter or if the path contains files.");
                    sb.Append(Environment.NewLine);
                    sb.Append("2. The files exists but are corrupted and cannot be decrypted.");
                    sb.Append(Environment.NewLine);
                    sb.Append("3. Invalid deskey or deskey not found.");

                    errorWriting.WriteErrorLog(sb.ToString());

                    return null;
                }

                foreach (cFilm film in films)
                {
                    ListViewItem item = new ListViewItem(film.pGroup);

                    int result;
                    dataAccess.checkLoggedSessions(film, out result);


                    if (result != 1)
                    {
                        if (!RemoveOldFiles(film.pPath))
                        {
                            item.Tag = film;
                            item.SubItems.Add(film.pPath);
                            errorWriting.WriteErrorLog(string.Format("File directory: {0} on machine: {1} is ready to be processed.", film.pPath, Environment.MachineName));
                            unloggedList.Items.Add(item);
                        }
                    }
                    else
                    {
                        //Lets delete old directories here
                        RemoveOldFiles(film.pPath);
                    }
                }
                return unloggedList;
            }

            finally
            {

            }
        }

        private bool RemoveOldFiles(string path)
        {
            string[] files = Directory.GetFiles(path);
            
            foreach (string file in files)
            {
                DirectoryInfo parent = Directory.GetParent(file);

                if (parent != null)
                {
                    //It might be that the file has zero length
                    long length = new System.IO.FileInfo(file).Length;
                    if (length == 0)
                    {
                        continue;
                    }
                        
                    DateTime? lastWrite;
                    try
                    {                       
                        lastWrite = parent.LastWriteTime;
                    }
                    catch
                    {
                        lastWrite = null;
                    }

                    if (lastWrite == null || (lastWrite.Value.Date < serverDateTime.AddDays(-32)))
                    {
                        errorWriting.WriteErrorLog(string.Format("Deleting file directory : {0} on machine: {1}", path, Environment.MachineName));
                        Directory.Delete(path, true);
                        return true;
                    }
                }
            }
            return false;
        }

        private void SendMail(string subject, string body)
        {
            Mail mail = new Mail(Settings.Default.SmtpHost, Settings.Default.SmtpPort, Settings.Default.SmtpUserName,
                                        Settings.Default.SmtpPassword, Settings.Default.SmtpMailFrom, Settings.Default.MailDistributionList,
                                        Settings.Default.MailEnableSsl, Settings.Default.SmtpUseDefaultCredentials);

            Attachment attachment1 = null;
            Attachment attachment2 = null;

            string fileName = Path.Combine(Settings.Default.LogFileFolder, String.Format("{0:yyyyMMdd}", DateTime.Now) + "_LogFile.txt");
            
            if (File.Exists(fileName))
            {
                attachment1 = new Attachment(fileName);                
            }

            fileName = string.Empty;
            fileName = Path.Combine(Settings.Default.LogFileFolder, String.Format("{0:yyyyMMdd}", DateTime.Now) + "_ExceptionLogFile.txt");
            if (File.Exists(fileName))
            {
                attachment2 = new Attachment(fileName);
            }
            mail.sendMail(subject, body, attachment1, attachment2);
        }

        private void ProcessInfringementList(ListViewItem unLoggedDirectoryItem)
        {           
            if (unLoggedDirectoryItem != null)
            {
                var film = (cFilm)unLoggedDirectoryItem.Tag;

                Log_Case(unLoggedDirectoryItem);
            }           
        }

        private void Log_Case(ListViewItem sessionItem)
        {
            int totalLoggedCases = 0;
            int totalPreviouslyLogged = 0;
            mUserId = 2;  //AutoiLog user on Credentials
            SavedStatsFilePath = ConfigurationManager.AppSettings["SavedStatsFilePath"];
            SavedStatsFilePath = SavedStatsFilePath + "\\" + serverDateTime.Date.ToString("yyyyMMdd"); 

            Directory.CreateDirectory(SavedStatsFilePath);
            cFilm film = new cFilm();

            try
            {
                if (sessionItem != null)
                {
                    film = (cFilm)sessionItem.Tag;

                    int numberLoggedCases;
                    int previouslyLogged;
                    cFilm reportFilm;
                    if (dataAccess.logNewCases(film, mUserId, mImagePath, out numberLoggedCases, out reportFilm, out previouslyLogged))
                    {
                        totalLoggedCases += numberLoggedCases;
                        totalPreviouslyLogged += previouslyLogged;

                        if (totalPreviouslyLogged > 0)
                        {
                            errorWriting.WriteErrorLog(string.Format("Total Infringements logged {0} out of {1} and Total Previously logged {2} out of {1}", totalLoggedCases, film.pEncryptedPictureFileCollection.Count(), totalPreviouslyLogged));
                        }
                        else
                        {                              
                            errorWriting.WriteErrorLog(string.Format("Total Infringements logged {0} out of {1}", totalLoggedCases, film.pEncryptedPictureFileCollection.Count()));
                        }

                        if (numberLoggedCases > 0)
                        {
                            //Check if session is missing any files for SAfETYCAM infringements only
                            if (film.pCameraDriver.pName.ToUpper() == "SAFETYCAM")
                            {
                                checkForValidFilesInSession(film);
                            }

                            string date = DateTime.Now.ToString("yyyyMMdd HHmmss");

                            //Generate stats file here and save it.                        
                            var sf = new StatsReport();
                            sf.BindFormData(reportFilm);
                            sf.Save(String.Format("{0}\\{1}-{2}.pdf", 
                                SavedStatsFilePath, 
                                sessionItem.Text,
                                date));

                            //Process vosi list
                            dataAccess.logVosiFile(film);

                            if (!string.IsNullOrEmpty(film.pStatsFileName))
                            {
                                FileInfo fileInfo = new FileInfo(film.pStatsFileName);
                                var encStatsFileList = dataAccess.ReadStatsFile(fileInfo);
                                dataAccess.SubmitStatsFile(encStatsFileList);
                            }

                            Application.DoEvents();

                            int sessionLogged;
                            dataAccess.checkLoggedSessions(film, out sessionLogged);
                        }
                    } 

                    if (dataAccess.pError.Length > 0)
                    {
                        if (film.getFirstValidPictureFile() != null)
                        {
                            errorWriting.WriteErrorLog("ERROR with logging film in path:" + film.pPath + "; Session: " + film.getFirstValidPictureFile().pFormattedSession + "; Error:  " + dataAccess.pError);
                        }
                        else
                        {
                            errorWriting.WriteErrorLog("ERROR with logging film <SESSION READ ERROR> - " + dataAccess.pError);
                        }
                    }

                    mPhysicalStudio.ClearFilm(film);
                }
            }
            catch(Exception ex)
            {
                dataAccess.ExceptionLog(Environment.MachineName, ex.Message + " Inner: " + ex.InnerException, film.getFirstValidPictureFile().pFormattedSession,
                film.getFirstValidPictureFile().pOffenceDate, film.getFirstValidPictureFile().pLocationCode, film.pPath, "");

                errorWriting.WriteErrorLog(ex.Message + " Inner: " + ex.InnerException, "Session: " + film.getFirstValidPictureFile().pFormattedSession +
                    "; OffenceDate: " + film.getFirstValidPictureFile().pOffenceDate +
                    "; FilePath: " + film.pPath);

            }
        }

        private void data_NewCaseLoggedError(object sender, cPictureFile pictureFile, string errorDescription)
        {
            dataAccess.ExceptionLog(Environment.MachineName, errorDescription, pictureFile.pFormattedSession,
                pictureFile.pOffenceDate, pictureFile.pLocationCode,
                pictureFile.pEncryptedPicture.pEncryptedFilePath, pictureFile.pEncryptedPicture.pEncryptedFileName);

            errorWriting.WriteErrorLog(errorDescription, "Session: " + pictureFile.pFormattedSession +
                "; OffenceDate: " + pictureFile.pOffenceDateStringYYYYMMDD +
                "; Location: " + pictureFile.pLocationCode +
                "; FilePath: " + pictureFile.pEncryptedPicture.pEncryptedFilePath +
                "; FileName: " + pictureFile.pEncryptedPicture.pEncryptedFileName);
        }

        private void checkForValidFilesInSession(cFilm film)
        {
            cPictureFile pictureFile = new cPictureFile();

            string result = string.Empty;

            string[] pdfFile = Directory.GetFiles(film.pPath, "*.pdf");

            if (pdfFile == null || pdfFile.Length == 0)
                result = "No Fieldsheet. ";

            string[] statsFile = Directory.GetFiles(film.pPath, "stat*.txt");

            if (statsFile == null || statsFile.Length == 0)
                result = result + "Enc files can not be validated against stats file as stats file missing. ";

            if ((statsFile != null || statsFile.Length >= 0) && (film.pTestPhotos + film.pInfringements != film.pEncryptedPictureFileCollection.Count))
                result = result + "Enc files do not match stats file";

            if (result != string.Empty)
            {
                pictureFile = film.getFirstValidPictureFile();

                dataAccess.ExceptionLog(Environment.MachineName, result, pictureFile.pFormattedSession,
                   pictureFile.pOffenceDate, pictureFile.pLocationCode,
                   pictureFile.pEncryptedPicture.pEncryptedFilePath, pictureFile.pEncryptedPicture.pEncryptedFileName);

                errorWriting.WriteErrorLog(result, "Session: " + pictureFile.pFormattedSession +
                    "; OffenceDate: " + pictureFile.pOffenceDateStringYYYYMMDD +
                    "; Location: " + pictureFile.pLocationCode +
                    "; FilePath: " + pictureFile.pEncryptedPicture.pEncryptedFilePath +
                    "; FileName: " + pictureFile.pEncryptedPicture.pEncryptedFileName);
            }
        }

        private string DefaultPath()
        {
            string exe = Process.GetCurrentProcess().MainModule.FileName;
            string path = Path.GetDirectoryName(exe);

            return path;
            ////return @"C:\TMT\Camera";
            //if (Environment.Is64BitOperatingSystem)
            //{
            //    return @"C:\Program Files (x86)\TMT\iAutoLog";
            //}
            //else
            //{
            //    return @"C:\Program Files\TMT\iAutoLog";
            //}
        }        
    }
}
