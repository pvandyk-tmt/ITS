using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Reflection;

namespace TMT.iVerify
{
    /// <summary>
    /// Data layer for iVerify
    /// </summary>
    public class cDataAccess
    {
        public struct sAddressInfo
        {
            public int mKey;
            public string mStreet;
            public string mSuburb;
            public string mTown;
            public string mCode;
            public DateTime mDate;
            public string mPOBox;
            public string mResidual;
            public int mResidualScore;
        }

        public struct sCaseInfo
        {
            public string mTicketNo;
            public string mVehicleRegNo;
            public bool mVehicleRegNoConfirmed;
            public string mVehicleMake;
            public string mVehicleModel;
            public string mVehicleColour;
            public string mVehicleType;
            public string mVehicleCaptureType;
            public DateTime mOffenceDate;
            public string mOffenceOldNotes;
            public string mOffenceNewNotes;
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

            public int mPersonKey;
            public string mPersonName;
            public string mPersonSurname;
            public string mPersonMiddleNames;
            public string mPersonTelephone;
            public string mPersonID;
            public int mPersonPhysicalAddressKey;
            public int mPersonPostalAddressKey;

            public bool mUseGismoAddress;
            public sAddressInfo mNatisPhysical;
            public sAddressInfo mNatisPostal;
            public sAddressInfo mSystemPhysical;
            public sAddressInfo mSystemPostal;
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
            public string mDescription;
            public decimal mAmount;

            public override string ToString()
            {
                return mDescription;
            }
        }

        public class cPostalCode
        {
            public string pCity { get; set; }
            public string pSuburb { get; set; }
            public string pCode { get; set; }
        }

        private OracleConnection mDBConnection = null;

        private string mConnectionString = string.Empty;
        private string mError = string.Empty;
        private int mOperatorID = -1;
        private string mOperatorName = string.Empty;
        private bool mNPCapturedOnCentral = false;     // Number-plate image captured at central
        private string mNPCapturePath = string.Empty;  // Number-plate image path to central
        private bool addressIndicator = false;

        /// <summary>
        /// Check if the number-plate capturing is done on central.
        /// </summary>
        public bool pNPCapturedOnCentral
        {
            get { return mNPCapturedOnCentral; }
        }

