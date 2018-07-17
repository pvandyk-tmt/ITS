#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Authentication;
using System.Text;
using System.Xml.Schema;
using TMT.Core.Camera.Base;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using Encoder = System.Drawing.Imaging.Encoder;
using System.Globalization;
using TMT.Enforcement.ErrorWriting;
using TMT.Enforcement.iLog.Persistence.Models.Enc;
using TMT.Enforcement.iLog.Persistence.OracleTableTypeClasses;

#endregion

namespace TMT.Enforcement.iLog.Persistence
{
    public class cDataAccess
    {
        #region Delegates

        public delegate void NewCaseLoggedDelegate(object sender, cPictureFile pictureFile, int fileNumber, int numberLogged);

        public delegate void NewCaseLoggedErrorDelegate(object sender, cPictureFile pictureFile, string errorDescription);

        public delegate void NewCasePreviouslyLoggedDelegate(object sender, cFilm film, cEncryptedPictureFile encryptedPictureFile);

        public delegate void ValidateImageDelegate(object sender, sLoggedImage img, bool isMissing);

        //public delegate void NewCaseAutoLoggedErrorDelegate(object sender, cPictureFile pictureFile, string errorDescription);

        #endregion

        public string databaseName;
        private readonly string mConnectionString = string.Empty;
        private OracleConnection mDbConnection;
        private string mError = string.Empty; // Latest error message
        private ErrorLogging errorWriting;
                
        public cDataAccess(string connectionString)
        {
            mConnectionString = connectionString;
           // errorWriting = new ErrorLogging();
        }

        #region ----- Connect, disconnect, authorise, errorclean -----

        /// <summary>
        /// 	Make original connection to DB.
        /// </summary>
        /// <returns>True if all okay</returns>
        public bool connect()
        {
            mError = string.Empty;

            #if (!DEBUG)
                if ((string.IsNullOrEmpty(mConnectionString)) || (mConnectionString == "Empty"))
                {
                    mError = "Database connection string in config file is incorrect.";
                    return false;
                }
            #endif

            try
            {
                mDbConnection = new OracleConnection(mConnectionString);
                
                databaseName = mDbConnection.DataSource;

                mDbConnection.Open();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                errorWriting = new ErrorLogging();
                errorWriting.WriteErrorLog(ex);
                mDbConnection = null;
            }

            return (mDbConnection != null);
        }

        /// <summary>
        /// 	Close DB connection.
        /// </summary>
        public void disconnect()
        {
            if (mDbConnection != null)
            {
                mDbConnection.Close();
                mDbConnection.Dispose();
            }

            mDbConnection = null;
        }
        
        public bool validateUser(string username, string password, out int userId)
        {
            bool okay = true;
            userId = 0;

            if (!connect())
                return false;

            try
            {
                var cmd = new OracleCommand("credentials.credential_data.validate_user", mDbConnection) {CommandType = CommandType.StoredProcedure};

                // Inputs
                OracleParameter par = new OracleParameter("p_user_name", username) {Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2};
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_password", password) {Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2};
                cmd.Parameters.Add(par);

                par = new OracleParameter("o_result", userId) {Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Decimal};
                cmd.Parameters.Add(par);

                cmd.ExecuteNonQuery();

                if (!(cmd.Parameters["o_result"].Value is DBNull))
                {
                    var result = (OracleDecimal) cmd.Parameters["o_result"].Value;
                    userId = (int) result;
                }

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                okay = false;
                errorWriting = new ErrorLogging();
                errorWriting.WriteErrorLog(ex);
            }

            disconnect();

            return okay;
        }

        public string getUserDetail(int userId)
        {
            bool okay = true;
            string userDetail = "";

            if (!connect())
                return userDetail;

            try
            {
                var cmd = new OracleCommand("its.lookups.get_employee_person_detail", mDbConnection) { CommandType = CommandType.StoredProcedure };

                // Inputs
                OracleParameter par = new OracleParameter("p_user_id", userId) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Decimal };
                cmd.Parameters.Add(par);
                                
                cmd.Parameters.Add(new OracleParameter("o_results", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "o_results"));
                
                cmd.ExecuteNonQuery();
                if (!(cmd.Parameters["o_results"].Value is DBNull))
                {
                    var curs = (OracleRefCursor)cmd.Parameters["o_results"].Value;

                    OracleDataReader rd = curs.GetDataReader();

                    while (rd.Read())
                    {
                        userDetail = string.Format("{0} - {1} {2}", rd.GetString(0), rd.GetString(1), rd.GetString(2));
                    }
                }
                    
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                okay = false;
                errorWriting = new ErrorLogging();
                errorWriting.WriteErrorLog(ex);
            }

            disconnect();
            return userDetail;
        }

        public bool getImagePath(string computerName, out string imagePath)
        {
            bool okay = true;
            imagePath = "";

            if (!connect())
                return false;

            try
            {
                var cmd = new OracleCommand("its.offence_logging.get_image_path", mDbConnection) { CommandType = CommandType.StoredProcedure };
                // Inputs
                OracleParameter par = new OracleParameter("p_computer_name", computerName) {Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2};
                cmd.Parameters.Add(par);

                par = new OracleParameter("o_path", imagePath) {Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Varchar2, Size = 1024};
                cmd.Parameters.Add(par);

                cmd.ExecuteNonQuery();

                if (!(cmd.Parameters["o_path"].Value is DBNull))
                {
                    var result = (OracleString) cmd.Parameters["o_path"].Value;
                    imagePath = result.ToString();
                }

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                errorWriting = new ErrorLogging();
                errorWriting.WriteErrorLog(ex);
                mError = ex.Message;
                okay = false;
            }

            disconnect();

            return okay;
        }

