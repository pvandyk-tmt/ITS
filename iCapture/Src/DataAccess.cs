using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Reflection;

namespace TMT.iCapture
{
    /// <summary>
    /// Data layer for iCapture
    /// </summary>
    public class cDataAccess
    {
        public struct sCaseInfo
        {
            public string mVehicleRegNo;
            public string mVehicleType;
            public string mOffenceDate;
            public string mOffencePlace;
            public int mPrevRejectID;
            public int mOffenceSpeed;
            public int mOffenceZone;
            public string mImage1;
            public string mImage2;
            public string mImage3;
            public string mImage4;
            public string mImageNP;
            public int mImage1ID;
            public int mImage2ID;
            public int mImage3ID;
            public int mImage4ID;
            public bool mOnlyOneImage;
            public int mPrintImageNo;
        }

        public struct sRejectReasons
        {
            public int mID;
            public string mDescription;

            public override string ToString()
            {
                return mDescription;
            }
        }

        public struct sCaptureTypes
        {
            public int mID;
            public int mCode;
            public string mType;
            public string mDescription;
            public string mBeskrywing;
            public decimal mAmount;

            public override string ToString()
            {
                return mType;
            }
        }

        public struct sOfficerInfo
        {
            public int mID;
            public string mExternID;
            public string mName;
            public string mSurname;
        }

        public struct sSessionInfo
        {
            public string mLocationCode;
            public string mCamDate;
            public string mCamSession;
            public string mMachineId;
            public int mNothingDoneCol;
            public int mNothingDone;
            public int mCamDateCol;
            public object[] mColumns;
        }

        private OracleConnection mDBConnection = null;

        private string mConnectionString = string.Empty;
        private string mError = string.Empty;
        private int mOperatorID = -1;
        private string mOperatorUsername = string.Empty;
        private bool mNPCapturedOnCentral = false;     // Number-plate image captured at central
        private string mNPCapturePath = string.Empty;  // Number-plate image path to central

        /// <summary>
        /// Check if the number-plate capturing is done on central.
        /// </summary>
        public bool pNPCapturedOnCentral
        {
            get { return mNPCapturedOnCentral; }
        }

        /// <summary>
        /// Gets the last error in the data layer.
        /// </summary>
        public string pError
        {
            get { return mError; }
        }

        public cDataAccess(string connectionString)
        {
            mConnectionString = connectionString;
        }

        /// <summary>
        /// Make original connection to DB.
        /// </summary>
        /// <param name="connectString">Connection string</param>
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
                mDBConnection = new OracleConnection(mConnectionString);

                mDBConnection.Open();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                mDBConnection = null;
            }