        /// <summary>
        /// Check if address recently verified
        /// </summary>
        public bool pAddressIndicator
        {
            get { return addressIndicator; }
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
            mOperatorName = username;

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

        public bool mayVerify(string username)
        {
            mError = string.Empty;

            /*** Any logged-in user may Verify ***/
            //try
            //{
            //    OracleCommand cmd = new OracleCommand("SELECT ACTIVITY_ID FROM GISMOEMPLOYEE_ACTIVITY WHERE EMPLOYEE_ID=" + mOperatorID + " AND ACTIVITY_ID=101", mDBConnection); //TODO ACTIVITY_ID=???

            //    object val = cmd.ExecuteScalar();

            //    cmd.Dispose();

            //    if ((val == null) || (val is System.DBNull))
            //    {
            //        mError = "Username '" + username + "' may not Verify tickets.";
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

        public void addressInitialise(out sAddressInfo addr)
        {
            addr.mResidualScore = 
            addr.mKey = -1;
            addr.mStreet =
            addr.mSuburb =
            addr.mTown =
            addr.mPOBox =
            addr.mResidual =
            addr.mCode = string.Empty;
            addr.mDate = DateTime.MinValue;
        }

        public void caseInitialise(out sCaseInfo caseInfo)
        {
            caseInfo.mTicketNo = string.Empty;
            caseInfo.mImage1 = string.Empty;
            caseInfo.mImage2 = string.Empty;
            caseInfo.mImage3 = string.Empty;
            caseInfo.mImage4 = string.Empty;
            caseInfo.mImageNP = string.Empty;
            caseInfo.mImage1ID =
            caseInfo.mImage2ID =
            caseInfo.mImage3ID =
            caseInfo.mImage4ID = -1;
            caseInfo.mPrintImageNo = 0;
            caseInfo.mOnlyOneImage = false;
            caseInfo.mVehicleRegNo = string.Empty;
            caseInfo.mVehicleRegNoConfirmed = false;
            caseInfo.mVehicleMake = string.Empty;
            caseInfo.mVehicleModel = string.Empty;
            caseInfo.mVehicleColour = string.Empty;
            caseInfo.mVehicleType = string.Empty;
            caseInfo.mVehicleCaptureType = string.Empty;
            caseInfo.mOffenceDate = DateTime.MinValue;
            caseInfo.mOffenceOldNotes = string.Empty;
            caseInfo.mOffenceNewNotes = string.Empty;

            caseInfo.mPersonName =
            caseInfo.mPersonSurname =
            caseInfo.mPersonMiddleNames =
            caseInfo.mPersonTelephone =
            caseInfo.mPersonID = string.Empty;
            caseInfo.mPersonKey = 
            caseInfo.mPersonPhysicalAddressKey =
            caseInfo.mPersonPostalAddressKey = -1;

            caseInfo.mUseGismoAddress = false;
            addressInitialise(out caseInfo.mNatisPhysical);
            addressInitialise(out caseInfo.mNatisPostal);
            addressInitialise(out caseInfo.mSystemPhysical);
            addressInitialise(out caseInfo.mSystemPostal);
        }

        public bool getPostalCodes(string city, string suburb, string code, bool isPhysical, List<cPostalCode> codes)
        {
            bool okay = true;

            mError = string.Empty;
            codes.Clear();

            if (!connect())
                return false;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("LOOKUPS.get_postal_codes", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("p_postal", (isPhysical ? 0 : 1));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_town", city);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_suburb", suburb);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_code", code);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("o_result", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "o_result"));

                var ds = new DataSet();
                var da = new OracleDataAdapter(cmd);
                int val = da.Fill(ds);

                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        cPostalCode cod = new cPostalCode();

                        cod.pCity = dr[0].ToString();
                        cod.pSuburb = dr[1].ToString();
                        cod.pCode = dr[2].ToString();

                        codes.Add(cod);
                    }
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

            disconnect();

            return okay;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reasons"></param>
        /// <param name="types"></param>
        /// <param name="count"></param>
        /// <param name="caseInfo"></param>
        /// <returns></returns>
        public bool caseFirst(ref List<sRejectReasons> reasons, ref List<sCaptureTypes> types, out int count, ref sCaseInfo caseInfo)
        {
            mError = string.Empty;
            count = 0;
            caseInitialise(out caseInfo);

            reasons.Clear();
            types.Clear();

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_VERIFICATION_DATES.get_veri_multi_image_lookup", mDBConnection);
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
                cmd.Parameters.Add(new OracleParameter("O_PERSON_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_PERSON_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_SYSTEM_PHYSICAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_SYSTEM_PHYSICAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_SYSTEM_POSTAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_SYSTEM_POSTAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_NATIS_PHYSICAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NATIS_PHYSICAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_NATIS_POSTAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NATIS_POSTAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_ALT_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_ALT_VEHICLE_CURSOR"));
                                
                par = new OracleParameter();
                par.ParameterName = "O_ADDRESS_INDICATOR";
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

                if (!(cmd.Parameters["o_count"].Value is DBNull))
                {
                    var test = cmd.Parameters["o_count"].Value.ToString();
                }

                count = int.Parse(cmd.Parameters[2].Value.ToString());
                mNPCapturedOnCentral = (int.Parse(cmd.Parameters["O_CENTRAL_INDICATOR"].Value.ToString()) == 1 ? true : false);
                
                addressIndicator = (int.Parse(cmd.Parameters["O_ADDRESS_INDICATOR"].Value.ToString()) == 1 ? true : false);

                if (count > 0)
                {
                    reasonsFromCursor(ds.Tables[0], ref reasons);
                    vehicleFromCursor(ds.Tables[1], ref caseInfo);
                    personFromCursor(ds.Tables[2], ref caseInfo);

                    addressFromCursor(ds.Tables[3], ref caseInfo.mSystemPhysical);
                    addressFromCursor(ds.Tables[4], ref caseInfo.mSystemPostal);
                 
                    addressFromCursor(ds.Tables[5], ref caseInfo.mNatisPhysical);
                    addressFromCursor(ds.Tables[6], ref caseInfo.mNatisPostal);

                    imagesFromCursor(ds.Tables[7], ref caseInfo);
                    if (!File.Exists(caseInfo.mImage1)) // Images does not exist locally, get remote paths
                        imagesFromCursor(ds.Tables[8], ref caseInfo);
                    else if (mNPCapturedOnCentral) // if capture is done at central then retrieve number-plate image from remote (central) location
                    {
                        sCaseInfo tmp;

                        caseInitialise(out tmp);
                        imagesFromCursor(ds.Tables[8], ref tmp);
                        mNPCapturePath = Path.GetDirectoryName(tmp.mImage1);
                        caseInfo.mImageNP = constructNPName(tmp.mImage1);
                    }
                    typesFromCursor(ds.Tables[9], ref types);
                }

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                return false;
            }

            return true;
        }

        public bool caseAccept(ref sCaseInfo caseInfo, bool addrChanged, bool personChanged, int typeID, decimal typeAmount, ref List<sCaptureTypes> types, out int count, int printImageID)
        {
            mError = string.Empty;
            count = 0;

            types.Clear();

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_VERIFICATION_DATES.accept_veri_multi_image", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_TICKET_NO", caseInfo.mTicketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", System.Environment.MachineName);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_ID", mOperatorID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_VEHICLE_CHANGE_XML", (typeID >= 0 && typeAmount > 0 ? typeXML(typeID, typeAmount) : string.Empty));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_PERSON_XML", (personChanged ? personXML(caseInfo) : string.Empty));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                //if (caseInfo.mUseGismoAddress)
                //{
                //    caseInfo.mSystemPhysical.mKey = caseInfo.mNatisPhysical.mKey;
                //    caseInfo.mSystemPostal.mKey = caseInfo.mNatisPostal.mKey;
                    par = new OracleParameter("P_ADDRESS_XML", (addrChanged ? addressXML(caseInfo.mSystemPhysical, caseInfo.mSystemPostal) : string.Empty));
                //}
                //else
                //{
                //    par = new OracleParameter("P_ADDRESS_XML", (addrChanged ? addressXML(caseInfo.mNatisPhysical, caseInfo.mNatisPostal) : string.Empty));
               // }
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_NOTES", caseInfo.mOffenceOldNotes + " " + caseInfo.mOffenceNewNotes);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("P_PRINT_IMAGE_ID", printImageID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);

                // Outputs
                par = new OracleParameter();
                par.ParameterName = "O_COUNT";
                par.Direction = ParameterDirection.Output;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("O_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_VEHICLE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_PERSON_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_PERSON_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_SYSTEM_PHYSICAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_SYSTEM_PHYSICAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_SYSTEM_POSTAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_SYSTEM_POSTAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_NATIS_PHYSICAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NATIS_PHYSICAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_NATIS_POSTAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NATIS_POSTAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_ALT_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_ALT_VEHICLE_CURSOR"));