        public bool syncImages(string computerName, cFilm film, out List<sLoggedImage> missingImages)
        {
            bool okay = true;
            missingImages = new List<sLoggedImage>();

            if (!film.pHasEncryptedPictureFiles)
            {
                mError = "The film does not contain any encrypted picture files.";
                return false;
            }

            if (film.pHasErrors)
            {
                mError = "The first picture in this collection of pictures cannot be developed. Please remove it and reload this list.";
                return false;
            }

            if (!connect())
                return false;

            try
            {
                //Apply the stats
                film.applyStats();

                var cmd = new OracleCommand("its.offence_logging.get_existing_cases", mDbConnection) { CommandType = CommandType.StoredProcedure };

                cPictureFile pic = film.getFirstValidPictureFile();

                // Inputs
                OracleParameter par = new OracleParameter("p_computer_name", computerName) {Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2};
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_grouping_type", (int)film.pGroupType) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Decimal };
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_machine_id", pic.pMachineId) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.NVarchar2 };
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_cam_date", pic.pOffenceDate) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Date };
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_num_days", film.pNumDays) {Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Int32};
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_cam_session", xmlClean(pic.pFormattedSession)) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2 };
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_location_code", xmlClean(pic.pLocationCode)) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2 };
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("o_results", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "o_results"));

                cmd.ExecuteNonQuery();

                //Clear the film - make sure no pictures - will affect reporting - only add pictures that are not logged
                film.clearFilm();

                if (!(cmd.Parameters["o_results"].Value is DBNull))
                {
                    var curs = (OracleRefCursor) cmd.Parameters["o_results"].Value;

                    OracleDataReader rd = curs.GetDataReader();
                    sLoggedImage img;

                    while (rd.Read())
                    {
                        img.mFileName = rd.GetString(0);
                        img.mImagePath = rd.GetString(1);
                        img.mLogDate = rd.GetString(2);
                        img.mOffenceDate = rd.GetString(3);
                        img.mDvdSession = rd.GetString(4);
                        img.mMachineId = rd.GetString(5);
                        img.mLocationCode = rd.GetString(6);
                        img.mSession = rd.GetString(7);
                        img.mFileNumber = rd.GetDecimal(8);

                        img.mHasError = false;
                        img.mMessage = "";

                        bool isMissing = false;

                        string imagePathCheck = string.Empty;
                        try
                        {
                            imagePathCheck = Path.Combine(imagePathCheck, img.mImagePath);
                            imagePathCheck = Path.Combine(imagePathCheck, img.mLogDate); 
                            imagePathCheck = Path.Combine(imagePathCheck, img.mOffenceDate);
                            imagePathCheck = Path.Combine(imagePathCheck, img.mDvdSession);
                            imagePathCheck = Path.Combine(imagePathCheck, img.mMachineId);
                            imagePathCheck = Path.Combine(imagePathCheck, img.mLocationCode);
                            imagePathCheck = Path.Combine(imagePathCheck, img.mSession);
                            imagePathCheck = Path.Combine(imagePathCheck, img.mFileName);
                        }
                        catch (Exception ex)
                        {
                            imagePathCheck = string.Empty;
                        }

                        if (!File.Exists(imagePathCheck))
                        {
                            isMissing = true;
                            missingImages.Add(img);

                            //Now Sync...
                            foreach (cEncryptedPictureFile encryptedPictureFile in film.getEncryptedPictureFiles())
                            {
                                if (encryptedPictureFile.encryptedFileNumberDb == img.mFileNumber)
                                {
                                    cPictureFile pictureFile = film.getPictureFile(encryptedPictureFile.pEncryptedFileName);

                                    if (pictureFile == null)
                                    {
                                        img.mHasError = true;
                                        img.mMessage = "Picture Error - No picture exists (Null)";
                                        break;
                                    }

                                    if (!pictureFile.pHasDecryptedData)
                                    {
                                        img.mHasError = true;
                                        img.mMessage = "Picture File Error - Cannot decrypt the data in the picture";
                                        break;
                                    }

                                    if (pictureFile.pHasError)
                                    {
                                        img.mHasError = true;
                                        StringBuilder sb = new StringBuilder();
                                        foreach (string s in pictureFile.pErrorCollection)
                                        {
                                            sb.Append(s);
                                            sb.Append(",");
                                        }

                                        img.mMessage = "Picture Error - " + sb.ToString().TrimEnd(',');
                                        break;
                                    }

                                    string path = img.mImagePath + img.mLogDate;

                                    try
                                    {
                                        makeDir(path);
                                        path = Path.Combine(path, img.mOffenceDate);
                                        makeDir(path);
                                        path = Path.Combine(path, img.mDvdSession);
                                        makeDir(path);
                                        path = Path.Combine(path, img.mMachineId);
                                        makeDir(path);
                                        path = Path.Combine(path, img.mLocationCode);
                                        makeDir(path);
                                        path = Path.Combine(path, img.mSession);
                                        makeDir(path);
                                    }
                                    catch (Exception ex)
                                    {
                                        //Stop Process if the Paths cannot be created!!
                                        mError = ex.Message;
                                        return false;
                                    }

                                    try
                                    {
                                        //Copy enc
                                        string destFileName = Path.Combine(path, encryptedPictureFile.pEncryptedFileName);
                                        string sourceFileName = Path.Combine(encryptedPictureFile.pEncryptedFilePath, encryptedPictureFile.pEncryptedFileName);
                                        copyEnc(destFileName, sourceFileName);
                                    }
                                    catch
                                    {
                                        //Okay to continue...just show as warning message
                                        img.mMessage = "Warning - Could not replace enc file";
                                    }

                                    cCamera driver = pictureFile.pBelongsToFilm.pCameraDriver;
                                    List<cPicture> pictures;
                                    string message;
                                    if (!driver.developPicture(pictureFile, out pictures, out message))
                                    {
                                        img.mMessage = "Picture File Error - Could not decrypt the picture - picture will not save";
                                    }
                                    else
                                    {
                                        foreach (cPicture picture in pictures)
                                        {
                                            //Copy jpg
                                            try
                                            {
                                                byteArrayToFile(Path.Combine(path, picture.pJpegFileName), picture.pJpeg);
                                            }
                                            catch (Exception ex)
                                            {
                                                //Stop Process if the JPG cannot be copied!!
                                                mError = ex.Message;
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (evSyncedImage != null)
                        {
                            evSyncedImage(this, img, isMissing);
                        }
                    }

                    rd.Close();
                    rd.Dispose();
                    curs.Dispose();
                }

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                okay = false;
                errorWriting = new ErrorLogging();
                errorWriting.WriteErrorLog(ex);
            }

            disconnect();

            return okay;
        }

        public bool getCurrentLoggedCases(cFilm film, int userId, string imagePath, out List<int> fileNumbers)
        {
            bool okay = true;
            fileNumbers = new List<int>();

            if (!film.pHasEncryptedPictureFiles)
            {
                mError = "The film does not contain any encrypted picture files.";
                return false;
            }

            if (film.pHasErrors)
            {
                mError = "The first picture in this collection of pictures cannot be developed. Please remove it and reload this list.";
                return false;
            }

            if (!connect())
                return false;

            try
            {
                var cmd = new OracleCommand("its.offence_logging.get_logged_cases", mDbConnection) { CommandType = CommandType.StoredProcedure };

                cPictureFile pic = film.getFirstValidPictureFile();
                
                // Inputs
                OracleParameter par = new OracleParameter("p_grouping_type", (int) film.pGroupType) {Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Decimal};
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_machine_id", pic.pMachineId) {Direction = ParameterDirection.Input, OracleDbType = OracleDbType.NVarchar2};
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_cam_date", pic.pOffenceDate) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Date };
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_num_days", film.pNumDays) {Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Int32};
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_cam_session", xmlClean(pic.pFormattedSession)) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2 };
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_location_code", xmlClean(pic.pLocationCode)) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2 };
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("o_file_number", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "o_file_number"));

                cmd.ExecuteNonQuery();

                if (!(cmd.Parameters["o_file_number"].Value is DBNull))
                {
                    var curs = (OracleRefCursor) cmd.Parameters["o_file_number"].Value;

                    OracleDataReader rd = curs.GetDataReader();

                    while (rd.Read())
                    {
                        int val = (int) rd.GetDecimal(0);
                        fileNumbers.Add(val);
                    }

                    rd.Close();
                    rd.Dispose();
                    curs.Dispose();
                }

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                okay = false;
                errorWriting = new ErrorLogging();
                errorWriting.WriteErrorLog(ex);
            }

            disconnect();

            return okay;
        }

        public bool checkLoggedSessions(cFilm film, out int result)
        {
            bool okay = true;
            result = 0;

            cPictureFile pic = new cPictureFile();

            if (!film.pHasEncryptedPictureFiles)
            {
                mError = "The film does not contain any encrypted picture files.";
                return false;
            }

            if (film.pHasErrors)
            {
                mError = "The first picture in this collection of pictures cannot be developed. Please remove it and reload this list.";
                return false;
            }

            if (!connect())
                return false;

            try
            {
                var cmd = new OracleCommand("its.offence_logging.compare_logged_case_count", mDbConnection) { CommandType = CommandType.StoredProcedure };

                int fileCount = film.getEncryptedPictureFiles().Count;
                pic = film.getFirstValidPictureFile();
                                
                // Inputs
                OracleParameter par = new OracleParameter("p_grouping_type", (int) film.pGroupType) {Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Decimal};
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_machine_id", pic.pMachineId) {Direction = ParameterDirection.Input, OracleDbType = OracleDbType.NVarchar2};
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_cam_date", pic.pOffenceDate) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Date };
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_num_days", film.pNumDays) {Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Int32};
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_cam_session", xmlClean(pic.pFormattedSession)) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2 };
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_location_code", xmlClean(pic.pLocationCode)) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2 };
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_file_count", fileCount) {Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Decimal};
                cmd.Parameters.Add(par);

                par = new OracleParameter("o_result", result) {Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Decimal};
                cmd.Parameters.Add(par);

                cmd.ExecuteNonQuery();

                if (!(cmd.Parameters["o_result"].Value is DBNull))
                {
                    result = (int) (OracleDecimal) cmd.Parameters["o_result"].Value;
                }

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                ExceptionLog(Environment.MachineName, ex.Message, pic.pFormattedSession,
                film.pCreationTime, "",
                film.pPath, "");

                mError = ex.Message;
                okay = false;
            }

            disconnect();

            return okay;
        }

        public bool logNewCases(cFilm film, int userId, string imagePath, out int numberLogged, out cFilm reportFilm, out int previouslyLogged)
        {
            bool okay = true;
            numberLogged = 0;
            reportFilm = null;
            List<int> lst;
            previouslyLogged = 0;
            string statsFieldsheetDestinationPath = string.Empty;

            if (!film.pHasEncryptedPictureFiles)
            {
                ExceptionLog(Environment.MachineName, "The film does not contain any encrypted picture files.", 
                    "",
                film.pCreationTime, "",
                film.pPath, "");

                mError = "The film does not contain any encrypted picture files.";
                return false;
            }

            if (film.pHasErrors)
            {
                ExceptionLog(Environment.MachineName, "The first picture in this collection of pictures cannot be developed. Please remove it and reload this list.", "",
                film.pCreationTime, "",
                film.pPath, "");

                mError = "The first picture in this collection of pictures cannot be developed. Please remove it and reload this list.";
                return false;
            }

            getCurrentLoggedCases(film, userId, imagePath, out lst);

            if (!connect())
                return false;
           
            try
            {
                //Apply the stats
                film.applyStats();

                //Clear the film - make sure no pictures - will affect reporting - only add pictures that are not logged
                film.clearFilm();

                //Use this to show the report. It will only contain the logged data...
                reportFilm = (cFilm)film.Clone();

                int fileCounter = film.getEncryptedPictureFiles().Count;
                int i = 1;

                foreach (cEncryptedPictureFile encryptedPictureFile in film.getEncryptedPictureFiles())
                {
                    //Check if previously logged
                    cEncryptedPictureFile file = encryptedPictureFile;
                    bool exists = lst.Any(i1 => file.encryptedFileNumberDb == i1);
                    if (exists)
                    {
                        if (evNewCasePreviouslyLogged != null)
                        {
                            evNewCasePreviouslyLogged(this, film, encryptedPictureFile);                            
                        }

                        previouslyLogged += 1;

                        i++;
                        continue;
                    }

                    cPictureFile pictureFile = film.getPictureFile(encryptedPictureFile.pEncryptedFileName);

                    if (pictureFile == null)
                    {
                        if (evNewCaseLoggedError != null)
                        {
                            cPictureFile firstPictureFile = film. getFirstValidPictureFile();
                            evNewCaseLoggedError(this, firstPictureFile, "Picture File Error on file " + encryptedPictureFile.pEncryptedFileName + " - Please redump file or delete file from location.");
                        }

                        i++;
                        continue;
                    }

                    if (!pictureFile.pHasDecryptedData)
                    {
                        if (evNewCaseLoggedError != null)
                        {
                            evNewCaseLoggedError(this, pictureFile, "Picture File Error - Cannot decrypt the data in the picture");
                        }

                        i++;
                        continue;
                    }

                    if (pictureFile.pHasError)
                    {
                        if (evNewCaseLoggedError != null)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (string s in pictureFile.pErrorCollection)
                            {
                                sb.Append(s);
                                sb.Append(",");
                            }

                            evNewCaseLoggedError(this, pictureFile, "Picture Error - " + sb.ToString().TrimEnd(','));
                        }

                        i++;
                        continue;
                    }

                    //string logDate = DateTime.Now.ToString("ddMMyyyy");
                    string logDate = DateTime.Now.ToString("yyyyMMdd");
                    if (logDate.Length == 7)
                    {
                        logDate = "0" + logDate;
                    }
                                        
                    string fileName = logDate + @"\" + pictureFile.pOffenceDateStringYYYYMMDD + @"\" + pictureFile.pDvdSession + @"\" + pictureFile.pMachineId + @"\" + pictureFile.pLocationCode + @"\" + pictureFile.pSession;
                    string path = imagePath + @"\" + logDate;

                    try
                    {
                        makeDir(path);
                        path = Path.Combine(path, pictureFile.pOffenceDateStringYYYYMMDD);
                        makeDir(path);
                        path = Path.Combine(path, pictureFile.pDvdSession);
                        makeDir(path);
                        path = Path.Combine(path, pictureFile.pMachineId);
                        makeDir(path);
                        path = Path.Combine(path, pictureFile.pLocationCode);
                        makeDir(path);
                        path = Path.Combine(path, pictureFile.pSession);
                        makeDir(path);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        if (evNewCaseLoggedError != null)
                        {
                            evNewCaseLoggedError(this, pictureFile, "The following path " + path + " was not found. (" + ex.Message + ")");
                        }

                        //Show stopper. Cannot continue if I cannot create the paths
                        mError =  "The following path " + path + " was not found. (" + ex.Message + ")";
                        return false;
                    }
                    catch (AuthenticationException ex)
                    {
                        if (evNewCaseLoggedError != null)
                        {
                            evNewCaseLoggedError(this, pictureFile, "Authentication to the following " + path + " failed. (" + ex.Message + ")");
                        }

                        //Show stopper. Cannot continue if you dont have access to the paths
                        mError = "Authentication to the following " + path + " failed. (" + ex.Message + ")";
                        return false;
                    }
                    catch (Exception ex)
                    {
                        mError = ex.Message;
                        return false;
                    }

                    statsFieldsheetDestinationPath = path;

                    try
                    {
                        //Copy infringement file
                        string destFileName = Path.Combine(path, encryptedPictureFile.pEncryptedFileName);
                        string sourceFileName = Path.Combine(encryptedPictureFile.pEncryptedFilePath, encryptedPictureFile.pEncryptedFileName);
                        copyEnc(destFileName, sourceFileName);
                    }
                    catch (Exception ex)
                    {
                        //Continue if I cannot copy enc...sometimes it is used by another process
                        if (evNewCaseLoggedError != null)
                        {
                            evNewCaseLoggedError(this, pictureFile, ex.Message);
                        }
                    }

                    //finally copy over the stats file and the fieldsheet
                    if (statsFieldsheetDestinationPath.Length > 0)
                    {
                        if (film.pStatsFileName != null)
                        {
                            //copyStatsFieldSheet(statsFieldsheetDestinationPath, film.pPath, film.pStatsFileName);
                        }
                    }

                    StringBuilder xmlData = new StringBuilder("<main> ");

                    cCamera driver = pictureFile.pBelongsToFilm.pCameraDriver;
                    List<cPicture> pictures;
                    string message;
                    if (!driver.developPicture(pictureFile, out pictures, out message))
                    {
                        if (evNewCaseLoggedError != null)
                        {
                            evNewCaseLoggedError(this, pictureFile, "Picture File Error - " + message + " - Could not decrypt the picture - picture will not save");
                        }

                        i++;
                        continue;
                    }

                    int platesCount = pictures.Count(pic => pic.pIsPlateImage);

                    foreach (cPicture picture in pictures)
                    {
                        //HACK!! 15/01/2016 - Cannot allow more than 2 plates into the system
                        if (platesCount > 1 && picture.pIsPlateImage)
                        {
                            continue;
                        }

                        //Copy jpg
                        try
                        {
                            byteArrayToFile(Path.Combine(path, picture.pJpegFileName), picture.pJpeg);
                        }
                        catch (Exception ex)
                        {
                            if (evNewCaseLoggedError != null)
                            {
                                evNewCaseLoggedError(this, pictureFile, ex.Message);
                            }

                            //Show stopper. Cannot continue if i cannot save the image
                            mError = ex.Message;
                            return false;
                        }

                        if (pictureFile.pOffenceDate != null)
                        {
                            xmlData.Append("<DATA_RECORD> ");

                            xmlData.Append("<CAM_DATE>" + pictureFile.pOffenceDateStringYYMMDD + "</CAM_DATE> ");
                            xmlData.Append("<CAM_SESSION>" + xmlClean(pictureFile.pFormattedSession) + "</CAM_SESSION> ");
                            xmlData.Append("<LOCATION_CODE>" + xmlClean(pictureFile.pLocationCode) + "</LOCATION_CODE> ");
                            xmlData.Append("<FILE_NUMBER>" + encryptedPictureFile.encryptedFileNumberDb + "</FILE_NUMBER> ");
                            xmlData.Append("<FILE_LOCATION>" + xmlClean(encryptedPictureFile.pEncryptedFileName) + "</FILE_LOCATION> ");
                            xmlData.Append("<ZONE>" + pictureFile.pZone + "</ZONE> ");
                            xmlData.Append("<SPEED>" + pictureFile.pSpeed + "</SPEED> ");
                            xmlData.Append("<DISTANCE>" + pictureFile.pDistance + "</DISTANCE> ");
                            xmlData.Append("<DIRECTION>" + xmlClean(pictureFile.pDirection) + "</DIRECTION> ");
                            xmlData.Append("<INFRINGEMENT_DATE>" + pictureFile.pOffenceDate.Value.ToString("dd/MM/yyyy HH:mm:ss ") + "</INFRINGEMENT_DATE> ");
                            xmlData.Append("<GISMO_USER>" + userId + "</GISMO_USER> ");
                            xmlData.Append("<EXTRAINFO>" + xmlClean(pictureFile.pExtraInfo) + "</EXTRAINFO> ");
                            xmlData.Append("<MACHINE_ID>" + xmlClean(pictureFile.pMachineId) + "</MACHINE_ID> ");
                            xmlData.Append("<FILEPATH>" + xmlClean(fileName) + "</FILEPATH> ");
                            xmlData.Append("<FILENAME>" + xmlClean(picture.pJpegFileName) + "</FILENAME> ");
                            xmlData.Append("<ENCRYPTIONKEY>" + xmlClean(film.pEncryptionKey) + "</ENCRYPTIONKEY> ");
                            xmlData.Append("<HASHCODE>" + xmlClean(picture.pJpegHash) + "</HASHCODE> ");

                            if (pictureFile.DotIntialTime != null)
                            {
                                xmlData.Append("<INITIAL_TIME>" + xmlClean(pictureFile.DotIntialTime.Value.ToString("dd/MM/yyyy HH:mm:ss ")) + "</INITIAL_TIME> ");
                            }
                            else
                            {
                                xmlData.Append("<INITIAL_TIME>" + "" + "</INITIAL_TIME> ");
                            }

                            if (pictureFile.DotFinalTime != null)
                            {
                                xmlData.Append("<FINAL_TIME>" + xmlClean(pictureFile.DotFinalTime.Value.ToString("dd/MM/yyyy HH:mm:ss ")) + "</FINAL_TIME> ");
                            }
                            else
                            {
                                xmlData.Append("<FINAL_TIME>" + "" + "</FINAL_TIME> ");
                            }

                            //TMT HACK!!
                            if (platesCount == 1 && !string.IsNullOrEmpty(pictureFile.pPlates[0].pPlate) || (pictureFile.pInfringementType == "8"))
                            {
                                xmlData.Append("<VLN>" + xmlClean(pictureFile.pPlates[0].pPlate) + "</VLN> ");
                            }
                            else
                            {
                                xmlData.Append("<VLN>" + "" + "</VLN> ");
                            }

                            xmlData.Append("<CLASSIFICATION>" + xmlClean(pictureFile.pClassification) + "</CLASSIFICATION> ");

                            //TMT HACK!!
                            if (platesCount == 1 && !string.IsNullOrEmpty(pictureFile.pPlates[0].pPlateConfidence))
                            {
                                xmlData.Append("<CONFIDENCE>" + xmlClean(pictureFile.pPlates[0].pPlateConfidence) + "</CONFIDENCE> ");
                            }
                            else
                            {
                                xmlData.Append("<CONFIDENCE>" + "" + "</CONFIDENCE> ");
                            }
                            
                            if (pictureFile.OffenceCode != null)
                            {
                                xmlData.Append("<OFFENCE_CODE>" + xmlClean(pictureFile.OffenceCode) + "</OFFENCE_CODE> ");
                            }
                            else
                            {
                                xmlData.Append("<OFFENCE_CODE>" + "" + "</OFFENCE_CODE> ");
                            }

                            xmlData.Append("<VIDEO_LOG_ID>" + "" + "</VIDEO_LOG_ID> ");

                            if (!string.IsNullOrEmpty(pictureFile.SourceFileName))
                            {
                                xmlData.Append("<VIDEO_FILE_NAME>" + xmlClean(Path.GetFileName(pictureFile.SourceFileName)) + "</VIDEO_FILE_NAME> ");
                            }
                            else
                            {
                                xmlData.Append("<VIDEO_FILE_NAME>" + "" + "</VIDEO_FILE_NAME> ");
                            }
                                                        
                            xmlData.Append("<VIDEO_FILE_DATE>" + pictureFile.pOffenceDate.Value.ToString("dd/MM/yyyy") + "</VIDEO_FILE_DATE> ");

                            if (!string.IsNullOrEmpty(pictureFile.pMachineId))
                            {
                                xmlData.Append("<VIDEO_SOURCE_REF>" + xmlClean(pictureFile.pMachineId) + "</VIDEO_SOURCE_REF> ");
                            }
                            else
                            {
                                xmlData.Append("<VIDEO_SOURCE_REF>" + "" + "</VIDEO_SOURCE_REF> ");
                            }

                            xmlData.Append("<VIDEO_FILE_LOCATION>" + xmlClean(pictureFile.SourceFileName) + "</VIDEO_FILE_LOCATION> ");

                            xmlData.Append("<OFFICER_ID>" + xmlClean(pictureFile.OfficerCode) + "</OFFICER_ID> ");

                            xmlData.Append("<INFRINGEMENT_TYPE>" + xmlClean(pictureFile.pInfringementType) + "</INFRINGEMENT_TYPE> ");
                            
                            xmlData.Append("</DATA_RECORD> ");
                        }
                    }

                    xmlData.Append("</main>");

                    var cmd = new OracleCommand("its.offence_logging.log_new_cases_vids", mDbConnection) { CommandType = CommandType.StoredProcedure };
                                    
                    OracleParameter par = new OracleParameter();

                    // Inputs
                    par = new OracleParameter("p_xml_data", xmlData.ToString()) { Direction = ParameterDirection.Input, Size = 8000, OracleDbType = OracleDbType.XmlType };
                    cmd.Parameters.Add(par);

                    if (i == fileCounter)
                    {
                        cPictureFile firstPicture = film.getFirstValidPictureFile();

                        if (firstPicture != null)
                        {
                            StringBuilder xmlSummary = new StringBuilder("<main> ");
                            xmlSummary.Append("<DATA_RECORD> ");

                            xmlSummary.Append("<CAM_DATE>" + firstPicture.pOffenceDateStringYYMMDD + "</CAM_DATE> ");
                            xmlSummary.Append("<CAM_SESSION>" + xmlClean(firstPicture.pFormattedSession) + "</CAM_SESSION> ");
                            xmlSummary.Append("<LOCATION_CODE>" + xmlClean(firstPicture.pLocationCode) + "</LOCATION_CODE> ");

                            xmlSummary.Append("<FILE_COUNT>" + film.getPicturesFiles().Count + "</FILE_COUNT> ");
                            xmlSummary.Append("<FILE_START>" + "" + "</FILE_START> ");
                            xmlSummary.Append("<FILE_END>" + "" + "</FILE_END> ");
                            xmlSummary.Append("<GISMO_USER>" + userId + "</GISMO_USER> ");
                            xmlSummary.Append("<EDIT_DATE>" + "" + "</EDIT_DATE> ");

                            if (film.pStartDate.HasValue)
                            {
                                xmlSummary.Append("<START_TIME>" + film.pStartDate.Value.ToString("dd/MM/yyyy HH:mm:ss ") + "</START_TIME> ");
                            }
                            else
                            {
                                xmlSummary.Append("<START_TIME>" + "" + "</START_TIME> ");
                            }

                            if (film.pEndDate.HasValue)
                            {
                                xmlSummary.Append("<END_TIME>" + film.pEndDate.Value.ToString("dd/MM/yyyy HH:mm:ss ") + "</END_TIME> ");
                            }
                            else
                            {
                                xmlSummary.Append("<END_TIME>" + "" + "</END_TIME> ");
                            }

                            if (film.pStartDate.HasValue && film.pEndDate.HasValue)
                            {
                                TimeSpan ts = film.pEndDate.Value.Subtract(film.pStartDate.Value);
                                xmlSummary.Append("<DURATION>" + ts.TotalMinutes + "</DURATION> ");
                            }
                            else
                            {
                                xmlSummary.Append("<DURATION>" + "" + "</DURATION> ");
                            }

                            xmlSummary.Append("<VEHICLES_CHECKED>" + film.pVehiclesChecked + "</VEHICLES_CHECKED> ");
                            xmlSummary.Append("<INFRINGEMENTS>" + film.pInfringements + "</INFRINGEMENTS> ");
                            xmlSummary.Append("<HIGHEST_SPEED>" + film.pHighestSpeed + "</HIGHEST_SPEED> ");
                            xmlSummary.Append("<LOWEST_SPEED>" + film.pLowestSpeed + "</LOWEST_SPEED> ");
                            xmlSummary.Append("<AVERAGE_SPEED>" + film.pAverageSpeed + "</AVERAGE_SPEED> ");

                            xmlSummary.Append("<LONGITUDE>" + xmlClean(firstPicture.pLongitude) + "</LONGITUDE> ");
                            xmlSummary.Append("<LATITUDE>" + xmlClean(firstPicture.pLatitude) + "</LATITUDE> ");
                            xmlSummary.Append("<LIGHT_ZONE>" + firstPicture.pZoneL + "</LIGHT_ZONE> ");
                            xmlSummary.Append("<HEAVY_ZONE>" + firstPicture.pZoneH + "</HEAVY_ZONE> ");
                            xmlSummary.Append("<PT_ZONE>" + firstPicture.pZonePT + "</PT_ZONE> ");

                            xmlSummary.Append("</DATA_RECORD> ");
                            xmlSummary.Append("</main>");
                                                        
                            // Inputs
                            par = new OracleParameter("p_xml_summary", xmlSummary.ToString()) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2 };
                            cmd.Parameters.Add(par);
                        }
                        else
                        {
                            // Inputs
                            par = new OracleParameter("p_xml_summary", " ") { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2, Size = 4000 };
                            cmd.Parameters.Add(par);
                        }
                    }
                    else
                    {
                        // Inputs
                        par = new OracleParameter("p_xml_summary", " ") { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Varchar2, Size = 4000 };
                        cmd.Parameters.Add(par);
                    }

                    //Output
                    par = new OracleParameter("o_number_logged", numberLogged) { Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Decimal };
                    cmd.Parameters.Add(par);

                    par = new OracleParameter("o_file_number", numberLogged) { Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Decimal };
                    cmd.Parameters.Add(par);

                    try
                    {
                        cmd.ExecuteNonQuery();

                        if (cmd.Parameters["o_file_number"].Value != DBNull.Value && cmd.Parameters["o_number_logged"].Value != DBNull.Value)
                        {                        
                            int nl;
                            if (int.TryParse(cmd.Parameters["o_number_logged"].Value.ToString(), out nl))
                            {
                                numberLogged += nl;
                            }

                            int fn;
                            if (int.TryParse(cmd.Parameters["o_file_number"].Value.ToString(), out fn))
                            {
                                if (evNewCaseLogged != null)
                                {
                                    evNewCaseLogged(this, pictureFile, fn, numberLogged);
                                }
                            }
                        }

                        reportFilm.addToPictureFiles(pictureFile);
                    }
                    catch (Exception ex)
                    {                        
                        //Inform user but continue...
                        if (evNewCaseLoggedError != null)
                        {
                            evNewCaseLoggedError(this, pictureFile, ex.Message);
                        }
                    }
                    finally
                    {
                        cmd.Dispose();
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                okay = false;
                errorWriting = new ErrorLogging();
                errorWriting.WriteErrorLog(ex);
            }

            disconnect();

            return okay;
        }
        public static void copyStatsFieldSheet(string destPath, string sourcepath, string statsFileName)
        {
            string destFileName;
            FileAttributes attributes;

            destFileName = Path.Combine(destPath, statsFileName);
            //If readonly in destination and you must overwrite..remove readonly...
            if (File.Exists(destFileName))
            {
                attributes = File.GetAttributes(destFileName);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(destFileName, FileAttributes.Normal);
                }
            }

            File.Copy(Path.Combine(sourcepath, statsFileName), destFileName, true);

            //Make sure its not read only after copied - becomes readonly if copied from dvd...
            attributes = File.GetAttributes(destFileName);
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(destFileName, FileAttributes.Normal);
            }


            string[] files = Directory.GetFiles(sourcepath, "*.pdf");

            if (files != null || files.Length > 0)
            {
                foreach (string file in files)
                {
                    destFileName = Path.Combine(destPath, file);
                    //If readonly in destination and you must overwrite..remove readonly...
                    if (File.Exists(destFileName))
                    {
                        attributes = File.GetAttributes(destFileName);
                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            File.SetAttributes(destFileName, FileAttributes.Normal);
                        }
                    }

                    File.Copy(Path.Combine(sourcepath, file), destFileName, true);

                    //Make sure its not read only after copied - becomes readonly if copied from dvd...
                    attributes = File.GetAttributes(destFileName);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        File.SetAttributes(destFileName, FileAttributes.Normal);
                    }
                }
            }
        }

