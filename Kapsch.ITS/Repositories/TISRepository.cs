using Kapsch.Core.Data;
using Kapsch.ITS.Gateway.Models.TISCapture;
using Kapsch.ITS.Types;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;

namespace Kapsch.ITS.Repositories
{
    public class TISRepository
    {
        private readonly DataContext _dbContext;
        private long _credentialID = -1;
        private string _credentialUsername = string.Empty;
        private string _error = string.Empty;
        public static string DataContextConnectionString = ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString;

        public TISRepository(DataContext dbContext, long credentialID, string credentialUserName)
        {
            this._dbContext = dbContext;
            this._credentialID = credentialID;
            this._credentialUsername = credentialUserName;
        }

        public struct NatisExport
        {
            public string VehicleRegistration;
            public string InfringementDate;
            public string ReferenceNumber;
            public DateTime ExportDate;
            public long DistrictID;
            public long? LockedByCredentialID;
        }

        //public struct TISVehicleDetail
        //{
        //    public string ID;
        //    public string ReferenceNumber;
        //    public string VehicleRegistrationNumber;
        //    public string VehicleMake;
        //    public string VehicleModelID;
        //    public string VehicleTypeID;
        //    public string VehicleType;
        //    public string VehicleUsageID;
        //    public string VehicleColourID;
        //    public DateTime LicenseExpireDate;
        //    public string YearOfMake;
        //    public string ClearanceCertificateNumber;
        //    public string OwnerInitials;
        //    public string OwnerIDType;
        //    public string OwnerID;
        //    public string OwnerName;
        //    public string Surname;
        //    public string OwnerGender;
        //    public string Email;//Richard moet nog EMAIL bysit
        //    public string OwnerTelephone;
        //    public string OwnerPostal;
        //    public string OwnerPostalStreet;
        //    public string OwnerPostalSuburb;
        //    public string OwnerPostalTown;
        //    public long PostalCode;
        //    public string OwnerPhysical;
        //    public string OwnerPhysicalStreet;
        //    public string OwnerPhysicalSuburb;
        //    public string OwnerPhysicalTown;
        //    public long PhysicalCode;
        //    public long OwnerCellphone;
        //    public string VehicleModel;
        //    public string OwnerCompany;
        //    public DateTime DateOfOwner;
        //    public string ImportFileName;
        //    public string NatureOfOwnership;
        //    public string ProxyIndicator;
        //}