                par = new OracleParameter();
                par.ParameterName = "O_ADDRESS_INDICATOR";
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

                caseInitialise(out caseInfo);

                count = int.Parse(cmd.Parameters[8].Value.ToString());
                mNPCapturedOnCentral = (int.Parse(cmd.Parameters["O_CENTRAL_INDICATOR"].Value.ToString()) == 1 ? true : false);

                addressIndicator = (int.Parse(cmd.Parameters["O_ADDRESS_INDICATOR"].Value.ToString()) == 1 ? true : false);

                if (count > 0)
                {
                    vehicleFromCursor(ds.Tables[0], ref caseInfo);
                    personFromCursor(ds.Tables[1], ref caseInfo);

                    addressFromCursor(ds.Tables[2], ref caseInfo.mSystemPhysical);
                    addressFromCursor(ds.Tables[3], ref caseInfo.mSystemPostal);
                    
                    addressFromCursor(ds.Tables[4], ref caseInfo.mNatisPhysical);
                    addressFromCursor(ds.Tables[5], ref caseInfo.mNatisPostal);
                                        
                    imagesFromCursor(ds.Tables[6], ref caseInfo);
                    if (!File.Exists(caseInfo.mImage1))
                        imagesFromCursor(ds.Tables[7], ref caseInfo);
                    else if (mNPCapturedOnCentral) // if capture is done at central then retrieve number-plate image from remote (central) location
                    {
                        sCaseInfo tmp;

                        caseInitialise(out tmp);
                        imagesFromCursor(ds.Tables[7], ref tmp);
                        mNPCapturePath = Path.GetDirectoryName(tmp.mImage1);
                        caseInfo.mImageNP = constructNPName(tmp.mImage1);
                    }
                    typesFromCursor(ds.Tables[8], ref types);
                }

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                return false;
            }

