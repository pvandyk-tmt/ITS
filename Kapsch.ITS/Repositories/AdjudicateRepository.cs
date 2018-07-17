using Kapsch.Core.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Kapsch.ITS.Repositories
{
    public class AdjudicateRepository
    {
        private readonly DataContext _dbContext;
        private long _credentialID = -1;
        private string _credentialUsername = string.Empty;
        private string _error = string.Empty;
        private List<sCodesCachItem> _codesCache = null;

        public AdjudicateRepository(DataContext dbContext, long credentialID, string credentialUserName)
        {
            this._dbContext = dbContext;
            this._credentialID = credentialID;
            this._credentialUsername = credentialUserName;
        }

        public struct sCaseInfo
        {
            public string mTicketNo;
            public string mNotification;
            public string mVehicleRegNo;
            public bool mVehicleRegNoConfirmed;
            public string mVehicleMake;
            public string mVehicleModel;
            public string mVehicleColour;
            public string mVehicleType;
            public DateTime mVehicleLicenseExpire;
            public int mOffenceSet;
            public DateTime mOffenceDate;
            public int mOffenceSpeed;
            public int mOffenceZone;
            public string mOffenceDirectionLane;
            public int mOffenceCode;
            public string mOffenceNotes;
            public string mOffenceAdditionalsXml;
            public string mImage1;
            public string mImage2;
            public string mImage3;
            public string mImage4;
            public string mImageNP;
            public bool mOnlyOneImage;
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

        public struct sOffenseCode
        {
            public int mCode;
            public decimal mAmount;
            public string mDescription;

            public override string ToString()
            {
                return mCode.ToString();
            }
        }

        private struct sCodesCachItem
        {
            public int mOffenceSet;
            public List<sOffenseCode> mCodes;
        }

        public string Error
        {
            get { return _error; }
        }

        public void caseInitialise(out sCaseInfo caseInfo)
        {
            caseInfo.mTicketNo = string.Empty;
            caseInfo.mOffenceSet = -1;
            caseInfo.mNotification = string.Empty;
            caseInfo.mImage1 = string.Empty;
            caseInfo.mImage2 = string.Empty;
            caseInfo.mImage3 = string.Empty;
            caseInfo.mImage4 = string.Empty;
            caseInfo.mImageNP = string.Empty;
            caseInfo.mVehicleRegNo = string.Empty;
            caseInfo.mVehicleRegNoConfirmed = false;
            caseInfo.mVehicleMake = string.Empty;
            caseInfo.mVehicleModel = string.Empty;
            caseInfo.mVehicleColour = string.Empty;
            caseInfo.mVehicleType = string.Empty;
            caseInfo.mVehicleLicenseExpire = DateTime.MinValue;
            caseInfo.mOffenceDate = DateTime.MinValue;
            caseInfo.mOffenceSpeed = -1;
            caseInfo.mOffenceZone = -1;
            caseInfo.mOffenceDirectionLane = string.Empty;
            caseInfo.mOffenceCode = -1;
            caseInfo.mOffenceNotes = string.Empty;
            caseInfo.mOffenceAdditionalsXml = string.Empty;
            caseInfo.mOnlyOneImage = false;
        }

        public bool caseFirst(ref List<sRejectReasons> reasons, out int count, out sCaseInfo caseInfo, out sCaseInfo remoteCaseInfo, string computerName)
        {
            bool okay = true;

            _error = string.Empty;
            count = 0;
            caseInitialise(out caseInfo);
            caseInitialise(out remoteCaseInfo);

            if (reasons == null)
                reasons = new List<sRejectReasons>();
            reasons.Clear();

            try
            {
                OracleParameter par;
                //var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.GET_ADJUDICATION_LOOKUP_DATA", DbConnection);
                var cmd = new OracleCommand("ITS.OFFENCE_ADJUDICATION.get_adj_lookup_multi_image", DbConnection);
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

                // Outputs
                par = new OracleParameter();
                par.ParameterName = "O_COUNT";
                par.Direction = ParameterDirection.Output;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("O_REJECTION_REASON_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REJECTION_REASON_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_VEHICLE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_OFFENCE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_OFFENCE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));

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

                var ds = new DataSet();
                var da = new OracleDataAdapter(cmd);
                int val = da.Fill(ds);

                //mNPCapturedOnCentral = (int.Parse(cmd.Parameters["O_CENTRAL_INDICATOR"].Value.ToString()) == 1 ? true : false);

                count = int.Parse(cmd.Parameters[2].Value.ToString());
                if (count <= 0)
                    _error = "No cases to Adjudicate exist in database.";
                else
                {
                    caseInfo.mOffenceSet = int.Parse(cmd.Parameters[8].Value.ToString());

                    reasonsFromCursor(ds.Tables[0], ref reasons);
                    vehicleFromCursor(ds.Tables[1], ref caseInfo);
                    offenceFromCursor(ds.Tables[2], ref caseInfo);
                    if (!imagesFromCursor(ds.Tables[3], ds.Tables[4], ref caseInfo, ref remoteCaseInfo))
                        okay = false;
                }

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ErrorCheckFriendly();
                okay = false;
            }

            return okay && (count > 0);
        }

        public bool caseAdjudicate(ref sCaseInfo caseInfo, ref sCaseInfo remoteCaseInfo, out int count, string computerName)
        {
            bool okay = true;

            _error = string.Empty;
            count = 0;

            try
            {
                OracleParameter par;
                //var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.ACCEPT_ADJUDICATION", DbConnection);
                var cmd = new OracleCommand("ITS.OFFENCE_ADJUDICATION.accept_adj_multi_image", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_TICKET_NO", caseInfo.mTicketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", computerName);
                par.OracleDbType = OracleDbType.Varchar2;
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_ID", _credentialID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_ADDITIONALS_XML", caseInfo.mOffenceAdditionalsXml);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_OFFENCE_SET", caseInfo.mOffenceSet);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_NOTES", caseInfo.mOffenceNotes);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                // Outputs
                par = new OracleParameter();
                par.ParameterName = "O_COUNT";
                par.Direction = ParameterDirection.Output;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("O_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_VEHICLE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_OFFENCE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_OFFENCE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));

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

                var ds = new DataSet();
                var da = new OracleDataAdapter(cmd);
                int val = da.Fill(ds);

                //mNPCapturedOnCentral = (int.Parse(cmd.Parameters["O_CENTRAL_INDICATOR"].Value.ToString()) == 1 ? true : false);

                count = int.Parse(cmd.Parameters[6].Value.ToString());
                if (count <= 0)
                    _error = "No more cases to Adjudicate exist in database.";
                else
                {
                    caseInfo.mOffenceSet = int.Parse(cmd.Parameters[11].Value.ToString());

                    vehicleFromCursor(ds.Tables[0], ref caseInfo);
                    offenceFromCursor(ds.Tables[1], ref caseInfo);
                    if (!imagesFromCursor(ds.Tables[2], ds.Tables[3], ref caseInfo, ref remoteCaseInfo))
                        okay = false;
                }

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ErrorCheckFriendly();
                okay = false;
            }

            return okay && (count > 0);
        }

        public bool caseReject(ref sCaseInfo caseInfo, ref sCaseInfo remoteCaseInfo, int reasonID, out int count, string computerName)
        {
            bool okay = true;

            _error = string.Empty;
            count = 0;

            try
            {
                OracleParameter par;
                //var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.REJECT_ADJUDICATION_NOTES", DbConnection);
                var cmd = new OracleCommand("ITS.OFFENCE_ADJUDICATION.reject_adj_multi_image", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_TICKET_NO", caseInfo.mTicketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", computerName);
                par.OracleDbType = OracleDbType.Varchar2;
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_ID", _credentialID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_REJECTION_ID", reasonID); 
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_NOTES", caseInfo.mOffenceNotes);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                // Outputs
                par = new OracleParameter();
                par.ParameterName = "O_COUNT";
                par.Direction = ParameterDirection.Output;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("O_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_VEHICLE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_OFFENCE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_OFFENCE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));

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

                var ds = new DataSet();
                var da = new OracleDataAdapter(cmd);
                int val = da.Fill(ds);

                //mNPCapturedOnCentral = (int.Parse(cmd.Parameters["O_CENTRAL_INDICATOR"].Value.ToString()) == 1 ? true : false);

                count = int.Parse(cmd.Parameters[5].Value.ToString());
                if (count <= 0)
                    _error = "No more cases to Adjudicate exist in database.";
                else
                {
                    caseInfo.mOffenceSet = int.Parse(cmd.Parameters[10].Value.ToString());

                    vehicleFromCursor(ds.Tables[0], ref caseInfo);
                    offenceFromCursor(ds.Tables[1], ref caseInfo);
                    if (!imagesFromCursor(ds.Tables[2], ds.Tables[3], ref caseInfo, ref remoteCaseInfo))
                        okay = false;
                }

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ErrorCheckFriendly();
                okay = false;
            }

            return okay;
        }

        public bool caseUnlock(string ticketNo)
        {
            _error = string.Empty;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_ADJUDICATION.UNLOCK_ADJUDICATION_USER", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                par = new OracleParameter("P_TICKET_NO", ticketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                cmd.ExecuteScalar();
                //cmd.ExecuteNonQuery();

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

         public bool caseAdditionals(int offenceSet, out List<sOffenseCode> codes)
        {
            _error = string.Empty;
            codes = null;
            _codesCache = new List<sCodesCachItem>();

            for (int i = 0; i < _codesCache.Count; i++)
                if ((_codesCache[i].mOffenceSet == offenceSet) && (_codesCache[i].mCodes != null) && (_codesCache[i].mCodes.Count > 0))
                {
                    codes = _codesCache[i].mCodes;
                    return true;
                }

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_ADJUDICATION.GET_ADDITIONAL_CODES", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                par = new OracleParameter("P_OFFENCE_SET", offenceSet);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("O_OFFENCE_CODE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_OFFENCE_CODE_CURSOR"));

                var ds = new DataSet();
                var da = new OracleDataAdapter(cmd);
                int val = da.Fill(ds);

                sCodesCachItem itm;
                itm.mOffenceSet = offenceSet;
                itm.mCodes = new List<sOffenseCode>();
                if (codesFromCursor(ds.Tables[0], ref itm.mCodes))
                {
                    codes = itm.mCodes;
                    _codesCache.Add(itm);
                }
                else
                    _error = "No Offence Codes exist for Offence Set " + offenceSet;

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ErrorCheckFriendly();
                return false;
            }

            return (codes != null);
        }

        private bool offenceFromCursor(DataTable table, ref sCaseInfo caseInfo)
        {
            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];

                caseInfo.mOffenceZone = int.Parse(dr["ZONE"].ToString());
                caseInfo.mOffenceSpeed = int.Parse(dr["SPEED"].ToString());
                string date = dr["OFFENCE_DATE"].ToString();
                caseInfo.mOffenceDate = Convert.ToDateTime(date);
                //caseInfo.mOffenceDate = Convert.ToDateTime(dr["OFFENCE_DATE"]);
                caseInfo.mOffenceCode = int.Parse(dr["OFFENCE_CODE"].ToString());
                caseInfo.mNotification = dr["NOTIFICATION_STATUS"].ToString();
                if (dr["NOTES"] is System.DBNull)
                    caseInfo.mOffenceNotes = string.Empty;
                else
                    caseInfo.mOffenceNotes = dr["NOTES"].ToString();

                return true;
            }

            return false;
        }

        private bool vehicleFromCursor(DataTable table, ref sCaseInfo caseInfo)
        {
            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];

                caseInfo.mTicketNo = dr["TICKET_NO"].ToString();
                caseInfo.mVehicleRegNo = dr["VEHICLE_REGISTRATION"].ToString();
                caseInfo.mVehicleMake = dr["MAKE"].ToString();
                caseInfo.mVehicleModel = dr["MODEL"].ToString();
                caseInfo.mVehicleType = dr["VEHICLE_TYPE"].ToString();
                caseInfo.mVehicleColour = dr["COLOUR"].ToString();
                string date = dr["LICENSE_EXPIRE_DATE"].ToString();
                caseInfo.mVehicleLicenseExpire = Convert.ToDateTime(date);
                //caseInfo.mVehicleLicenseExpire = Convert.ToDateTime(dr["LICENSE_EXPIRE_DATE"]);
                caseInfo.mVehicleRegNoConfirmed = (int.Parse(dr["REGISTRATION_CHECK"].ToString()) == 0 ? true : false);

                return true;
            }

            return false;
        }

        private bool imagesFromCursor(DataTable local, DataTable remote, ref sCaseInfo caseInfo, ref sCaseInfo remoteCaseInfo)
        {
            caseInfo.mImage1 = caseInfo.mImage2 = caseInfo.mImage3 = caseInfo.mImage4 = caseInfo.mImageNP = string.Empty;
            remoteCaseInfo.mImage1 = remoteCaseInfo.mImage2 = remoteCaseInfo.mImage3 = remoteCaseInfo.mImage4 = remoteCaseInfo.mImageNP = string.Empty;

            if (local == null)
                return false;
            if (local.Rows.Count <= 0)
                return false;

            // Check if image exist local and if not use remote path for images
            DataRow dr;
            string localImage1 = string.Empty;
            string localImage2 = string.Empty;
            string localImage3 = string.Empty;
            string localImage4 = string.Empty;
            //string localImageNP = string.Empty;
            string remoteImage1 = string.Empty;
            string remoteImage2 = string.Empty;
            string remoteImage3 = string.Empty;
            string remoteImage4 = string.Empty;
            //string remoteImageNP = string.Empty;

            for (int i = 0; i < local.Rows.Count; i++)
            {
                dr = local.Rows[i];
                switch (i)
                {
                    case 0:
                        localImage1 = dr[0].ToString();
                        break;
                    case 1:
                        localImage2 = dr[0].ToString();
                        break;
                    case 2:
                        localImage3 = dr[0].ToString();
                        break;
                    case 3:
                        localImage4 = dr[0].ToString();
                        break;
                    //case 4:
                    //    localImageNP = dr[0].ToString();
                    //    break;
                }
            }

            for (int i = 0; i < remote.Rows.Count; i++)
            {
                dr = remote.Rows[i];
                switch (i)
                {
                    case 0:
                        remoteImage1 = dr[0].ToString();
                        break;
                    case 1:
                        remoteImage2 = dr[0].ToString();
                        break;
                    case 2:
                        remoteImage3 = dr[0].ToString();
                        break;
                    case 3:
                        remoteImage4 = dr[0].ToString();
                        break;
                    //case 4:
                    //    remoteImageNP = dr[0].ToString();
                    //    break;
                }
            }

            caseInfo.mImage1 = localImage1;
            caseInfo.mImage2 = localImage2;
            caseInfo.mImage3 = localImage3;
            caseInfo.mImage4 = localImage4;

            remoteCaseInfo.mImage1 = remoteImage1;
            remoteCaseInfo.mImage2 = remoteImage2;
            remoteCaseInfo.mImage3 = remoteImage3;
            remoteCaseInfo.mImage4 = remoteImage4;

           string localNP = constructNPName(localImage1);
           string remotNP = constructNPName(remoteImage1);

            caseInfo.mImageNP = localNP;
            remoteCaseInfo.mImageNP = remotNP;

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

        private string constructNPName(string fromName)
        {
            //if (mNPCapturedOnCentral && (!string.IsNullOrEmpty(mNPCapturePath))) // Number-plate captured on remote (central)
            //{
            //    fromName = fromName.Replace(Path.GetDirectoryName(fromName), mNPCapturePath);
            //}

            if (fromName.IndexOf("_3.jpg") > 1)
                return fromName.Replace("_3.jpg", "_NP.jpg");
            if (fromName.IndexOf("_2.jpg") > 1)
                return fromName.Replace("_2.jpg", "_NP.jpg");
            if (fromName.IndexOf("_1.jpg") > 1)
                return fromName.Replace("_1.jpg", "_NP.jpg");

            return fromName.Replace("_0.jpg", "_NP.jpg");
        }

        private bool reasonsFromCursor(DataTable table, ref List<sRejectReasons> reasons)
        {
            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr;
                sRejectReasons rsn;

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    dr = table.Rows[i];
                    rsn.mID = int.Parse(dr["ID"].ToString());
                    rsn.mDescription = dr["DESCRIPTION"].ToString();
                    reasons.Add(rsn);
                }

                return true;
            }

            return false;
        }

        private bool codesFromCursor(DataTable table, ref List<sOffenseCode> codes)
        {
            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr;
                sOffenseCode off;

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    dr = table.Rows[i];
                    off.mCode = int.Parse(dr["CODE"].ToString());
                    off.mAmount = decimal.Parse(dr["FINE_AMOUNT"].ToString());
                    off.mDescription = dr["DESCRIPTION"].ToString();
                    codes.Add(off);
                }

                return true;
            }

            return false;
        }

        public class cFishpondInfo
        {
            public string pTicketNo { get; set; }
            public string pVehicleRegistration { get; set; }
            public DateTime pTicketDate { get; set; }
            public string pVehicleMake { get; set; }
            public string pVehicleModel { get; set; }
            public string pRejectReason { get; set; }
            public string pRejectBy { get; set; }
            public DateTime pVerifyDate { get; set; }
            public int pTimesRejected { get; set; }
            public string pLockedBy { get; set; }
        }

        public bool fishpondGetCases(ref List<sRejectReasons> reasons, ref List<cFishpondInfo> cases)
        {
            bool okay = true;

            if (cases == null)
                cases = new List<cFishpondInfo>();
            cases.Clear();

            if (reasons == null)
                reasons = new List<sRejectReasons>();
            reasons.Clear();

            //if (!connect())
            //    return false;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_ADJUDICATION.get_fishpond_lookup_data", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("p_user_id", _credentialID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("o_rejection_reason_cursor", OracleDbType.RefCursor, 0,
                                       ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current,
                                       "o_rejection_reason_cursor"));

                cmd.Parameters.Add(new OracleParameter("o_result_cursor", OracleDbType.RefCursor, 0,
                                       ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current,
                                       "o_result_cursor"));

                cmd.ExecuteNonQuery();

                if (!(cmd.Parameters["o_rejection_reason_cursor"].Value is DBNull))
                {
                    var curs = (OracleRefCursor)cmd.Parameters["o_rejection_reason_cursor"].Value;

                    OracleDataReader rd = curs.GetDataReader();
                    sRejectReasons reas;

                    while (rd.Read())
                    {
                        reas.mID = (int)rd.GetDecimal(0);
                        reas.mDescription = rd.IsDBNull(1) ? string.Empty : rd.GetString(1);

                        reasons.Add(reas);
                    }

                    rd.Close();
                    rd.Dispose();
                    curs.Dispose();
                }


                if (!(cmd.Parameters["o_result_cursor"].Value is DBNull))
                {
                    var curs = (OracleRefCursor)cmd.Parameters["o_result_cursor"].Value;

                    OracleDataReader rd = curs.GetDataReader();
                    cFishpondInfo pondCase;

                    while (rd.Read())
                    {
                        pondCase = new cFishpondInfo();
                        pondCase.pTicketNo = rd.GetString(0);
                        pondCase.pVehicleRegistration = rd.IsDBNull(1) ? string.Empty : rd.GetString(1);
                        pondCase.pTicketDate = rd.IsDBNull(2) ? DateTime.MinValue : rd.GetDateTime(2);
                        pondCase.pVehicleMake = rd.IsDBNull(3) ? string.Empty : rd.GetString(3);
                        pondCase.pVehicleModel = rd.IsDBNull(4) ? string.Empty : rd.GetString(4);
                        pondCase.pRejectReason = rd.IsDBNull(5) ? string.Empty : rd.GetString(5);
                        pondCase.pRejectBy = rd.IsDBNull(6) ? string.Empty : rd.GetString(6);
                        pondCase.pVerifyDate = rd.IsDBNull(7) ? DateTime.MinValue : rd.GetDateTime(7);
                        pondCase.pTimesRejected = rd.IsDBNull(8) ? 0 : (int)rd.GetDecimal(8);
                        pondCase.pLockedBy = rd.IsDBNull(9) ? string.Empty : rd.GetString(9);
                        cases.Add(pondCase);
                    }

                    rd.Close();
                    rd.Dispose();
                    curs.Dispose();
                }

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ErrorCheckFriendly();
                okay = false;
            }

            //disconnect();

            return okay;
        }

        public bool fishpondGetCase(string ticketNo, ref sCaseInfo caseInfo, ref sCaseInfo remoteCaseInfo, string computerName)
        {
            bool okay = true;

            _error = string.Empty;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_ADJUDICATION.lock_fishpond_user", DbConnection);
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

                par = new OracleParameter("P_TICKET_NO", ticketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                // Outputs
                cmd.Parameters.Add(new OracleParameter("O_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_VEHICLE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_OFFENCE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_OFFENCE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));

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

                var ds = new DataSet();
                var da = new OracleDataAdapter(cmd);
                int val = da.Fill(ds);

                //mNPCapturedOnCentral = (int.Parse(cmd.Parameters["O_CENTRAL_INDICATOR"].Value.ToString()) == 1 ? true : false);

                caseInitialise(out caseInfo);

                caseInfo.mTicketNo = ticketNo;

                caseInfo.mOffenceSet = int.Parse(cmd.Parameters[8].Value.ToString()); //Sit hierdie later terug en sien of dit werk (of vice versa)

                vehicleFromCursor(ds.Tables[0], ref caseInfo);
                offenceFromCursor(ds.Tables[1], ref caseInfo);
                if (!imagesFromCursor(ds.Tables[2], ds.Tables[3], ref caseInfo, ref remoteCaseInfo))
                    okay = false;

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                ErrorCheckFriendly();
                okay = false;
            }

            //disconnect();

            return okay;
        }

        public bool fishpondCaseReject(string ticketNumber, int reasonID, string notes, string computerName)
        {
            //if (!connect())
            //    return false;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_ADJUDICATION.reject_adjudication_fishpond", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_TICKET_NO", ticketNumber);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME",computerName);
                par.OracleDbType = OracleDbType.Varchar2;
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_ID", _credentialID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_REJECTION_ID", reasonID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_NOTES", notes);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
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

            //disconnect();

            return true;
        }

        public bool fishpondCaseAccept(sCaseInfo caseInfo, string computerName)
        {
            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_ADJUDICATION.accept_adjudication_fishpond", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_TICKET_NO", caseInfo.mTicketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", computerName);
                par.OracleDbType = OracleDbType.Varchar2;
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_ID", _credentialID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_ADDITIONALS_XML", caseInfo.mOffenceAdditionalsXml);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_OFFENCE_SET", caseInfo.mOffenceSet);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_NOTES", caseInfo.mOffenceNotes);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
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

            //disconnect();

            return true;
        }

        public bool fishpondUnlock(string ticketNo)
        {
            _error = string.Empty;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.OFFENCE_ADJUDICATION.unlock_fishpond_user", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                par = new OracleParameter("P_TICKET_NO", ticketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                cmd.ExecuteScalar();
                //cmd.ExecuteNonQuery();

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
            if (_error.StartsWith("ORA-"))
            {
                string errorTemp = _error;
                string errorCode = errorTemp.TrimStart("ORA-".ToCharArray());
                errorCode = errorCode.Remove(errorCode.IndexOf(":"), errorCode.Substring(errorCode.IndexOf(":")).Length);
                int errorNumber = Convert.ToInt32(errorCode);

                if (errorNumber <= 21000)
                {
                    int s = _error.IndexOf("-");

                    if (s > 0)
                    {
                        s++;
                        _error = _error.Substring(s, _error.Length - s);
                    }

                    if (_error.IndexOf("\n") > 0)
                    {
                        _error = _error.Substring(0, _error.IndexOf("\n") + 1);
                    }
                    _error = _error.Trim();
                }
            }
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

    }
}