            return (mDBConnection != null);
        }

        /// <summary>
        /// Close DB connection.
        /// </summary>
        public void disconnect()
        {
            if (mDBConnection != null)
            {
                mDBConnection.Close();
                mDBConnection.Dispose();
            }

            mDBConnection = null;
        }

        public bool authorise(string username, string password)
        {
            mError = string.Empty;
            mOperatorID = -1;
            mOperatorUsername = username;

            try
            {
                OracleCommand cmd = new OracleCommand("SELECT ID FROM GISMO.GISMOEMPLOYEE WHERE USER_ID='" + username + "' AND USER_PASSWORD='" + password + "'", mDBConnection);

                object val = cmd.ExecuteScalar();

                cmd.Dispose();

                if ((val == null) || (val is System.DBNull))
                    mError = "Username '" + username + "' and given Password not found in database.";
                else
                    mOperatorID = int.Parse(val.ToString());
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                mOperatorID = -1;
            }

            return (mOperatorID != -1);
        }

        public bool mayCapture(string username)
        {
            mError = string.Empty;

            /*** Any logged-in user may Capture ***/
            //try
            //{
            //    OracleCommand cmd = new OracleCommand("SELECT ACTIVITY_ID FROM GISMOEMPLOYEE_ACTIVITY WHERE EMPLOYEE_ID=" + mOperatorID + " AND ACTIVITY_ID=101", mDBConnection); //TODO ACTIVITY_ID=???

            //    object val = cmd.ExecuteScalar();

            //    cmd.Dispose();

            //    if ((val == null) || (val is System.DBNull))
            //    {
            //        mError = "Username '" + username + "' may not Capture tickets.";
            //        return false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    mError = ex.Message;
            //    return false;
            //}

            return true;
        }

        public void caseInitialise(out sCaseInfo caseInfo)
        {
            caseInfo.mImage1 = string.Empty;
            caseInfo.mImage2 = string.Empty;
            caseInfo.mImage3 = string.Empty;
            caseInfo.mImage4 = string.Empty;
            caseInfo.mImageNP = string.Empty;
            caseInfo.mImage1ID =
            caseInfo.mImage2ID =
            caseInfo.mImage3ID =
            caseInfo.mImage4ID = -1;
            caseInfo.mOnlyOneImage = false;
            caseInfo.mPrintImageNo = 0;
            caseInfo.mVehicleRegNo = string.Empty;
            caseInfo.mVehicleType = string.Empty;
            caseInfo.mPrevRejectID = -1;
            caseInfo.mOffenceDate = string.Empty;
            caseInfo.mOffencePlace = string.Empty;
            caseInfo.mOffenceSpeed = -1;
            caseInfo.mOffenceZone = -1;
        }

        public void typeInitialise(out sCaptureTypes type)
        {
            type.mID = -1;

            type.mCode = -1;
            type.mType = string.Empty;
            type.mDescription = string.Empty;
            type.mBeskrywing = string.Empty;
            type.mAmount = 0;
        }

        public bool sessions(out string[] headings, ref List<sSessionInfo> sessions)
        {
            headings = null;
            sessions.Clear();
            mError = string.Empty;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_CAPTURE_CENTRAL.GET_CAPTURE_SCREEN", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_USER_ID", mOperatorID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("O_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_CURSOR"));

                cmd.ExecuteNonQuery();

                if (!(cmd.Parameters["O_CURSOR"].Value is System.DBNull))
                {
                    OracleRefCursor rc = (OracleRefCursor)cmd.Parameters["O_CURSOR"].Value;
                    OracleDataReader rd = rc.GetDataReader();

                    headings = new string[rd.FieldCount];

                    for (int i = 0; i < rd.FieldCount; i++)
                        headings[i] = rd.GetName(i);

                    int locationCodeIndex = -1;
                    int machineIdIndex = -1;
                    int camDateIndex = -1;
                    int camSessionIndex = -1;
                    int nothingDoneIndex = -1;

                    for (int i = 0; i < headings.Length; i++)
                    {
                        if (headings[i].ToUpper() == "LOCATION_CODE")
                            locationCodeIndex = i;
                        else if (headings[i].ToUpper() == "CAM_DATE")
                            camDateIndex = i;
                        else if (headings[i].ToUpper() == "CAM_SESSION")
                            camSessionIndex = i;
                        else if (headings[i].ToUpper() == "NOTHING_DONE")
                            nothingDoneIndex = i;
                        else if (headings[i].ToUpper() == "MACHINE_ID")
                            machineIdIndex = i;
                    }

                    if (locationCodeIndex < 0)
                        mError += "LOCATION_CODE could not be found in the data.";
                    if (camDateIndex < 0)
                        mError += "CAM_DATE could not be found in the data.";
                    if (camSessionIndex < 0)
                        mError += "CAM_SESSION could not be found in the data.";
                    if (nothingDoneIndex < 0)
                        mError += "NOTHING_DONE could not be found in the data.";
                    if (machineIdIndex < 0)
                        mError += "MACHINE_ID could not be found in the data.";
                    if (mError.Length > 0)
                    {
                        rd.Close();
                        rd.Dispose();
                        cmd.Dispose();
                        return false;
                    }

                    sessions = new List<sSessionInfo>();

                    while (rd.Read())
                    {
                        sSessionInfo session;
                        session.mColumns = new object[rd.FieldCount];

                        rd.GetValues(session.mColumns);

                        session.mLocationCode = session.mColumns[locationCodeIndex].ToString();
                        session.mCamDate = session.mColumns[camDateIndex].ToString(); // YYMMdd = YY/MM/DD
                        session.mCamSession = session.mColumns[camSessionIndex].ToString();
                        session.mNothingDone = int.Parse(session.mColumns[nothingDoneIndex].ToString());
                        session.mNothingDoneCol = nothingDoneIndex;
                        session.mCamDateCol = camDateIndex;
                        session.mMachineId = session.mColumns[machineIdIndex].ToString();

                        sessions.Add(session);
                    }
                    rd.Close();
                    rd.Dispose();
                }
                else
                {
                    cmd.Dispose();
                    mError = "No sessions for capturing exist.";
                    return false;
                }

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                errorCheckFriendly();
                return false;
            }

            return true;
        }

        private void errorCheckFriendly()
        {
            if (mError.StartsWith("ORA-20002:") ||
                mError.StartsWith("ORA-20030:") ||
                mError.StartsWith("ORA-20031:") ||
                mError.StartsWith("ORA-20032:") ||
                mError.StartsWith("ORA-20033:") ||
                mError.StartsWith("ORA-20034:") ||
                mError.StartsWith("ORA-20035:"))
            {
                int s = mError.IndexOf(":");

                if (s > 0)
                {
                    s++;
                    mError = mError.Substring(s, mError.Length - s);
                }
                mError = mError.Substring(0, mError.IndexOf("\n") + 1);
                mError = mError.Trim();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reasons"></param>
        /// <param name="types"></param>
        /// <param name="count"></param>
        /// <param name="caseInfo"></param>
        /// <returns></returns>
        public bool caseFirst(sSessionInfo session, bool loadNewOnly, ref List<sRejectReasons> reasons, ref List<sCaptureTypes> types, ref List<int> fileNumbers, ref List<sOfficerInfo> officers, ref sCaseInfo caseInfo, out int offenceSet, out int startIndex)
        {
            mError = string.Empty;
            offenceSet = -1;
            startIndex = 0;

            caseInitialise(out caseInfo);

            fileNumbers.Clear();
            reasons.Clear();
            types.Clear();

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_CAPTURE_CENTRAL.get_cap_multi_image_lookup", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_USER_ID", mOperatorID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", System.Environment.MachineName);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_CAM_DATE", session.mCamDate);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_CAM_SESSION", session.mCamSession);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_LOCATION_CODE", session.mLocationCode);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_LOAD_TYPE", (loadNewOnly ? 1 : 0));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);


                // Outputs
                cmd.Parameters.Add(new OracleParameter("O_NUMBER_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NUMBER_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_HOLDING_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_HOLDING_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REJECTION_REASON_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REJECTION_REASON_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_OFFICER_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_OFFICER_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_VEHICLE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_ALT_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_ALT_VEHICLE_CURSOR"));

                par = new OracleParameter();
                par.ParameterName = "O_FIRST_FILE_NR";
                par.Direction = ParameterDirection.Output;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                cmd.Parameters.Add(par);

                par = new OracleParameter();
                par.ParameterName = "O_OFFENCE_SET";
                par.Direction = ParameterDirection.Output;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                cmd.Parameters.Add(par);

                par = new OracleParameter();
                par.ParameterName = "O_CENTRAL_INDICATOR";
                par.Direction = ParameterDirection.Output;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                cmd.Parameters.Add(par);

                cmd.ExecuteNonQuery();

                int firstFileNumber = int.Parse(cmd.Parameters["O_FIRST_FILE_NR"].Value.ToString());
                offenceSet = int.Parse(cmd.Parameters["O_OFFENCE_SET"].Value.ToString());
                mNPCapturedOnCentral = (int.Parse(cmd.Parameters["O_CENTRAL_INDICATOR"].Value.ToString()) == 1 ? true : false);

                fileNumbersFromCursor((OracleRefCursor)cmd.Parameters["O_HOLDING_CURSOR"].Value, ref fileNumbers);
                startIndex = fileNumbers.Count;
                fileNumbers.Add(firstFileNumber);
                fileNumbersFromCursor((OracleRefCursor)cmd.Parameters["O_NUMBER_CURSOR"].Value, ref fileNumbers);

                if (!reasonsFromCursor((OracleRefCursor)cmd.Parameters["O_REJECTION_REASON_CURSOR"].Value, ref reasons))
                    return false;

                if (!officersFromCursor((OracleRefCursor)cmd.Parameters["O_OFFICER_CURSOR"].Value, ref officers))
                    return false;

                if (!vehicleFromCursor((OracleRefCursor)cmd.Parameters["O_VEHICLE_CURSOR"].Value, ref caseInfo))
                    return false;

                if (!imagesFromCursor((OracleRefCursor)cmd.Parameters["O_LOCAL_IMAGE_CURSOR"].Value, ref caseInfo)) // First look if images exist on local drive
                    return false;
                if (!File.Exists(caseInfo.mImage1)) // Images does not exist locally, get remote paths
                    imagesFromCursor((OracleRefCursor)cmd.Parameters["O_REMOTE_IMAGE_CURSOR"].Value, ref caseInfo);
                else if (mNPCapturedOnCentral) // if capture is done at central then retrieve number-plate image from remote (central) location
                {
                    sCaseInfo tmp;

                    caseInitialise(out tmp);
                    imagesFromCursor((OracleRefCursor)cmd.Parameters["O_REMOTE_IMAGE_CURSOR"].Value, ref tmp);
                    mNPCapturePath = Path.GetDirectoryName(tmp.mImage1);
                    //if (!mNPCapturePath.EndsWith(@"\"))
                    //    mNPCapturePath += @"\";
                }

                if (!typesFromCursor((OracleRefCursor)cmd.Parameters["O_ALT_VEHICLE_CURSOR"].Value, ref types))
                    return false;

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                errorCheckFriendly();
                return false;
            }

            return true;
        }

        public bool caseAccept(sSessionInfo session, int offenceSet, int fileNumber, int nextFileNumber, int officerID, string sheetNo, bool sheetNoChanged, ref sCaseInfo caseInfo, sCaptureTypes capType, ref List<sCaptureTypes> types, int printImageID)
        {
            mError = string.Empty;

            types.Clear();

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_CAPTURE_CENTRAL.accept_cap_multi_image", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_USER_ID", mOperatorID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_NAME", mOperatorUsername);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", System.Environment.MachineName);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_OFFENCE_SET", offenceSet);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_CAM_DATE", session.mCamDate);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_CAM_SESSION", session.mCamSession);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_LOCATION_CODE", session.mLocationCode);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_FILE_NUMBER", fileNumber);
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_NEXT_FILE_NUMBER", nextFileNumber);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_VEHICLE_XML", vehicleXML(capType, caseInfo, officerID));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_PRINT_IMAGE_ID", printImageID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_FIELD_SHEET_NO", sheetNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_FIELD_SHEET_CHANGE", sheetNoChanged ? 1 : 0);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);

                // Outputs
                cmd.Parameters.Add(new OracleParameter("O_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_VEHICLE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_ALT_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_ALT_VEHICLE_CURSOR"));

                caseInitialise(out caseInfo);

                cmd.ExecuteNonQuery();

                if (nextFileNumber != -1) // Not last ticket
                {
                    if (!vehicleFromCursor((OracleRefCursor)cmd.Parameters["O_VEHICLE_CURSOR"].Value, ref caseInfo))
                        return false;

                    if (!imagesFromCursor((OracleRefCursor)cmd.Parameters["O_LOCAL_IMAGE_CURSOR"].Value, ref caseInfo))
                        return false;
                    if (!File.Exists(caseInfo.mImage1))
                        imagesFromCursor((OracleRefCursor)cmd.Parameters["O_REMOTE_IMAGE_CURSOR"].Value, ref caseInfo);

                    if (!typesFromCursor((OracleRefCursor)cmd.Parameters["O_ALT_VEHICLE_CURSOR"].Value, ref types))
                        return false;
                }

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                errorCheckFriendly();
                return false;
            }

            return true;
        }

        public bool caseReject(sSessionInfo session, int reasonID, int offenceSet, int fileNumber, int nextFileNumber, int officerID, string sheetNo, bool sheetNoChanged, ref sCaseInfo caseInfo, ref List<sCaptureTypes> types)
        {
            mError = string.Empty;

            types.Clear();

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_CAPTURE_CENTRAL.reject_cap_multi_image", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_USER_ID", mOperatorID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_NAME", mOperatorUsername);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", System.Environment.MachineName);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_OFFENCE_SET", offenceSet);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_CAM_DATE", session.mCamDate);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_CAM_SESSION", session.mCamSession);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_LOCATION_CODE", session.mLocationCode);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_FILE_NUMBER", fileNumber);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_NEXT_FILE_NUMBER", nextFileNumber);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_REJECTION_ID", reasonID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_OFFICER_ID", officerID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_FIELD_SHEET_NO", sheetNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_FIELD_SHEET_CHANGE", sheetNoChanged ? 1 : 0);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);

                // Outputs
                cmd.Parameters.Add(new OracleParameter("O_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_VEHICLE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_ALT_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_ALT_VEHICLE_CURSOR"));

                caseInitialise(out caseInfo);

                cmd.ExecuteNonQuery();

                if (nextFileNumber != -1) // Not last ticket
                {
                    if (!vehicleFromCursor((OracleRefCursor)cmd.Parameters["O_VEHICLE_CURSOR"].Value, ref caseInfo))
                        return false;

                    if (!imagesFromCursor((OracleRefCursor)cmd.Parameters["O_LOCAL_IMAGE_CURSOR"].Value, ref caseInfo))
                        return false;
                    if (!File.Exists(caseInfo.mImage1))
                        imagesFromCursor((OracleRefCursor)cmd.Parameters["O_REMOTE_IMAGE_CURSOR"].Value, ref caseInfo);

                    if (!typesFromCursor((OracleRefCursor)cmd.Parameters["O_ALT_VEHICLE_CURSOR"].Value, ref types))
                        return false;
                }

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                errorCheckFriendly();
                return false;
            }

            return true;
        }

        private string xmlClean(string xml)
        {
            string tmp;

            tmp = xml.Replace("&", "&amp;");
            tmp = tmp.Replace("<", "&lt;");
            tmp = tmp.Replace(">", "&gt;");
            tmp = tmp.Replace("\"", "&quot;");
            tmp = tmp.Replace("'", "&apos;");

            return tmp;
        }

        private string vehicleXML(sCaptureTypes capType, sCaseInfo caseInfo, int officerID)
        {
            string xml = "<main> ";

            xml += "<DATA_RECORD> ";
            xml += "<ID>" + capType.mID.ToString() + "</ID> ";
            xml += "<ZONE>" + caseInfo.mOffenceZone.ToString() + "</ZONE> ";
            xml += "<SPEED>" + caseInfo.mOffenceSpeed.ToString() + "</SPEED> ";
            xml += "<AMOUNT>" + capType.mAmount.ToString("0.00") + "</AMOUNT> ";
            xml += "<DESCRIPTION>" + xmlClean(capType.mDescription) + "</DESCRIPTION> ";
            xml += "<BESKRYWING>" + xmlClean(capType.mBeskrywing) + "</BESKRYWING> ";
            xml += "<INF_DATE>" + caseInfo.mOffenceDate + "</INF_DATE> ";
            xml += "<VEHICLE_REG>" + caseInfo.mVehicleRegNo + "</VEHICLE_REG> ";
            xml += "<OFFICER_ID>" + officerID.ToString() + "</OFFICER_ID> ";
            //xml += "<USER_ID>" + mOperatorID.ToString() + "</USER_ID> ";
            xml += "</DATA_RECORD> ";
            xml += "</main>";

            return xml;
        }

        public bool caseUnlock(string camDate, string camSession, string locationCode)
        {
            mError = string.Empty;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_CAPTURE_CENTRAL.UNLOCK_CAPTURE_USER", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                par = new OracleParameter("P_CAM_DATE", camDate);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_CAM_SESSION", camSession);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_LOCATION_CODE", locationCode);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                cmd.ExecuteScalar();

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                errorCheckFriendly();
                return false;
            }

            return true;
        }

        public bool casePrevious(sSessionInfo session, int offenceSet, int fileNumber, ref sCaseInfo caseInfo, ref List<sCaptureTypes> types)
        {
            mError = string.Empty;

            types.Clear();

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_CAPTURE_CENTRAL.get_last_cap_multi_image", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_USER_ID", mOperatorID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_NAME", mOperatorUsername);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", System.Environment.MachineName);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_OFFENCE_SET", offenceSet);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_FILE_NUMBER", fileNumber);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);

                // Outputs
                cmd.Parameters.Add(new OracleParameter("O_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_VEHICLE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_ALT_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_ALT_VEHICLE_CURSOR"));

                caseInitialise(out caseInfo);

                cmd.ExecuteNonQuery();

                if (!vehicleFromCursor((OracleRefCursor)cmd.Parameters["O_VEHICLE_CURSOR"].Value, ref caseInfo))
                    return false;

                if (!imagesFromCursor((OracleRefCursor)cmd.Parameters["O_LOCAL_IMAGE_CURSOR"].Value, ref caseInfo))
                    return false;
                if (!File.Exists(caseInfo.mImage1))
                    imagesFromCursor((OracleRefCursor)cmd.Parameters["O_REMOTE_IMAGE_CURSOR"].Value, ref caseInfo);

                if (!typesFromCursor((OracleRefCursor)cmd.Parameters["O_ALT_VEHICLE_CURSOR"].Value, ref types))
                    return false;

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                errorCheckFriendly();
                return false;
            }

            return true;
        }

        public bool casesSubmit(string camDate, string camSession, string locationCode, out int captureTotal, out int rejectTotal)
        {
            mError = string.Empty;

            captureTotal = rejectTotal = 0;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_CAPTURE_CENTRAL.SUBMIT_CAPTURE", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                par = new OracleParameter("P_CAM_DATE", camDate);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_CAM_SESSION", camSession);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_LOCATION_CODE", locationCode);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter();
                par.ParameterName = "O_CAPTURE_TOTAL";
                par.Direction = ParameterDirection.Output;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                cmd.Parameters.Add(par);

                par = new OracleParameter();
                par.ParameterName = "O_REJECT_TOTAL";
                par.Direction = ParameterDirection.Output;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                cmd.Parameters.Add(par);

                cmd.ExecuteNonQuery();

                captureTotal = int.Parse(cmd.Parameters[3].Value.ToString());
                rejectTotal = int.Parse(cmd.Parameters[4].Value.ToString());

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                errorCheckFriendly();
                return false;
            }

            return true;
        }

        private bool fileNumbersFromCursor(OracleRefCursor cursor, ref List<int> fileNumbers)
        {
            if (cursor is System.DBNull)
            {
                mError = "No file numbers given.";
                return false;
            }

            OracleDataReader rd = cursor.GetDataReader();

            while (rd.Read())
            {
                fileNumbers.Add((int)rd.GetDecimal(0));
            }

            rd.Close();
            rd.Dispose();

            return (fileNumbers.Count > 0);
        }

        private bool reasonsFromCursor(OracleRefCursor cursor, ref List<sRejectReasons> reasons)
        {
            if (cursor is System.DBNull)
            {
                mError = "No rejection reasons given.";
                return false;
            }

            OracleDataReader rd = cursor.GetDataReader();
            sRejectReasons rsn;

            while (rd.Read())
            {
                rsn.mID = (int)rd.GetDecimal(0);
                rsn.mDescription = rd.GetString(1);
                reasons.Add(rsn);
            }

            rd.Close();
            rd.Dispose();

            if (reasons.Count <= 0)
            {
                mError = "No reject reasons found, contact helpdesk.";
                return false;
            }

            return true;
        }

        private bool officersFromCursor(OracleRefCursor cursor, ref List<sOfficerInfo> officers)
        {
            if (cursor is System.DBNull)
            {
                mError = "No officers given.";
                return false;
            }

            OracleDataReader rd = cursor.GetDataReader();
            sOfficerInfo off;

            while (rd.Read())
            {
                off.mID = (int)rd.GetDecimal(0);
                off.mExternID = rd.GetString(1);
                off.mName = rd.GetString(2);
                off.mSurname = rd.GetString(3);
                officers.Add(off);
            }

            rd.Close();
            rd.Dispose();

            if (officers.Count <= 0)
            {
                mError = "No officers loaded, contact helpdesk.";
                return false;
            }

            return true;
        }

        private bool vehicleFromCursor(OracleRefCursor cursor, ref sCaseInfo vehicle)
        {
            if (cursor is System.DBNull)
            {
                mError = "No vehicle info given.";
                return false;
            }

            OracleDataReader rd = cursor.GetDataReader();
            vehicle.mOffencePlace = vehicle.mOffenceDate = string.Empty;

            if (rd.Read())
            {
                vehicle.mOffenceSpeed = (int)rd.GetDecimal(0);
                vehicle.mOffenceZone = (int)rd.GetDecimal(1);
                vehicle.mOffenceDate = rd.GetString(2);
                vehicle.mOffencePlace = rd.GetString(3);
                vehicle.mPrevRejectID = (int)rd.GetDecimal(4);
                vehicle.mVehicleRegNo = rd.IsDBNull(5) ? string.Empty : rd.GetString(5);
            }

            rd.Close();
            rd.Dispose();

            if ((vehicle.mOffencePlace.Length <= 0) || (vehicle.mOffenceDate.Length <= 0))
            {
                mError = "No vehicle data loaded, contact helpdesk.";
                return false;
            }

            return true;
        }