            return true;
        }

        public bool caseReject(ref sCaseInfo caseInfo, int reasonID, bool regnoChanged, bool addrChanged, bool personChanged, int typeID, decimal typeAmount, ref List<sCaptureTypes> types, out int count)
        {
            mError = string.Empty;
            count = 0;

            types.Clear();

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_VERIFICATION_DATES.reject_veri_multi_image", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_TICKET_NO", caseInfo.mTicketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_COMPUTER_NAME", System.Environment.MachineName);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_USER_ID", mOperatorID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_REJECTION_ID", reasonID); 
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_VEHICLE_CHANGE_XML", (typeID >= 0 && typeAmount > 0 ? typeXML(typeID, typeAmount) : string.Empty));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_PERSON_XML", (personChanged ? personXML(caseInfo) : string.Empty));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
 //               if (caseInfo.mUseGismoAddress)
 //               {
  //                  caseInfo.mSystemPhysical.mKey = caseInfo.mNatisPhysical.mKey;
   //                 caseInfo.mSystemPostal.mKey = caseInfo.mNatisPostal.mKey;
                    par = new OracleParameter("P_ADDRESS_XML", (addrChanged ? addressXML(caseInfo.mSystemPhysical, caseInfo.mSystemPostal) : string.Empty));
       //         }
        //        else
         //       {
           //         par = new OracleParameter("P_ADDRESS_XML", (addrChanged ? addressXML(caseInfo.mNatisPhysical, caseInfo.mNatisPostal) : string.Empty));
             //   }
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_VEHICLE_REG", (regnoChanged ? caseInfo.mVehicleRegNo : string.Empty));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_NOTES", caseInfo.mOffenceOldNotes + " " + caseInfo.mOffenceNewNotes);
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
                cmd.Parameters.Add(new OracleParameter("O_PERSON_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_PERSON_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_SYSTEM_PHYSICAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_SYSTEM_PHYSICAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_SYSTEM_POSTAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_SYSTEM_POSTAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_NATIS_PHYSICAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NATIS_PHYSICAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_NATIS_POSTAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NATIS_POSTAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_ALT_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_ALT_VEHICLE_CURSOR"));

                par = new OracleParameter();
                par.ParameterName = "O_ADDRESS_INDICATOR";
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

                caseInitialise(out caseInfo);

                count = int.Parse(cmd.Parameters[9].Value.ToString());
                mNPCapturedOnCentral = (int.Parse(cmd.Parameters["O_CENTRAL_INDICATOR"].Value.ToString()) == 1 ? true : false);

                addressIndicator = (int.Parse(cmd.Parameters["O_ADDRESS_INDICATOR"].Value.ToString()) == 1 ? true : false);

                if (count > 0)
                {
                    vehicleFromCursor(ds.Tables[0], ref caseInfo);
                    personFromCursor(ds.Tables[1], ref caseInfo);

                    addressFromCursor(ds.Tables[2], ref caseInfo.mSystemPhysical);
                    addressFromCursor(ds.Tables[3], ref caseInfo.mSystemPostal);
                    
                    addressFromCursor(ds.Tables[4], ref caseInfo.mNatisPhysical);
                    addressFromCursor(ds.Tables[5], ref caseInfo.mNatisPostal);
                    
                    imagesFromCursor(ds.Tables[6], ref caseInfo);
                    if (!File.Exists(caseInfo.mImage1))
                        imagesFromCursor(ds.Tables[7], ref caseInfo);
                    else if (mNPCapturedOnCentral) // if capture is done at central then retrieve number-plate image from remote (central) location
                    {
                        sCaseInfo tmp;

                        caseInitialise(out tmp);
                        imagesFromCursor(ds.Tables[7], ref tmp);
                        mNPCapturePath = Path.GetDirectoryName(tmp.mImage1);
                        caseInfo.mImageNP = constructNPName(tmp.mImage1);
                    }
                    typesFromCursor(ds.Tables[8], ref types);
                }

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                return false;
            }

            return true;
        }

        public bool checkAddress(ref sCaseInfo caseInfo, bool forSummons)
        {
            mError = string.Empty;

            try
            {
                OracleParameter par;
                OracleCommand cmd;

                if (forSummons)
                    cmd = new OracleCommand("summons_verification.check_address", mDBConnection);
                else
                    cmd = new OracleCommand("OFFENCE_VERIFICATION_DATES.check_address", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("p_po_id", caseInfo.mSystemPostal.mKey);
                par.Direction = ParameterDirection.Input;
                par.Size = 4;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("p_po_town", string.IsNullOrEmpty(caseInfo.mSystemPostal.mTown) ? null : caseInfo.mSystemPostal.mTown);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("p_po_suburb", string.IsNullOrEmpty(caseInfo.mSystemPostal.mSuburb) ? null : caseInfo.mSystemPostal.mSuburb);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("p_po_street", string.IsNullOrEmpty(caseInfo.mSystemPostal.mStreet) ? null : caseInfo.mSystemPostal.mStreet);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("p_po_po_box", string.IsNullOrEmpty(caseInfo.mSystemPostal.mPOBox) ? null : caseInfo.mSystemPostal.mPOBox);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("p_po_code", string.IsNullOrEmpty(caseInfo.mSystemPostal.mCode) ? null : caseInfo.mSystemPostal.mCode);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_ph_id", caseInfo.mSystemPhysical.mKey);
                par.Direction = ParameterDirection.Input;
                par.Size = 4;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);
                par = new OracleParameter("p_ph_town", string.IsNullOrEmpty(caseInfo.mSystemPhysical.mTown) ? null : caseInfo.mSystemPhysical.mTown);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("p_ph_suburb", string.IsNullOrEmpty(caseInfo.mSystemPhysical.mSuburb) ? null : caseInfo.mSystemPhysical.mSuburb);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("p_ph_street", string.IsNullOrEmpty(caseInfo.mSystemPhysical.mStreet) ? null : caseInfo.mSystemPhysical.mStreet);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("p_ph_po_box", string.IsNullOrEmpty(caseInfo.mSystemPhysical.mPOBox) ? null : caseInfo.mSystemPhysical.mPOBox);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("p_ph_code", string.IsNullOrEmpty(caseInfo.mSystemPhysical.mCode) ? null : caseInfo.mSystemPhysical.mCode);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("o_address_po", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "o_address_po"));
                cmd.Parameters.Add(new OracleParameter("o_address_ph", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "o_address_ph"));

                var ds = new DataSet();
                var da = new OracleDataAdapter(cmd);
                int val = da.Fill(ds);

                //caseInitialise(out caseInfo);

                addressNoDateFromCursor(ds.Tables[0], ref caseInfo.mSystemPostal);
                addressNoDateFromCursor(ds.Tables[1], ref caseInfo.mSystemPhysical);

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
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

        private string typeXML(int id, decimal amount)
        {
            string xml = "<main> ";

            xml += "<DATA_RECORD> ";
            xml += "<ID>" + id.ToString() + "</ID> ";
            xml += "<AMOUNT>" + amount.ToString("0.00") + "</AMOUNT> ";
            xml += "</DATA_RECORD> ";
            xml += "</main>";

            return xml;
        }

        private string addressXML(sAddressInfo physical, sAddressInfo postal)
        {
            string xml = "<main> ";

            xml += "<DATA_RECORD> ";
            xml += "<ID>" + physical.mKey.ToString() + "</ID> ";
            xml += "<TOWN>" + xmlClean(physical.mTown) + "</TOWN> ";
            xml += "<SUBURB>" + xmlClean(physical.mSuburb) + "</SUBURB> ";
            xml += "<STREET>" + xmlClean(physical.mStreet) + "</STREET> ";
            xml += "<PO_BOX>" + xmlClean(physical.mPOBox) + "</PO_BOX> ";
            xml += "<CODE>" + xmlClean(physical.mCode) + "</CODE> ";
            xml += "<SCORE>" + physical.mResidualScore.ToString() + "</SCORE> ";
            xml += "</DATA_RECORD> ";

            xml += "<DATA_RECORD> ";
            xml += "<ID>" + postal.mKey.ToString() + "</ID> ";
            xml += "<TOWN>" + xmlClean(postal.mTown) + "</TOWN> ";
            xml += "<SUBURB>" + xmlClean(postal.mSuburb) + "</SUBURB> ";
            xml += "<STREET>" + xmlClean(postal.mStreet) + "</STREET> ";
            xml += "<PO_BOX>" + xmlClean(postal.mPOBox) + "</PO_BOX> ";
            xml += "<CODE>" + xmlClean(postal.mCode) + "</CODE> ";
            xml += "<SCORE>" + postal.mResidualScore.ToString() + "</SCORE> ";
            xml += "</DATA_RECORD> ";

            xml += "</main>";

            return xml;
        }

        private string personXML(sCaseInfo caseInfo)
        {
            string xml = "<main> ";

            xml += "<DATA_RECORD> ";
            xml += "<ID>" + caseInfo.mPersonKey.ToString() + "</ID> ";
            xml += "<NAME>" + xmlClean(caseInfo.mPersonName) + "</NAME> ";
            xml += "<SURNAME>" + xmlClean(caseInfo.mPersonSurname) + "</SURNAME> ";
            xml += "<MIDDLE_NAMES>" + xmlClean(caseInfo.mPersonMiddleNames) + "</MIDDLE_NAMES> ";
            xml += "<ID_NO>" + xmlClean(caseInfo.mPersonID) + "</ID_NO> ";
            xml += "<TELEPHONE>" + xmlClean(caseInfo.mPersonTelephone) + "</TELEPHONE> ";
            xml += "<PHYSICAL_ADDRESS_ID>" + caseInfo.mPersonPhysicalAddressKey.ToString() + "</PHYSICAL_ADDRESS_ID> ";
            xml += "<POSTAL_ADDRESS_ID>" + caseInfo.mPersonPostalAddressKey.ToString() + "</POSTAL_ADDRESS_ID> ";
            xml += "</DATA_RECORD> ";

            xml += "</main>";

            return xml;
        }

        public bool caseUnlock(string ticketNo)
        {
            mError = string.Empty;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_VERIFICATION_DATES.UNLOCK_VERIFICATION_USER", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                par = new OracleParameter("P_TICKET_NO", ticketNo);
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);

                cmd.ExecuteScalar();

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                return false;
            }

            return true;
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
                caseInfo.mVehicleCaptureType = dr["CAPTURE_TYPE"].ToString();
                caseInfo.mOffenceOldNotes = dr["NOTES"].ToString();
                caseInfo.mOffenceNewNotes = string.Empty;
                caseInfo.mVehicleRegNoConfirmed = (int.Parse(dr["REGISTRATION_CHECK"].ToString()) == 0 ? true : false);

                return true;
            }

            return false;
        }

        private bool personFromCursor(DataTable table, ref sCaseInfo caseInfo)
        {
            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];

                caseInfo.mPersonKey = int.Parse(dr["ID"].ToString());
                caseInfo.mPersonName = dr["NAME"].ToString();
                caseInfo.mPersonSurname = dr["SURNAME"].ToString();
                caseInfo.mPersonMiddleNames = dr["MIDDLE_NAMES"].ToString();
                caseInfo.mPersonID = dr["ID_NUMBER"].ToString();
                caseInfo.mPersonTelephone = dr["TELEPHONE"].ToString();
                caseInfo.mPersonPostalAddressKey = int.Parse(dr["POSTAL_ADDRESS_ID"].ToString());
                caseInfo.mPersonPhysicalAddressKey = int.Parse(dr["PHYSICAL_ADDRESS_ID"].ToString());

                return true;
            }

            return false;
        }

        private bool addressNoDateFromCursor(DataTable table, ref sAddressInfo addressInfo)
        {
            return addressFromCursor(table, ref addressInfo, false);
        }

        private bool addressFromCursor(DataTable table, ref sAddressInfo addressInfo)
        {
            return addressFromCursor(table, ref addressInfo, true);
        }

        private bool addressFromCursor(DataTable table, ref sAddressInfo addressInfo, bool doDate)
        {
            addressInitialise(out addressInfo);
            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];

                addressInfo.mKey = int.Parse(dr["ID"].ToString());
                addressInfo.mTown = dr["TOWN"].ToString();
                addressInfo.mSuburb = dr["SUBURB"].ToString();
                addressInfo.mStreet = dr["STREET"].ToString();
                addressInfo.mPOBox = dr["PO_BOX"].ToString();
                addressInfo.mCode = dr["CODE"].ToString();
                if (doDate)
                    DateTime.TryParse(dr["address_date"].ToString(), out addressInfo.mDate);

                if (table.Columns.Count > 7)
                {
                    addressInfo.mResidual = dr["RESIDUAL"].ToString();
                    addressInfo.mResidualScore = int.Parse(dr["SCORE"].ToString());
                }

                return true;
            }

            return false;
        }

