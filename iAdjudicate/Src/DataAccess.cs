using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Reflection;

namespace TMT.iAdjudicate
{
    public class cDataAccess
    {
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

        private OracleConnection mDBConnection = null;

        private string mConnectionString = string.Empty;
        private string mError = string.Empty;
        private int mOperatorID = -1;
        private List<sCodesCachItem> mCodesCache = null;
        //private bool mNPCapturedOnCentral = false;     // Number-plate image captured at central
        //private string mNPCapturePath = string.Empty;  // Number-plate image path to central

        ///// <summary>
        ///// Check if the number-plate capturing is done on central.
        ///// </summary>
        //public bool pNPCapturedOnCentral
        //{
        //    get { return mNPCapturedOnCentral; }
        //}

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
            mCodesCache = new List<sCodesCachItem>();
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

        public bool mayAdjudicate(string username, string version)
        {
            mError = string.Empty;

            try
            {
                OracleCommand cmd = new OracleCommand("SELECT ACTIVITY_ID FROM GISMOEMPLOYEE_ACTIVITY WHERE EMPLOYEE_ID=" + mOperatorID + " AND ACTIVITY_ID=101", mDBConnection);

                object val = cmd.ExecuteScalar();

                cmd.Dispose();

                if ((val == null) || (val is System.DBNull))
                {
                    mError = "Username '" + username + "' may not do Adjudications.";
                    return false;
                }

                //cmd = new OracleCommand("SELECT VERSION FROM GISMO.IADJUDICATE_VERSION_CONTROL", mDBConnection);

                //val = cmd.ExecuteScalar();

                //cmd.Dispose();

                //if ((val == null) || (val is System.DBNull))
                //{
                //    mError = "Version for iAdjudicate could not be found.";
                //    return false;
                //}
                //else
                //{
                    //string ver = val.ToString();

                    //if (version.StartsWith(ver))
                    //    return true;
                    //else
                    //{
                    //    mError = "Version for iAdjudicate '" + ver + "' must be '" + version + "'.";
                    //    return false;
                    //}
                //}
            }
            catch (Exception ex)
            {
                mError = "mayAdjudicate : " + ex.Message;
                return false;
            }

            return true;
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

        public bool caseFirst(ref List<sRejectReasons> reasons, out int count, out sCaseInfo caseInfo)
        {
            bool okay = true;

            mError = string.Empty;
            count = 0;
            caseInitialise(out caseInfo);

            if (reasons == null)
                reasons = new List<sRejectReasons>();
            reasons.Clear();

            try
            {
                OracleParameter par;
                //var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.GET_ADJUDICATION_LOOKUP_DATA", mDBConnection);
                var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.get_adj_lookup_multi_image", mDBConnection);
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
                    mError = "No cases to Adjudicate exist in database.";
                else
                {
                    caseInfo.mOffenceSet = int.Parse(cmd.Parameters[8].Value.ToString());

                    reasonsFromCursor(ds.Tables[0], ref reasons);
                    vehicleFromCursor(ds.Tables[1], ref caseInfo);
                    offenceFromCursor(ds.Tables[2], ref caseInfo);
                    if (!imagesFromCursor(ds.Tables[3], ds.Tables[4], ref caseInfo))
                        okay = false;

                    //imagesFromCursor(ds.Tables[3], ref caseInfo);

                    //// Images points to local, if they do not exit on local, point to remote
                    //if (!File.Exists(caseInfo.mImage1))
                    //    imagesFromCursor(ds.Tables[4], ref caseInfo);
                    //else if (!File.Exists(caseInfo.mImageNP)) // if numberplate image does not exist in local but image1 does then check if numberplate image does and if not point NP to central/remote
                    //{
                    //    sCaseInfo tmp;

                    //    caseInitialise(out tmp);

                    //    imagesFromCursor(ds.Tables[4], ref tmp);
                    //    caseInfo.mImageNP = tmp.mImageNP;
                    //}
                }

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                okay = false;
            }

            return okay && (count > 0);
        }

        public bool caseAdjudicate(ref sCaseInfo caseInfo, out int count)
        {
            bool okay = true;

            mError = string.Empty;
            count = 0;

            try
            {
                OracleParameter par;
                //var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.ACCEPT_ADJUDICATION", mDBConnection);
                var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.accept_adj_multi_image", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_TICKET_NO", caseInfo.mTicketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", System.Environment.MachineName);
                par.OracleDbType = OracleDbType.Varchar2;
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_ID", mOperatorID);
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
                    mError = "No more cases to Adjudicate exist in database.";
                else
                {
                    caseInfo.mOffenceSet = int.Parse(cmd.Parameters[11].Value.ToString());

                    vehicleFromCursor(ds.Tables[0], ref caseInfo);
                    offenceFromCursor(ds.Tables[1], ref caseInfo);
                    if (!imagesFromCursor(ds.Tables[2], ds.Tables[3], ref caseInfo))
                        okay = false;

                    //imagesFromCursor(ds.Tables[2], ref caseInfo);

                    //// Images points to local, if they do not exit on local, point to remote
                    //if (!File.Exists(caseInfo.mImage1))
                    //    imagesFromCursor(ds.Tables[3], ref caseInfo);
                    //else if (!File.Exists(caseInfo.mImageNP)) // if numberplate image does not exist in local but image1 does then check if numberplate image does and if not point NP to central/remote
                    //{
                    //    sCaseInfo tmp;

                    //    caseInitialise(out tmp);

                    //    imagesFromCursor(ds.Tables[3], ref tmp);
                    //    caseInfo.mImageNP = tmp.mImageNP;
                    //}
                }

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                okay = false;
            }

            return okay && (count > 0);
        }

        public bool caseReject(ref sCaseInfo caseInfo, int reasonID, out int count)
        {
            bool okay = true;

            mError = string.Empty;
            count = 0;

            try
            {
                OracleParameter par;
                //var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.REJECT_ADJUDICATION_NOTES", mDBConnection);
                var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.reject_adj_multi_image", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_TICKET_NO", caseInfo.mTicketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", System.Environment.MachineName);
                par.OracleDbType = OracleDbType.Varchar2;
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_ID", mOperatorID);
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
                    mError = "No more cases to Adjudicate exist in database.";
                else
                {
                    caseInfo.mOffenceSet = int.Parse(cmd.Parameters[10].Value.ToString());

                    vehicleFromCursor(ds.Tables[0], ref caseInfo);
                    offenceFromCursor(ds.Tables[1], ref caseInfo);
                    if (!imagesFromCursor(ds.Tables[2], ds.Tables[3], ref caseInfo))
                        okay = false;

                    //imagesFromCursor(ds.Tables[2], ref caseInfo);

                    //// Images points to local, if they do not exit on local, poiint to remote
                    //if (!File.Exists(caseInfo.mImage1))
                    //    imagesFromCursor(ds.Tables[3], ref caseInfo);
                    //else if (!File.Exists(caseInfo.mImageNP)) // if numberplate image does not exist in local but image1 does then check if numberplate image does and if not point NP to central/remote
                    //{
                    //    sCaseInfo tmp;

                    //    caseInitialise(out tmp);

                    //    imagesFromCursor(ds.Tables[3], ref tmp);
                    //    caseInfo.mImageNP = tmp.mImageNP;
                    //}
                }

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                okay = false;
            }

            return okay;
        }

        public bool caseUnlock(string ticketNo)
        {
            mError = string.Empty;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.UNLOCK_ADJUDICATION_USER", mDBConnection);
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
                mError = ex.Message;
                return false;
            }

            return true;
        }