        public bool logVosiFile(cFilm film)
        {
            int counterVosi = 0;
            if (film.pVosiCollection.Count > 0)
            {
                if (!connect())
                    return false;

                    foreach (IVosiEntry vosiEntry in film.pVosiCollection)
                    {
                        try
                        {
                            counterVosi += 1;
                            if (vosiEntry.LineItem != string.Empty)
                            {
                                string error;
                                vosiEntry.Extract(out error);

                                var cmd = new OracleCommand("REPORTING.STATISTICS.LOG_VOSI_STATS", mDbConnection) { CommandType = CommandType.StoredProcedure };

                                OracleParameter par = new OracleParameter();

                                DateTime vosiHitDate;

                                vosiHitDate = DateTime.ParseExact(vosiEntry.Date + ' ' + vosiEntry.Time, "yyyy/MM/dd HH:mm:ss.ff", CultureInfo.InvariantCulture);

                                // Inputs
                                par = new OracleParameter("p_vosi_hit_datetime", vosiHitDate) { Direction = ParameterDirection.Input, OracleDbType = OracleDbType.Date };
                                cmd.Parameters.Add(par);

                                par = new OracleParameter("p_vln", vosiEntry.VLN) { Direction = ParameterDirection.Input, Size = 4000, OracleDbType = OracleDbType.Varchar2 };
                                cmd.Parameters.Add(par);
                                par = new OracleParameter("p_location_code", vosiEntry.LocationCode) { Direction = ParameterDirection.Input, Size = 4000, OracleDbType = OracleDbType.Varchar2 };
                                cmd.Parameters.Add(par);
                                
                                par = new OracleParameter("p_reason", vosiEntry.Reason) { Direction = ParameterDirection.Input, Size = 4000, OracleDbType = OracleDbType.Varchar2 };
                                cmd.Parameters.Add(par);

                                par = new OracleParameter("o_success", OracleDbType.Int32, ParameterDirection.Output);
                                cmd.Parameters.Add(par);

                                try
                                {
                                    cmd.ExecuteNonQuery();

                                    if (cmd.Parameters["o_success"].Value != DBNull.Value)
                                    {
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorWriting = new ErrorLogging();
                                    errorWriting.WriteErrorLog(ex);                             
                                }
                                finally
                                {
                                    cmd.Dispose();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (evNewCaseLoggedError != null)
                            {
                                cPictureFile firstPictureFile = film.getFirstValidPictureFile();
                                evNewCaseLoggedError(this, firstPictureFile, "Vosi file Path and Name " + film.pVosiFileName + " has an error on line: " + counterVosi.ToString() + ". Please send to helpdesk for investigation");
                            }
                        }
                    }                
            }

            return false;
        }

        public bool ExceptionLog(string machineId, string exceptionMessage, string cameraSession,
            DateTime? offenceDate, string locationCode, string filePath, string fileName)
        {
            if (!connect())
                return false;

            var cmd = new OracleCommand("ITS.OFFENCE_LOGGING.EXCEPTION_LOGGING", mDbConnection) { CommandType = CommandType.StoredProcedure };

            OracleParameter par = new OracleParameter();

            par = new OracleParameter("p_machine_id", machineId) { Direction = ParameterDirection.Input, Size = 100, OracleDbType = OracleDbType.Varchar2 };
            cmd.Parameters.Add(par);

            par = new OracleParameter("p_exception_message", exceptionMessage) { Direction = ParameterDirection.Input, Size = 4000, OracleDbType = OracleDbType.Varchar2 };
            cmd.Parameters.Add(par);

            par = new OracleParameter("p_camera_session", cameraSession) { Direction = ParameterDirection.Input, Size = 30, OracleDbType = OracleDbType.Varchar2 };
            cmd.Parameters.Add(par);

            par = new OracleParameter("p_offence_date", string.Format("{0}, dd/MM/yyyy", offenceDate)) { Direction = ParameterDirection.Input, Size = 11, OracleDbType = OracleDbType.Varchar2 };
            cmd.Parameters.Add(par);

            par = new OracleParameter("p_location_code", locationCode) { Direction = ParameterDirection.Input, Size = 10, OracleDbType = OracleDbType.Varchar2 };
            cmd.Parameters.Add(par);

            par = new OracleParameter("p_file_path", filePath) { Direction = ParameterDirection.Input, Size = 260, OracleDbType = OracleDbType.Varchar2 };
            cmd.Parameters.Add(par);

            par = new OracleParameter("p_file_name", fileName) { Direction = ParameterDirection.Input, Size = 30, OracleDbType = OracleDbType.Varchar2 };
            cmd.Parameters.Add(par);
            
            int result;
            result = cmd.ExecuteNonQuery();
            return (result == 1);
        }

        public IList<EncStatisticModel> ReadStatsFile(FileInfo processFile)
        {
            //first update the following string in the 
            var statsText = File.ReadAllText(processFile.FullName);
            var statsTextR1 = statsText.Replace("smd-string=\"\"", "smd-string=\"\",");
            var statsTextR2 = statsTextR1.Replace(";", ",");

            var splitCommaDelimitedString = statsTextR2.Split(',');
            var result = splitCommaDelimitedString.Where(s => s.Contains("file="));
            var finalResult = result.Where(s => !s.Contains("file=None"));

            var list = new List<EncStatisticModel>();

            var fileName = processFile.Name;
            var fileNameSplit = fileName.Split('_');
            var locationCode = fileNameSplit[fileNameSplit.Length - 1].Split('.')[0];
            var machineId = string.Empty;
            if (processFile.Directory != null)
            {
                var directoryInfo = Directory.GetParent(processFile.FullName);
                machineId = directoryInfo.Parent.Parent.Name;
            }

            using (var file = new StreamReader(processFile.FullName))
            {
                string line;
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        line = line.Replace("\0", string.Empty);
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            statsTextR1 = line.Replace("smd-string=\"\"", "smd-string=\"\",");
                            statsTextR2 = statsTextR1.Replace(";", ",");
                            var statsTextR3 = statsTextR2.Replace(",)", ")");

                            splitCommaDelimitedString = statsTextR3.Trim().Split(',');

                            var enc = new EncStatisticModel
                            {
                                RunDate = splitCommaDelimitedString[0],
                                StatsFileId = fileName,
                                MachineId = machineId,
                                LocationCode = locationCode,
                                Time = splitCommaDelimitedString[1].Replace(" ", ""),
                                Speed = Convert.ToInt32(splitCommaDelimitedString[2].Split('=')[1]),
                                Zone = Convert.ToInt32(splitCommaDelimitedString[3].Split('=')[1]),
                                Lane = Convert.ToInt32(splitCommaDelimitedString[4].Split('=')[1]),
                                Type = splitCommaDelimitedString[5].Split('=')[1],
                                Distance = Convert.ToInt32(splitCommaDelimitedString[6].Split('=')[1]),
                                Direction = splitCommaDelimitedString[7].Split('=')[1],
                                Classification = splitCommaDelimitedString[8].Split('=')[1],
                                Captured = splitCommaDelimitedString[9].Split('=')[1],
                                File = splitCommaDelimitedString[10].Split('=')[1],
                                Error = splitCommaDelimitedString[11].Split('=')[1],
                                SmdString = splitCommaDelimitedString[12].Split('=')[1],
                            };
                            try
                            {
                                enc.Plates = !string.IsNullOrEmpty(splitCommaDelimitedString[13])
                                    ? splitCommaDelimitedString[13].Split('=')[1]
                                    : string.Empty;
                            }
                            catch (Exception e)
                            {
                                enc.Plates = string.Empty;
                            }

                            list.Add(enc);
                        }
                    }
                }
            }

            return list;
        }

