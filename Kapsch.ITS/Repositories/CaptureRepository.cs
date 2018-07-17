using Kapsch.Core.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Repositories
{
    public class CaptureRepository
    {
        private readonly DataContext _dbContext;
        private string _error = string.Empty;
        private long _credentialID = -1;
        private string _credentialUsername = string.Empty;
        private bool _numberPlateCapturedOnCentral = false;     // Number-plate image captured at central
        private string _numberPlateCapturePath = string.Empty;  // Number-plate image path to central

        public CaptureRepository(DataContext dbContext, long credentialID, string credentialUserName)
        {
            this._dbContext = dbContext;
            this._credentialID = credentialID;
            this._credentialUsername = credentialUserName;
        }

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
            public string mCode;
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
            public long mCredentialID;
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

        /// <summary>
        /// Gets the last error in the data layer.
        /// </summary>
        public string Error
        {
            get { return _error; }
        }

        public void CaseInitialise(out sCaseInfo caseInfo)
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

        public void TypeInitialise(out sCaptureTypes type)
        {
            type.mID = -1;

            type.mCode = string.Empty;
            type.mType = string.Empty;
            type.mDescription = string.Empty;
            type.mBeskrywing = string.Empty;
            type.mAmount = 0;
        }

        public bool Sessions(out string[] headings, ref List<sSessionInfo> sessions)
        {
            headings = null;
            sessions.Clear();
            _error = string.Empty;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_CAPTURE.GET_CAPTURE_SCREEN", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_USER_ID", _credentialID);
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
                        _error += "LOCATION_CODE could not be found in the data.";
                    if (camDateIndex < 0)
                        _error += "CAM_DATE could not be found in the data.";
                    if (camSessionIndex < 0)
                        _error += "CAM_SESSION could not be found in the data.";
                    if (nothingDoneIndex < 0)
                        _error += "NOTHING_DONE could not be found in the data.";
                    if (machineIdIndex < 0)
                        _error += "MACHINE_ID could not be found in the data.";
                    if (_error.Length > 0)
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
                    _error = "No sessions for capturing exist.";
                    return false;
                }

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ErrorCheckFriendly();
                return false;
            }

            return true;
        }

        private void ErrorCheckFriendly()
        {
            if (_error.StartsWith("ORA-20002:") ||
                _error.StartsWith("ORA-20030:") ||
                _error.StartsWith("ORA-20031:") ||
                _error.StartsWith("ORA-20032:") ||
                _error.StartsWith("ORA-20033:") ||
                _error.StartsWith("ORA-20034:") ||
                _error.StartsWith("ORA-20035:"))
            {
                int s = _error.IndexOf(":");

                if (s > 0)
                {
                    s++;
                    _error = _error.Substring(s, _error.Length - s);
                }
                _error = _error.Substring(0, _error.IndexOf("\n") + 1);
                _error = _error.Trim();
            }
        }

        public bool CaseFirst(sSessionInfo session, bool loadNewOnly, ref List<sRejectReasons> reasons, ref List<sCaptureTypes> types, ref List<int> fileNumbers, ref List<sOfficerInfo> officers, ref sCaseInfo caseInfo, ref sCaseInfo remoteCaseInfo, out int offenceSet, out int startIndex, string computerName)
        {
            _error = string.Empty;
            offenceSet = -1;
            startIndex = 0;

            CaseInitialise(out caseInfo);
            CaseInitialise(out remoteCaseInfo);

            fileNumbers.Clear();
            reasons.Clear();
            types.Clear();

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_CAPTURE.get_cap_multi_image_lookup", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_USER_ID", _credentialID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", computerName);
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
                _numberPlateCapturedOnCentral = (int.Parse(cmd.Parameters["O_CENTRAL_INDICATOR"].Value.ToString()) == 1 ? true : false);

                FileNumbersFromCursor((OracleRefCursor)cmd.Parameters["O_HOLDING_CURSOR"].Value, ref fileNumbers);
                startIndex = fileNumbers.Count;
                fileNumbers.Add(firstFileNumber);
                FileNumbersFromCursor((OracleRefCursor)cmd.Parameters["O_NUMBER_CURSOR"].Value, ref fileNumbers);

                if (!ReasonsFromCursor((OracleRefCursor)cmd.Parameters["O_REJECTION_REASON_CURSOR"].Value, ref reasons))
                    return false;

                if (!OfficersFromCursor((OracleRefCursor)cmd.Parameters["O_OFFICER_CURSOR"].Value, ref officers))
                    return false;

                if (!VehicleFromCursor((OracleRefCursor)cmd.Parameters["O_VEHICLE_CURSOR"].Value, ref caseInfo))
                    return false;

                if (!ImagesFromCursor((OracleRefCursor)cmd.Parameters["O_LOCAL_IMAGE_CURSOR"].Value, ref caseInfo))
                    return false;

                if (!ImagesFromCursor((OracleRefCursor)cmd.Parameters["O_REMOTE_IMAGE_CURSOR"].Value, ref remoteCaseInfo))
                    return false;

                if (!TypesFromCursor((OracleRefCursor)cmd.Parameters["O_ALT_VEHICLE_CURSOR"].Value, ref types))
                    return false;

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ErrorCheckFriendly();
                return false;
            }

            return true;
        }

        public bool CaseAccept(sSessionInfo session, int offenceSet, int fileNumber, int nextFileNumber, int officerID, string sheetNo, bool sheetNoChanged, ref sCaseInfo caseInfo, ref sCaseInfo remoteCaseInfo, sCaptureTypes capType, ref List<sCaptureTypes> types, int printImageID, string computerName)
        {
            _error = string.Empty;

            types.Clear();

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_CAPTURE.accept_cap_multi_image", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_USER_ID", _credentialID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_NAME", _credentialUsername);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", computerName);
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
                par = new OracleParameter("P_VEHICLE_XML", VehicleXML(capType, caseInfo, officerID));
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

                CaseInitialise(out caseInfo);

                cmd.ExecuteNonQuery();

                if (nextFileNumber != -1) // Not last ticket
                {
                    if (!VehicleFromCursor((OracleRefCursor)cmd.Parameters["O_VEHICLE_CURSOR"].Value, ref caseInfo))
                        return false;

                    if (!ImagesFromCursor((OracleRefCursor)cmd.Parameters["O_LOCAL_IMAGE_CURSOR"].Value, ref caseInfo))
                        return false;

                    if (!ImagesFromCursor((OracleRefCursor)cmd.Parameters["O_REMOTE_IMAGE_CURSOR"].Value, ref remoteCaseInfo))
                        return false;

                    if (!TypesFromCursor((OracleRefCursor)cmd.Parameters["O_ALT_VEHICLE_CURSOR"].Value, ref types))
                        return false;
                }

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ErrorCheckFriendly();
                return false;
            }

            return true;
        }

        public bool CaseReject(sSessionInfo session, int reasonID, int offenceSet, int fileNumber, int nextFileNumber, int officerID, string sheetNo, bool sheetNoChanged, ref sCaseInfo caseInfo, ref sCaseInfo remoteCaseInfo, ref List<sCaptureTypes> types, string computerName)
        {
            _error = string.Empty;

            types.Clear();

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_CAPTURE.reject_cap_multi_image", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_USER_ID", _credentialID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_NAME", _credentialUsername);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", computerName);
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

                CaseInitialise(out caseInfo);

                cmd.ExecuteNonQuery();

                if (nextFileNumber != -1) // Not last ticket
                {
                    if (!VehicleFromCursor((OracleRefCursor)cmd.Parameters["O_VEHICLE_CURSOR"].Value, ref caseInfo))
                        return false;

                    if (!ImagesFromCursor((OracleRefCursor)cmd.Parameters["O_LOCAL_IMAGE_CURSOR"].Value, ref caseInfo))
                        return false;

                    if (!ImagesFromCursor((OracleRefCursor)cmd.Parameters["O_REMOTE_IMAGE_CURSOR"].Value, ref remoteCaseInfo))
                        return false;

                    if (!TypesFromCursor((OracleRefCursor)cmd.Parameters["O_ALT_VEHICLE_CURSOR"].Value, ref types))
                        return false;
                }

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ErrorCheckFriendly();
                return false;
            }

            return true;
        }

        private string XmlClean(string xml)
        {
            string tmp;

            tmp = xml.Replace("&", "&amp;");
            tmp = tmp.Replace("<", "&lt;");
            tmp = tmp.Replace(">", "&gt;");
            tmp = tmp.Replace("\"", "&quot;");
            tmp = tmp.Replace("'", "&apos;");

            return tmp;
        }

        private string VehicleXML(sCaptureTypes capType, sCaseInfo caseInfo, int officerID)
        {
            string xml = "<main> ";

            xml += "<DATA_RECORD> ";
            xml += "<ID>" + capType.mID.ToString() + "</ID> ";
            xml += "<ZONE>" + caseInfo.mOffenceZone.ToString() + "</ZONE> ";
            xml += "<SPEED>" + caseInfo.mOffenceSpeed.ToString() + "</SPEED> ";
            xml += "<AMOUNT>" + capType.mAmount.ToString("0.00") + "</AMOUNT> ";
            xml += "<DESCRIPTION>" + XmlClean(capType.mDescription) + "</DESCRIPTION> ";
            xml += "<BESKRYWING>" + XmlClean(capType.mBeskrywing) + "</BESKRYWING> ";
            xml += "<INF_DATE>" + caseInfo.mOffenceDate + "</INF_DATE> ";
            xml += "<VEHICLE_REG>" + caseInfo.mVehicleRegNo + "</VEHICLE_REG> ";
            xml += "<OFFICER_ID>" + officerID.ToString() + "</OFFICER_ID> ";
            //xml += "<USER_ID>" + mOperatorID.ToString() + "</USER_ID> ";
            xml += "</DATA_RECORD> ";
            xml += "</main>";

            return xml;
        }

        public bool CaseUnlock(string camDate, string camSession, string locationCode)
        {
            _error = string.Empty;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_CAPTURE.UNLOCK_CAPTURE_USER", DbConnection);
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
                _error = ex.Message;
                ErrorCheckFriendly();
                return false;
            }

            return true;
        }

        public bool NPImageSave (string mimeType, string mimeDataPath, string fileName, int evidenceFileNumber)
        {
            _error = string.Empty;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_CAPTURE.SUBMIT_NUMBERPLATE_EVIDENCE", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                par = new OracleParameter("P_MIME_TYPE", mimeType);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_MIME_DATA_PATH", mimeDataPath);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_FILENAME", fileName);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_EVIDENCE_FILE_NUMBER", evidenceFileNumber);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);

                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ErrorCheckFriendly();
                return false;
            }

            return true;
        }

        public bool CasePrevious(sSessionInfo session, int offenceSet, int fileNumber, ref sCaseInfo caseInfo, ref sCaseInfo remoteCaseInfo, ref List<sCaptureTypes> types, string computerName)
        {
            _error = string.Empty;

            types.Clear();

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_CAPTURE.get_last_cap_multi_image", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_USER_ID", _credentialID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_NAME", _credentialUsername);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", computerName);
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

                CaseInitialise(out caseInfo);

                cmd.ExecuteNonQuery();

                if (!VehicleFromCursor((OracleRefCursor)cmd.Parameters["O_VEHICLE_CURSOR"].Value, ref caseInfo))
                    return false;

                if (!ImagesFromCursor((OracleRefCursor)cmd.Parameters["O_LOCAL_IMAGE_CURSOR"].Value, ref caseInfo))
                    return false;

                if (!ImagesFromCursor((OracleRefCursor)cmd.Parameters["O_REMOTE_IMAGE_CURSOR"].Value, ref remoteCaseInfo))
                    return false;

                if (!TypesFromCursor((OracleRefCursor)cmd.Parameters["O_ALT_VEHICLE_CURSOR"].Value, ref types))
                    return false;

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ErrorCheckFriendly();
                return false;
            }

            return true;
        }

        public bool CasesSubmit(string camDate, string camSession, string locationCode, out int captureTotal, out int rejectTotal)
        {
            _error = string.Empty;

            captureTotal = rejectTotal = 0;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_CAPTURE.SUBMIT_CAPTURE", DbConnection);
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
                _error = ex.Message;
                ErrorCheckFriendly();
                return false;
            }

            return true;
        }

        private bool FileNumbersFromCursor(OracleRefCursor cursor, ref List<int> fileNumbers)
        {
            if (cursor is System.DBNull)
            {
                _error = "No file numbers given.";
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

        private bool ReasonsFromCursor(OracleRefCursor cursor, ref List<sRejectReasons> reasons)
        {
            if (cursor is System.DBNull)
            {
                _error = "No rejection reasons given.";
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
                _error = "No reject reasons found, contact helpdesk.";
                return false;
            }

            return true;
        }

        private bool OfficersFromCursor(OracleRefCursor cursor, ref List<sOfficerInfo> officers)
        {
            if (cursor is System.DBNull)
            {
                _error = "No officers given.";
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
                off.mCredentialID = (long)rd.GetDecimal(4);
                officers.Add(off);
            }

            rd.Close();
            rd.Dispose();

            if (officers.Count <= 0)
            {
                _error = "No officers loaded, contact helpdesk.";
                return false;
            }

            return true;
        }

        private bool VehicleFromCursor(OracleRefCursor cursor, ref sCaseInfo vehicle)
        {
            if (cursor is System.DBNull)
            {
                _error = "No vehicle info given.";
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
                _error = "No vehicle data loaded, contact helpdesk.";
                return false;
            }

            return true;
        }

#if (DEBUG)
        //private int imgCnt = 0;
        //private void debugGetImageNames(ref sCaseInfo caseInfo)
        //{
        //    imgCnt++;

        //    string cntStr = imgCnt.ToString("00");

        //    caseInfo.mImageNP = string.Empty;
        //    caseInfo.mImage1 = @"C:\Development\iCapture\Doc\Img0" + cntStr + "_0.jpg";
        //    caseInfo.mImage2 = @"C:\Development\iCapture\Doc\Img0" + cntStr + "_1.jpg";
        //    if (imgCnt == 1)
        //        caseInfo.mImage2 = @"C:\Development\iCapture\Doc\Img001.sml_1.jpg";

        //    if (imgCnt == 2)
        //    {
        //        caseInfo.mImage3 = @"C:\Development\iCapture\Doc\Img0" + cntStr + "_2.jpg";
        //        caseInfo.mImage4 = @"C:\Development\iCapture\Doc\Img0" + cntStr + "_3.jpg";
        //    }
        //    else
        //        caseInfo.mImage3 = caseInfo.mImage4 = string.Empty;

        //    if (imgCnt > 2)
        //        imgCnt = 0;
        //}
#endif

        private bool ImagesFromCursor(OracleRefCursor cursor, ref sCaseInfo caseInfo)
        {
            caseInfo.mImage1 = caseInfo.mImage2 = caseInfo.mImage3 = caseInfo.mImage4 = caseInfo.mImageNP = string.Empty;

            if (cursor is System.DBNull)
            {
                _error = "No image paths given.";
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
            //debugGetImageNames(ref caseInfo);
#endif

            caseInfo.mOnlyOneImage = TestSinglePhoto(ref caseInfo);
            if (caseInfo.mOnlyOneImage)
                caseInfo.mPrintImageNo = printImageNo = 0;

            return true;
        }

        private bool TestSinglePhoto(ref sCaseInfo caseInfo)
        {
            if (caseInfo.mImage1.IndexOf(".sml") > 0)
                return true;
            else if (caseInfo.mImage2.IndexOf(".sml") > 0)
                return true;

            return false;
        }


        public string ConstructNPName(string fromName)
        {
            if (_numberPlateCapturedOnCentral && (!string.IsNullOrEmpty(_numberPlateCapturePath))) // Number-plate captured on remote (central)
            {
                fromName = fromName.Replace(Path.GetDirectoryName(fromName), _numberPlateCapturePath);
            }

            if (fromName.IndexOf("_3.jpg") > 1)
                return fromName.Replace("_3.jpg", "_NP.jpg");
            if (fromName.IndexOf("_2.jpg") > 1)
                return fromName.Replace("_2.jpg", "_NP.jpg");
            if (fromName.IndexOf("_1.jpg") > 1)
                return fromName.Replace("_1.jpg", "_NP.jpg");

            return fromName.Replace("_0.jpg", "_NP.jpg");
        }

        private bool TypesFromCursor(OracleRefCursor cursor, ref List<sCaptureTypes> types)
        {
            // Types may be none for test photos
            //if (cursor is System.DBNull)
            //{
            //    _error = "No vehicle types given.";
            //    return false;
            //}
            if (cursor is System.DBNull)
                return true;

            OracleDataReader rd = cursor.GetDataReader();
            sCaptureTypes typ;

            while (rd.Read())
            {
                typ.mID = (int)rd.GetDecimal(0);
                typ.mCode = (string)rd.GetString(1);
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

        private OracleConnection DbConnection
        {
            get
            {
                var dbConnection = _dbContext.Database.Connection as OracleConnection;
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }

                return dbConnection;
            }
        }

        public bool NumberPlateCapturedOnCentral
        {
            get { return _numberPlateCapturedOnCentral; }
        }

        public string NumberPlateCapturePath
        {
            get { return _numberPlateCapturePath; }
        }
    }
}