         public bool caseAdditionals(int offenceSet, out List<sOffenseCode> codes)
        {
            mError = string.Empty;
            codes = null;

            for(int i = 0; i < mCodesCache.Count; i++)
                if ((mCodesCache[i].mOffenceSet == offenceSet) && (mCodesCache[i].mCodes != null) && (mCodesCache[i].mCodes.Count > 0))
                {
                    codes = mCodesCache[i].mCodes;
                    return true;
                }

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.GET_ADDITIONAL_CODES", mDBConnection);
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
                    mCodesCache.Add(itm);
                }
                else
                    mError = "No Offence Codes exist for Offence Set " + offenceSet;

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
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

        private bool imagesFromCursor(DataTable local, DataTable remote, ref sCaseInfo caseInfo)
        {
            caseInfo.mImage1 = caseInfo.mImage2 = caseInfo.mImage3 = caseInfo.mImage4 = caseInfo.mImageNP = string.Empty;

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

            if (File.Exists(localImage1)) // If image exist locally, then all images is on local
            {
                caseInfo.mImage1 = localImage1;
                caseInfo.mImage2 = localImage2;
                caseInfo.mImage3 = localImage3;
                caseInfo.mImage4 = localImage4;
            }
            else // If image does not exist locally, then all images is on remote
            {
                caseInfo.mImage1 = remoteImage1;
                caseInfo.mImage2 = remoteImage2;
                caseInfo.mImage3 = remoteImage3;
                caseInfo.mImage4 = remoteImage4;
            }

#if (DEBUG)
            debugGetImageNames(ref caseInfo);
#endif

            caseInfo.mOnlyOneImage = testSinglePhoto(ref caseInfo);

            //if (string.IsNullOrEmpty(caseInfo.mImageNP))
            //{
                string localNP = constructNPName(localImage1);
                string remotNP = constructNPName(remoteImage1);

                if (File.Exists(localNP))
                    caseInfo.mImageNP = localNP;
                else
                {
                    if (File.Exists(remotNP))
                        caseInfo.mImageNP = remotNP;
                    else
                    {
                        mError = "Could not find numberplate image on local or remote paths. \n" + localNP + " \n" + remotNP;
                        return false;
                    }
                }
            //}

            return true;
        }

//        private bool imagesFromCursor(DataTable table, ref sCaseInfo caseInfo)
//        {
//            caseInfo.mImage1 = caseInfo.mImage2 = caseInfo.mImageNP = string.Empty;

//            if (table != null && table.Rows.Count > 0)
//            {
//                DataRow dr;

//                for (int i = 0; i < table.Rows.Count; i++)
//                {
//                    dr = table.Rows[i];
//                    switch (i)
//                    {
//                        case 0:
//                            caseInfo.mImage1 = dr[0].ToString();
//                            break;
//                        case 1:
//                            caseInfo.mImage2 = dr[0].ToString();
//                            break;
//                        case 2:
//                            caseInfo.mImage3 = dr[0].ToString();
//                            break;
//                        case 3:
//                            caseInfo.mImage4 = dr[0].ToString();
//                            break;
//                        case 4:
//                            caseInfo.mImageNP = dr[0].ToString();
//                            break;
//                    }
//                }

//#if (DEBUG)
//                debugGetImageNames(ref caseInfo);
//#endif

//                caseInfo.mOnlyOneImage = testSinglePhoto(ref caseInfo);

//                if (string.IsNullOrEmpty(caseInfo.mImageNP))
//                {
//                    string tmpNP = constructNPName(caseInfo.mImage1);

//                    if (File.Exists(tmpNP))
//                        caseInfo.mImageNP = tmpNP;
//                }

//                return true;
//            }

//            return false;
//        }