        public string Error
        {
            get { return _error; }
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

        public bool CaptureLock (string referenceNumber, string vehicleRegistrationNumber, out bool isSuccessful)
        {
            isSuccessful = false;

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.INFO_HANDLING.LOCK_CAPTURE_NATIS", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_USER_ID", _credentialID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                //par.Size = 4;
                cmd.Parameters.Add(par);

                par = new OracleParameter("P_REFERENCE_NUMBER", referenceNumber);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                //par.Size = 4;
                cmd.Parameters.Add(par);

                par = new OracleParameter("P_VEHICLE_REGISTRATION", vehicleRegistrationNumber);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Varchar2;
                //par.Size = 4;
                cmd.Parameters.Add(par);

                // Output
                par = new OracleParameter();
                par.ParameterName = "O_IS_SUCCESSFUL";
                par.Direction = ParameterDirection.Output;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                par.IsNullable = false;
               
                cmd.Parameters.Add(par);

                //cmd.Parameters.Add(new OracleParameter("O_RESULT", OracleDbType.RefCursor, 0,
                //                       ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current,
                //                       "O_RESULT"));

                cmd.ExecuteNonQuery();

                if (!(cmd.Parameters["O_IS_SUCCESSFUL"].Value is DBNull))
                {
                    isSuccessful = int.Parse(cmd.Parameters["O_IS_SUCCESSFUL"].Value.ToString()) > 0 ? true : false;
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

        public bool CaptureTIS (IList<TISDataModel> models)
        {
            try
            {
                using (var connection = new Oracle.DataAccess.Client.OracleConnection(DataContextConnectionString))

                using (var command = new Oracle.DataAccess.Client.OracleCommand())
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    var tisObjects = models.Select(
                            model => new TISType
                            {
                                //ID = model.ID,
                                REFERENCE_NUMBER = model.ReferenceNumber,
                                VEHICLE_REGISTRATION_NUMBER = model.VehicleRegistrationNumber,
                                VEHICLE_MAKE_ID = model.VehicleMakeID,
                                VEHICLE_MAKE = model.VehicleMake,
                                VEHICLE_MODEL_ID = model.VehicleModelID,
                                VEHICLE_MODEL = model.VehicleModel,
                                VEHICLE_TYPE_ID = model.VehicleTypeID,
                                VEHICLE_TYPE = model.VehicleType,
                                VEHICLE_USAGE_ID = model.VehicleUsageID,
                                VEHICLE_COLOUR_ID = model.VehicleColourID,
                                YEAR_OF_MAKE = model.YearOfMake,
                                LICENSE_EXPIRE_DATE = model.LicenseExpireDate,
                                CLEARENCE_CERT_NO = model.ClearanceCertificateNumber,
                                OWNER_ID = model.OwnerID,
                                OWNER_ID_TYPE = model.OwnerIDType,
                                OWNER_NAME = model.OwnerName,
                                OWNER_INIT = model.OwnerInitials,
                                OWNER_SURNAME = model.Surname,
                                OWNER_GENDER = model.OwnerGender,
                                OWNER_POSTAL = model.OwnerPostal,
                                OWNER_POSTAL_STREET = model.OwnerPostalStreet,
                                OWNER_POSTAL_SUBURB = model.OwnerPostalSuburb,
                                OWNER_POSTAL_TOWN = model.OwnerPostalTown,
                                OWNER_POSTAL_CODE = model.PostalCode,
                                OWNER_PHYS = model.OwnerPhysical,
                                OWNER_PHYS_STREET = model.OwnerPhysicalStreet,
                                OWNER_PHYS_SUBURB = model.OwnerPhysicalSuburb,
                                OWNER_PHYS_TOWN = model.OwnerPhysicalTown,
                                OWNER_PHYS_CODE = model.PhysicalCode,
                                OWNER_TELEPHONE = model.OwnerTelephone,
                                OWNER_CELLPHONE = model.OwnerCellphone,
                                OWNER_COMPANY = model.OwnerCompany,
                                DATE_OF_OWNERSHIP = model.DateOfOwnership,
                                IMPORT_FILE_NAME = model.ImportFileName,
                                NATURE_OF_OWNERSHIP = model.NatureOfOwnership,
                                PROXY_INDICATOR = model.ProxyIndicator,
                                EMAIL_ADDRESS = model.EmailAddress,
                            })
                            .ToList();

                    var par = new Oracle.DataAccess.Client.OracleParameter("P_CREDENTIAL_ID", _credentialID);
                    par.Direction = ParameterDirection.Input;
                    par.OracleDbType = Oracle.DataAccess.Client.OracleDbType.Int32;
                    command.Parameters.Add(par);

                    var param =
                        new Oracle.DataAccess.Client.OracleParameter("P_TABLE_NATIS_VEH_DETAIL_TYPE", Oracle.DataAccess.Client.OracleDbType.Array)
                        {
                            Value = tisObjects.ToArray(),
                            UdtTypeName = "ITS.TABLE_NATIS_VEH_DETAIL_TYPE"
                        };
                    command.Parameters.Add(param);

                    
                    //par = new Oracle.DataAccess.Client.OracleParameter();
                    //par.ParameterName = "O_MESSAGE";
                    //par.Direction = ParameterDirection.Output;
                    //par.OracleDbType = Oracle.DataAccess.Client.OracleDbType.Varchar2;
                    //par.Size = 4000;
                    //command.Parameters.Add(par);

                    command.BindByName = true;
                    try
                    {
                        ExcecuteNonQuery(command, "ITS.INFO_HANDLING.CAPTURE_NATIS", connection);

                        //if (!(command.Parameters["O_MESSAGE"].Value is DBNull))
                        //{
                        //    var isSuccessful = command.Parameters["O_MESSAGE"].Value.ToString();
                        //}
                    }
                    catch (Exception ex)
                    {
                        _error = ex.Message;
                        ErrorCheckFriendly();
                        return false;
                    }
                    finally
                    {
                        command.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                _error = e.Message;
                ErrorCheckFriendly();
                return false;
            }

            return true;
        }

        private void ExcecuteNonQuery(Oracle.DataAccess.Client.OracleCommand command, string storedProcName, Oracle.DataAccess.Client.OracleConnection dbConnection)
        {
            command.Connection = dbConnection;
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }

        public bool GetNatisExports(long numberToExport, long districtID, out List<NatisExport> natisExports)
        {
            natisExports = new List<NatisExport>();
            //natisExports.Clear();

            try
            {
                OracleParameter par;
                var cmd = new OracleCommand("ITS.INFO_HANDLING.EXPORT_NATIS", DbConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Inputs
                par = new OracleParameter("P_CREDENTIAL_ID", _credentialID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                cmd.Parameters.Add(par);

                par = new OracleParameter("P_QUANTITY", numberToExport);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                cmd.Parameters.Add(par);

                par = new OracleParameter("P_DISTRICT_ID", districtID);
                par.Direction = ParameterDirection.Input;
                par.OracleDbType = OracleDbType.Int32;
                par.Size = 4;
                cmd.Parameters.Add(par);

                // Outputs
                cmd.Parameters.Add(new OracleParameter("O_RESULT", OracleDbType.RefCursor, 0,
                                       ParameterDirection.Output, true, 0, 0, "", DataRowVersion.Current,
                                       "O_RESULT"));

                cmd.ExecuteNonQuery();

                if (!(cmd.Parameters["O_RESULT"].Value is DBNull))
                {
                    var curs = (Oracle.ManagedDataAccess.Types.OracleRefCursor)cmd.Parameters["O_RESULT"].Value;

                    OracleDataReader rd = curs.GetDataReader();
                    NatisExport natisExport;

                    while (rd.Read())
                    {
                        natisExport = new NatisExport();

                        natisExport.VehicleRegistration = rd.GetString(0);
                        natisExport.InfringementDate = rd.IsDBNull(1) ? string.Empty : rd.GetString(1);
                        natisExport.ReferenceNumber = rd.IsDBNull(2) ? string.Empty : rd.GetString(2);
                        natisExport.ExportDate = rd.IsDBNull(3) ? DateTime.MinValue : rd.GetDateTime(3);
                        natisExport.DistrictID = rd.IsDBNull(4) ? 0 : (long)rd.GetDecimal(4);
                        natisExport.LockedByCredentialID = rd.IsDBNull(5) ? 0 : (long)rd.GetDecimal(5);

                        natisExports.Add(natisExport);
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
    }
}