#if (DEBUG)
        private int imgCnt = 0;
        private void debugGetImageNames(ref sCaseInfo caseInfo)
        {
            imgCnt++;

            string cntStr = imgCnt.ToString("00");

            caseInfo.mImageNP = string.Empty;
            caseInfo.mImage1 = @"C:\Dev\TMT\trunk\iVerify\Doc\Img0" + cntStr + "_0.jpg";
            caseInfo.mImage2 = @"C:\Dev\TMT\trunk\iVerify\Doc\Img0" + cntStr + "_1.jpg";
            if (imgCnt == 1)
            {
                caseInfo.mImage3 = @"C:\Dev\TMT\trunk\iVerify\Doc\Img0" + cntStr + "_2.jpg";
                caseInfo.mImage4 = @"C:\Dev\TMT\trunk\iVerify\Doc\Img0" + cntStr + "_3.jpg";
            }
            else
                caseInfo.mImage3 = caseInfo.mImage4 = string.Empty;
        }
#endif

        private bool imagesFromCursor(DataTable table, ref sCaseInfo caseInfo)
        {
            caseInfo.mImage1 = caseInfo.mImage2 = caseInfo.mImage3 = caseInfo.mImage4 = caseInfo.mImageNP = string.Empty;

            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr;
                int printImageNo = 0;

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    dr = table.Rows[i];
                    switch (i)
                    {
                        case 0:
                            caseInfo.mImage1 = dr[0].ToString();
                            caseInfo.mImage1ID = int.Parse(dr[1].ToString());
                            if (dr[2].ToString() == "1")
                                printImageNo = 0;
                            break;
                        case 1:
                            caseInfo.mImage2 = dr[0].ToString();
                            caseInfo.mImage2ID = int.Parse(dr[1].ToString());
                            if (dr[2].ToString() == "1")
                                printImageNo = 1;
                            break;
                        case 2:
                            caseInfo.mImage3 = dr[0].ToString();
                            caseInfo.mImage3ID = int.Parse(dr[1].ToString());
                            if (dr[2].ToString() == "1")
                                printImageNo = 2;
                            break;
                        case 3:
                            caseInfo.mImage4 = dr[0].ToString();
                            caseInfo.mImage4ID = int.Parse(dr[1].ToString());
                            if (dr[2].ToString() == "1")
                                printImageNo = 3;
                            break;
                        case 4:
                            caseInfo.mImageNP = dr[0].ToString();
                            break;
                    }
                }
                caseInfo.mPrintImageNo = printImageNo;