        private bool testSinglePhoto(ref sCaseInfo caseInfo)
        {
            if (caseInfo.mImage1.IndexOf(".sml") > 0)
                return true;
            else if (caseInfo.mImage2.IndexOf(".sml") > 0)
                return true;

            return false;
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
                var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.get_fishpond_lookup_data", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("p_user_id", mOperatorID);
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
                mError = ex.Message;
                okay = false;
            }

            //disconnect();

            return okay;
        }

        public bool fishpondGetCase(string ticketNo, ref sCaseInfo caseInfo)
        {
            bool okay = true;

            mError = string.Empty;

            //if (!connect())
            //    return false;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.lock_fishpond_user", mDBConnection);
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

                par = new OracleParameter("P_TICKET_NO", ticketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("O_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_VEHICLE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_OFFENCE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_OFFENCE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));

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

                vehicleFromCursor(ds.Tables[0], ref caseInfo);
                offenceFromCursor(ds.Tables[1], ref caseInfo);
                if (!imagesFromCursor(ds.Tables[2], ds.Tables[3], ref caseInfo))
                    okay = false;

                //imagesFromCursor(ds.Tables[2], ref caseInfo);

                //// Images points to local, if they do not exit on local, point to remote
                //if (!File.Exists(caseInfo.mImage1))
                //    imagesFromCursor(ds.Tables[3], ref caseInfo);
                //else if (!File.Exists(caseInfo.mImageNP)) // if numberplate image does not exist in local but image1 does then check if numberplate image does and if not point NP to central/remote
                //{
                //    sCaseInfo tmp;

                //    caseInitialise(out tmp);

                //    imagesFromCursor(ds.Tables[3], ref tmp);
                //    caseInfo.mImageNP = tmp.mImageNP;
                //}

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                okay = false;
            }

            //disconnect();

            return okay;
        }

        public bool fishpondCaseReject(sCaseInfo caseInfo, int reasonID)
        {
            //if (!connect())
            //    return false;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.reject_adjudication_fishpond", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_TICKET_NO", caseInfo.mTicketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", System.Environment.MachineName);
                par.OracleDbType = OracleDbType.Varchar2;
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_ID", mOperatorID);
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

                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                return false;
            }

            //disconnect();

            return true;
        }

        public bool fishpondCaseAccept(sCaseInfo caseInfo)
        {
            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.accept_adjudication_fishpond", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_TICKET_NO", caseInfo.mTicketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", System.Environment.MachineName);
                par.OracleDbType = OracleDbType.Varchar2;
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_ID", mOperatorID);
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
                mError = ex.Message;
                return false;
            }

            //disconnect();

            return true;
        }

        public bool fishpondUnlock(string ticketNo)
        {
            mError = string.Empty;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_ADJUDICATION_CENTRAL.unlock_fishpond_user", mDBConnection);
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
                mError = ex.Message;
                return false;
            }

            return true;
        }

    }
}