        public static void copyEnc(string destFileName, string sourceFileName)
        {
            FileAttributes attributes;

            //If readonly in destination and you must overwrite..remove readonly...
            if (File.Exists(destFileName))
            {
                attributes = File.GetAttributes(destFileName);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(destFileName, FileAttributes.Normal);
                }
            }

            File.Copy(sourceFileName, destFileName, true);

            //Make sure its not read only after copied - becomes readonly if copied from dvd...
            attributes = File.GetAttributes(destFileName);
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(destFileName, FileAttributes.Normal);
            }
        }

        public static void makeDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                DirectorySecurity dirSec = Directory.GetAccessControl(path);
                dirSec.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                dirSec.AddAccessRule(new FileSystemAccessRule("Administrators", FileSystemRights.FullControl, AccessControlType.Allow));
                dirSec.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
                dirSec.AddAccessRule(new FileSystemAccessRule("SYSTEM", FileSystemRights.FullControl, AccessControlType.Allow));
                dirSec.AddAccessRule(new FileSystemAccessRule("Authenticated Users", FileSystemRights.FullControl, AccessControlType.Allow));
                Directory.SetAccessControl(path, dirSec);
            }
        }

        /// <summary>
        /// 	Function to save byte array to a file
        /// </summary>
        /// <param name = "fileName">File name to save byte array</param>
        /// <param name = "byteArray">Byte array to save to external file</param>
        /// <returns>Return true if byte array save successfully, if not return false</returns>
        public bool byteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                // Open file for reading
                FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);

                // Writes a block of bytes to this stream using data from a byte array.
                fileStream.Write(byteArray, 0, byteArray.Length);

                // close file stream
                fileStream.Close();

                return true;
            }
            catch (Exception exception)
            {
                // Error
                errorWriting = new ErrorLogging();
                errorWriting.WriteErrorLog(exception);
            }

            // error occured, return false
            return false;
        }

        public DateTime GetDatabaseDate()
        {
            var systemDate = DateTime.UtcNow;

            WithConnection(
                connection =>
                {
                    using (var cmd = new OracleCommand("SELECT SYSDATE FROM DUAL", connection))
                    {
                        var retValue = cmd.ExecuteScalar();

                        systemDate = DateTime.Parse(retValue.ToString());
                    }
                });

            return systemDate;
        }

        private void WithConnection(Action<OracleConnection> action)
        {
            OracleConnection connection = null;

            try
            {
                connection = new OracleConnection(mConnectionString);
                connection.Open();

                if (action != null)
                {
                    action.Invoke(connection);
                }
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public void SubmitStatsFile(IList<EncStatisticModel> models)
        {        
            try
            {
                using (var dbConnection = new OracleConnection(mConnectionString))
                using (var command = new OracleCommand())
                {
                    dbConnection.Open();

                    var objects =
                        models.Select(
                            model => new CameraStatistics
                            {
                                RUN_DATE = model.RunDate,
                                LOCATION_CODE = model.LocationCode,
                                TIME = model.Time,
                                MACHINE_ID = model.MachineId,
                                STATS_FILE_NAME = model.StatsFileId,
                                SPEED = model.Speed,
                                ZONE = model.Zone,
                                LANE = model.Lane,
                                TYPE = model.Type,
                                DISTANCE = model.Distance,
                                DIRECTION = model.Direction,
                                CLASSIFICATION = model.Classification,
                                CAPTURED = model.Captured,
                                ENC_FILE_NAME = model.File,
                                ERROR_MESSAGE = model.Error,
                                SMD_STRING = model.SmdString,
                                PLATES = model.Plates,
                            })
                            .ToList();

                    var param =
                        new OracleParameter("p_data", OracleDbType.Array)
                        {
                            Value = objects.ToArray(),
                            UdtTypeName = "REPORTING.TABLE_CAMERA_STATISTICS_TYPE"
                        };

                    command.Parameters.Add(param);
                    command.BindByName = true;

                    try 
                    {
                        ExcecuteNonQuery(command, "REPORTING.STATISTICS.UPLOAD_ENC_CAMERA_STATS", dbConnection); 
                    }
                    catch (Exception ex)
                    {
                        errorWriting = new ErrorLogging();
                        errorWriting.WriteErrorLog(ex);                                
                    }
                    finally
                    {
                        command.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
               var Error = e.Message;
            }
        }

        private void ExcecuteNonQuery(OracleCommand command, string storedProcName, OracleConnection dbConnection)
        {
            command.Connection = dbConnection;
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }

        public static void saveJpeg(string path, Image img, int quality)
        {
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);

            ImageCodecInfo jpegCodec = getEncoderInfo(@"image/jpeg");

            EncoderParameters encoderParams
                = new EncoderParameters(1);

            encoderParams.Param[0] = qualityParam;

            MemoryStream mss = new MemoryStream();

            FileStream fs
                = new FileStream(path, FileMode.Create
                                 , FileAccess.ReadWrite);

            img.Save(mss, jpegCodec, encoderParams);
            byte[] matriz = mss.ToArray();
            fs.Write(matriz, 0, matriz.Length);

            mss.Close();
            fs.Close();
        }

        private static ImageCodecInfo getEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private static string xmlClean(string xml)
        {
            if (xml == null) return "";

            string tmp = xml.Trim();

            tmp = tmp.Replace("&", "&amp;");
            tmp = tmp.Replace("<", "&lt;");
            tmp = tmp.Replace(">", "&gt;");
            tmp = tmp.Replace("\"", "&quot;");
            tmp = tmp.Replace("'", "&apos;");
            tmp = tmp.Replace("\0", "");

            return tmp;
        }

        #endregion

        public string pError
        {
            get { return mError; }
        }

        public event ValidateImageDelegate evSyncedImage;

        public event NewCaseLoggedDelegate evNewCaseLogged;
        public event NewCaseLoggedErrorDelegate evNewCaseLoggedError;
        public event NewCasePreviouslyLoggedDelegate evNewCasePreviouslyLogged;
        
    }

    public struct sLoggedImage
    {
        public string mDvdSession;
        public string mFileName;
        public decimal mFileNumber;
        public bool mHasError;
        public string mImagePath;
        public string mLocationCode;
        public string mLogDate;
        public string mMachineId;
        public string mOffenceDate;

        public string mMessage;
        public string mSession;
    }
}