#if (DEBUG)
                debugGetImageNames(ref caseInfo);
#endif

                caseInfo.mOnlyOneImage = testSinglePhoto(ref caseInfo);
                if (caseInfo.mOnlyOneImage)
                    caseInfo.mPrintImageNo = printImageNo = 0;

                if (string.IsNullOrEmpty(caseInfo.mImageNP))
                {
                    string tmpNP = constructNPName(caseInfo.mImage1);

                    if (File.Exists(tmpNP))
                        caseInfo.mImageNP = tmpNP;
                }

                return true;
            }

            return false;
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

        private bool typesFromCursor(DataTable table, ref List<sCaptureTypes> types)
        {
            if (table != null && table.Rows.Count > 0)
            {
                DataRow dr;
                sCaptureTypes typ;

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    dr = table.Rows[i];
                    typ.mID = int.Parse(dr["ID"].ToString());
                    typ.mDescription = dr["VEHICLE_TYPE"].ToString();
                    typ.mAmount = decimal.Parse(dr["AMOUNT"].ToString());
                    types.Add(typ);
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
                var cmd = new OracleCommand("OFFENCE_VERIFICATION_DATES.get_fishpond_lookup_data", mDBConnection);
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

        public bool fishpondGetCase(string ticketNo, ref sCaseInfo caseInfo, ref List<sCaptureTypes> types)
        {
            mError = string.Empty;

            if (types == null)
                types = new List<sCaptureTypes>();
            types.Clear();

            //if (!connect())
            //    return false;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_VERIFICATION_DATES.lock_fishpond_user", mDBConnection);
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
                cmd.Parameters.Add(new OracleParameter("O_PERSON_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_PERSON_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_SYSTEM_PHYSICAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_SYSTEM_PHYSICAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_SYSTEM_POSTAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_SYSTEM_POSTAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_NATIS_PHYSICAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NATIS_PHYSICAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_NATIS_POSTAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NATIS_POSTAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_LOCAL_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_LOCAL_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_REMOTE_IMAGE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_REMOTE_IMAGE_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_ALT_VEHICLE_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_ALT_VEHICLE_CURSOR"));

                par = new OracleParameter();
                par.ParameterName = "O_ADDRESS_INDICATOR";
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

                caseInitialise(out caseInfo);

                caseInfo.mTicketNo = ticketNo;

                mNPCapturedOnCentral = (int.Parse(cmd.Parameters["O_CENTRAL_INDICATOR"].Value.ToString()) == 1 ? true : false);

                addressIndicator = (int.Parse(cmd.Parameters["O_ADDRESS_INDICATOR"].Value.ToString()) == 1 ? true : false);

                vehicleFromCursor(ds.Tables[0], ref caseInfo);
                personFromCursor(ds.Tables[1], ref caseInfo);

                addressFromCursor(ds.Tables[2], ref caseInfo.mSystemPhysical);
                addressFromCursor(ds.Tables[3], ref caseInfo.mSystemPostal);

                addressFromCursor(ds.Tables[4], ref caseInfo.mNatisPhysical);
                addressFromCursor(ds.Tables[5], ref caseInfo.mNatisPostal);
                
                imagesFromCursor(ds.Tables[6], ref caseInfo);
                if (!File.Exists(caseInfo.mImage1))
                    imagesFromCursor(ds.Tables[7], ref caseInfo);
                else if (mNPCapturedOnCentral) // if capture is done at central then retrieve number-plate image from remote (central) location
                {
                    sCaseInfo tmp;

                    caseInitialise(out tmp);
                    imagesFromCursor(ds.Tables[7], ref tmp);
                    mNPCapturePath = Path.GetDirectoryName(tmp.mImage1);
                    caseInfo.mImageNP = constructNPName(tmp.mImage1);
                }
                typesFromCursor(ds.Tables[8], ref types);


                ds.Dispose();
                da.Dispose();
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

        public bool fishpondCaseReject(sCaseInfo caseInfo, int reasonID, bool regnoChanged, bool addrChanged, bool personChanged, int typeID, decimal typeAmount)
        {
            //if (!connect())
            //    return false;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_VERIFICATION_DATES.reject_verification_fishpond", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("p_ticket_no", caseInfo.mTicketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_computer_name", System.Environment.MachineName);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_user_id", mOperatorID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_rejection_id", reasonID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);

                par = new OracleParameter("P_VEHICLE_CHANGE_XML", (typeID >= 0 && typeAmount > 0 ? typeXML(typeID, typeAmount) : string.Empty));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_PERSON_XML", (personChanged ? personXML(caseInfo) : string.Empty));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                //if (caseInfo.mUseGismoAddress)
                //{
                //    caseInfo.mSystemPhysical.mKey = caseInfo.mNatisPhysical.mKey;
                //    caseInfo.mSystemPostal.mKey = caseInfo.mNatisPostal.mKey;
                    par = new OracleParameter("P_ADDRESS_XML", (addrChanged ? addressXML(caseInfo.mSystemPhysical, caseInfo.mSystemPostal) : string.Empty));
                //}
                //else
                //{/
                //    par = new OracleParameter("P_ADDRESS_XML", (addrChanged ? addressXML(caseInfo.mNatisPhysical, caseInfo.mNatisPostal) : string.Empty));
                //}
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("P_VEHICLE_REG", (regnoChanged ? caseInfo.mVehicleRegNo : string.Empty));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("P_NOTES", caseInfo.mOffenceOldNotes + " " + caseInfo.mOffenceNewNotes);
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

        public bool fishpondCaseAccept(sCaseInfo caseInfo, bool addrChanged, bool personChanged, int typeID, decimal typeAmount, int printImageID)
        {
            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("OFFENCE_VERIFICATION_DATES.accept_verification_fishpond", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("p_ticket_no", caseInfo.mTicketNo);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_computer_name", System.Environment.MachineName);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_user_id", mOperatorID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                cmd.Parameters.Add(par);

                par = new OracleParameter("P_VEHICLE_CHANGE_XML", (typeID >= 0 && typeAmount > 0 ? typeXML(typeID, typeAmount) : string.Empty));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                par = new OracleParameter("P_PERSON_XML", (personChanged ? personXML(caseInfo) : string.Empty));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);
                //if (caseInfo.mUseGismoAddress)
                //{
                    //caseInfo.mSystemPhysical.mKey = caseInfo.mNatisPhysical.mKey;
                    //caseInfo.mSystemPostal.mKey = caseInfo.mNatisPostal.mKey;
                    par = new OracleParameter("P_ADDRESS_XML", (addrChanged ? addressXML(caseInfo.mSystemPhysical, caseInfo.mSystemPostal) : string.Empty));
                //}
                //else
                //{
                //    par = new OracleParameter("P_ADDRESS_XML", (addrChanged ? addressXML(caseInfo.mNatisPhysical, caseInfo.mNatisPostal) : string.Empty));
               // }
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("P_NOTES", caseInfo.mOffenceOldNotes + " " + caseInfo.mOffenceNewNotes);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("p_print_image_id", printImageID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
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
                var cmd = new OracleCommand("OFFENCE_VERIFICATION_DATES.unlock_fishpond_user", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                par = new OracleParameter("P_TICKET_NO", ticketNo);
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);

                cmd.ExecuteScalar();

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                return false;
            }

            return true;
        }

        public bool summonsLock(ref sCaseInfo caseInfo)
        {
            mError = string.Empty;
            caseInitialise(out caseInfo);


            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("summons_verification.lock_summons_feeder", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_USER_NAME", mOperatorName);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                // Outputs
                //par = new OracleParameter();
                //par.ParameterName = "O_ID_NUMBER";
                //par.Direction = ParameterDirection.Output;
                //par.OracleDbType = OracleDbType.Varchar2;
                //cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("O_PERSON_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_PERSON_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_POSTAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_POSTAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_PHYSICAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_PHYSICAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_NATIS_PHYSICAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NATIS_PHYSICAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_NATIS_POSTAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NATIS_POSTAL_CURSOR"));

                var ds = new DataSet();
                var da = new OracleDataAdapter(cmd);
                int val = da.Fill(ds);

                //string idNum = cmd.Parameters[1].Value.ToString();

                if (true)//!string.IsNullOrEmpty(idNum))
                {
                    personFromCursor(ds.Tables[0], ref caseInfo);

                    addressFromCursor(ds.Tables[1], ref caseInfo.mSystemPostal);
                    addressFromCursor(ds.Tables[2], ref caseInfo.mSystemPhysical);

                    addressFromCursor(ds.Tables[3], ref caseInfo.mNatisPhysical);
                    addressFromCursor(ds.Tables[4], ref caseInfo.mNatisPostal);
                }

                ds.Dispose();
                da.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                return false;
            }

            return true;
        }

        public bool summonsAccept(ref sCaseInfo caseInfo, bool addrChanged, bool personChanged)
        {
            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("summons_verification.save_ticket_data", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_ID_NUMBER", caseInfo.mPersonID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                //par = new OracleParameter("p_computer_name", System.Environment.MachineName);
                //par.Direction = ParameterDirection.Input;
                //par.OracleDbType = OracleDbType.Varchar2;
                //cmd.Parameters.Add(par);

                par = new OracleParameter("P_USER_NAME", mOperatorName);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("P_PERSON_XML", (personChanged ? personXML(caseInfo) : string.Empty));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                par = new OracleParameter("P_ADDRESS_XML", (addrChanged ? addressXML(caseInfo.mNatisPhysical, caseInfo.mNatisPostal) : string.Empty));
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                cmd.Parameters.Add(par);

                cmd.Parameters.Add(new OracleParameter("O_PERSON_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_PERSON_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_POSTAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_POSTAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_PHYSICAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_PHYSICAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_NATIS_PHYSICAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NATIS_PHYSICAL_CURSOR"));
                cmd.Parameters.Add(new OracleParameter("O_NATIS_POSTAL_CURSOR", OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current, "O_NATIS_POSTAL_CURSOR"));

                //cmd.ExecuteNonQuery();
                var ds = new DataSet();
                var da = new OracleDataAdapter(cmd);
                int val = da.Fill(ds);

                personFromCursor(ds.Tables[0], ref caseInfo);

                addressFromCursor(ds.Tables[1], ref caseInfo.mSystemPostal);
                addressFromCursor(ds.Tables[2], ref caseInfo.mSystemPhysical);
                addressFromCursor(ds.Tables[3], ref caseInfo.mNatisPhysical);
                addressFromCursor(ds.Tables[4], ref caseInfo.mNatisPostal);

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

        public bool summonsUnlock()
        {
            mError = string.Empty;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("summons_verification.unlock_summons_feeder", mDBConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                par = new OracleParameter("P_USER_NAME", mOperatorName);
                par.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(par);

                cmd.ExecuteScalar();

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