#if (DEBUG)
        private int imgCnt = 0;
        private void debugGetImageNames(ref sCaseInfo caseInfo)
        {
            imgCnt++;

            string cntStr = imgCnt.ToString("00");

            caseInfo.mImageNP = string.Empty;
            caseInfo.mImage1 = @"C:\Dev\TMT\trunk\iCapture\Doc\Img0" + cntStr + "_0.jpg";
            caseInfo.mImage2 = @"C:\Dev\TMT\trunk\iCapture\Doc\Img0" + cntStr + "_1.jpg";
            if (imgCnt == 1)
                caseInfo.mImage2 = @"C:\Dev\TMT\trunk\iCapture\Doc\Img001.sml_1.jpg";

            if (imgCnt == 2)
            {
                caseInfo.mImage3 = @"C:\Dev\TMT\trunk\iCapture\Doc\Img0" + cntStr + "_2.jpg";
                caseInfo.mImage4 = @"C:\Dev\TMT\trunk\iCapture\Doc\Img0" + cntStr + "_3.jpg";
            }
            else
                caseInfo.mImage3 = caseInfo.mImage4 = string.Empty;

            if (imgCnt > 2)
                imgCnt = 0;
        }
#endif

        private bool imagesFromCursor(OracleRefCursor cursor, ref sCaseInfo caseInfo)
        {
            caseInfo.mImage1 = caseInfo.mImage2 = caseInfo.mImage3 = caseInfo.mImage4 = caseInfo.mImageNP = string.Empty;

            if (cursor is System.DBNull)
            {
                mError = "No image paths given.";
                return false;
            }

            OracleDataReader rd = cursor.GetDataReader();
            int cnt = 0;
            int printImageNo = 0;

            while (rd.Read())
            {
                switch (cnt)
                {
                    case 0:
                        caseInfo.mImage1 = rd.GetString(0);
                        caseInfo.mImage1ID = (int)rd.GetDecimal(1);
                        if (!rd.IsDBNull(2))
                            if (rd.GetDecimal(2) == 1)
                                printImageNo = 0;
                        break;
                    case 1:
                        caseInfo.mImage2 = rd.GetString(0);
                        caseInfo.mImage2ID = (int)rd.GetDecimal(1);
                        if (!rd.IsDBNull(2))
                            if (rd.GetDecimal(2) == 1)
                                printImageNo = 1;
                        break;
                    case 2:
                        caseInfo.mImage3 = rd.GetString(0);
                        caseInfo.mImage3ID = (int)rd.GetDecimal(1);
                        if (!rd.IsDBNull(2))
                            if (rd.GetDecimal(2) == 1)
                                printImageNo = 2;
                        break;
                    case 3:
                        caseInfo.mImage4 = rd.GetString(0);
                        caseInfo.mImage4ID = (int)rd.GetDecimal(1);
                        if (!rd.IsDBNull(2))
                            if (rd.GetDecimal(2) == 1)
                                printImageNo = 3;
                        break;
                    case 4:
                        caseInfo.mImageNP = rd.GetString(0);
                        break;
                }
                cnt++;
            }
            caseInfo.mPrintImageNo = printImageNo;

            rd.Close();
            rd.Dispose();

#if (DEBUG)
            debugGetImageNames(ref caseInfo);
#endif

            caseInfo.mOnlyOneImage = testSinglePhoto(ref caseInfo);
            if (caseInfo.mOnlyOneImage)
                caseInfo.mPrintImageNo = printImageNo = 0;

            // For new SOD images, the number plate image is present
            if (string.IsNullOrEmpty(caseInfo.mImageNP))
            {
                string np = constructNPName(caseInfo.mImage1);

                if (File.Exists(np))
                    caseInfo.mImageNP = np;
            }

            if (cnt <= 0)
            {
                mError = "No images found, contact helpdesk.";
                return false;
            }

            return true;
        }

        private bool testSinglePhoto(ref sCaseInfo caseInfo)
        {
            if (caseInfo.mImage1.IndexOf(".sml") > 0)
                return true;
            else if (caseInfo.mImage2.IndexOf(".sml") > 0)
                return true;

            return false;
        }


        public string constructNPName(string fromName)
        {
            if (mNPCapturedOnCentral && (!string.IsNullOrEmpty(mNPCapturePath))) // Number-plate captured on remote (central)
            {
                fromName = fromName.Replace(Path.GetDirectoryName(fromName), mNPCapturePath);
            }

            if (fromName.IndexOf("_3.jpg") > 1)
                return fromName.Replace("_3.jpg", "_NP.jpg");
            if (fromName.IndexOf("_2.jpg") > 1)
                return fromName.Replace("_2.jpg", "_NP.jpg");
            if (fromName.IndexOf("_1.jpg") > 1)
                return fromName.Replace("_1.jpg", "_NP.jpg");

            return fromName.Replace("_0.jpg", "_NP.jpg");
        }

        private bool typesFromCursor(OracleRefCursor cursor, ref List<sCaptureTypes> types)
        {
            // Types may be none for test photos
            //if (cursor is System.DBNull)
            //{
            //    mError = "No vehicle types given.";
            //    return false;
            //}
            if (cursor is System.DBNull)
                return true;

            OracleDataReader rd = cursor.GetDataReader();
            sCaptureTypes typ;

            while (rd.Read())
            {
                typ.mID = (int)rd.GetDecimal(0);
                typ.mCode = (int)rd.GetDecimal(1);
                typ.mType = rd.GetString(2);
                typ.mAmount = rd.GetDecimal(3);
                typ.mDescription = rd.GetString(4);
                typ.mBeskrywing = rd.GetString(5);
                types.Add(typ);
            }

            rd.Close();
            rd.Dispose();

            //return (types.Count > 0);  Types may be none for test photos
            return true;
        }

    }
}
