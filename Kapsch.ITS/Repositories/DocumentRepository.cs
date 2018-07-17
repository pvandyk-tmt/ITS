using Kapsch.Core.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using TMT.Build.OracleTableTypeClasses;

namespace Kapsch.ITS.Repositories
{
    public class DocumentRepository
    {
        public static string DataContextConnectionString = ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString;

        private readonly DataContext _dbContext;

        public DocumentRepository(DataContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public string GetDocumentPath(string referenceNumber, int documentType, string mimeType)
        {
            using (var connection = new Oracle.DataAccess.Client.OracleConnection(DataContextConnectionString))
            {
                var infringementDocumentType =
                    new InfringementDocumentType
                    {
                        REFERENCE_NUMBER = referenceNumber,
                        DOCUMENT_TYPE_ID = documentType,
                        MIME_TYPE = mimeType
                    };

                using (var command = new Oracle.DataAccess.Client.OracleCommand())
                {
                    try
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        command.Parameters.Add(
                            new Oracle.DataAccess.Client.OracleParameter("P_Infringement_Document", Oracle.DataAccess.Client.OracleDbType.Object)
                            {
                                Value = infringementDocumentType,
                                UdtTypeName = "ITS.INFRINGEMENT_DOCUMENT_TYPE"
                            });
                        command.Parameters.Add("O_RESULT", Oracle.DataAccess.Client.OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        ExcecuteNonQuery(command, "ITS.OFFENCE_DOCUMENTS.Submit_Infringement_Document", connection);

                        if ((command.Parameters["O_RESULT"].Value is DBNull))
                            throw new Exception("Failed to return valid file paths.");

                        var refCursor = (Oracle.DataAccess.Types.OracleRefCursor)command.Parameters["O_RESULT"].Value;

                        using (var dataReader = refCursor.GetDataReader())
                        {
                            while (dataReader.Read())
                            {
                                if (dataReader["MIME_DATA_PATH"] is DBNull || dataReader["FILENAME"] is DBNull)
                                    continue;

                                var filePath = dataReader["MIME_DATA_PATH"] as string;
                                var fileName = dataReader["FILENAME"] as string;

                                return Path.Combine(filePath, fileName);
                            }
                        }
                    }
                    finally
                    {
                        foreach (Oracle.DataAccess.Client.OracleParameter parameter in command.Parameters)
                        {
                            if (parameter.Value is IDisposable)
                            {
                                ((IDisposable)(parameter.Value)).Dispose();
                            }

                            parameter.Dispose();
                        }
                    }

                    return string.Empty;
                }
            }
        }

        private void ExcecuteNonQuery(OracleCommand command, string storedProcName, OracleConnection dbConnection)
        {
            command.Connection = dbConnection;
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }

        private void ExcecuteNonQuery(Oracle.DataAccess.Client.OracleCommand command, string storedProcName, Oracle.DataAccess.Client.OracleConnection dbConnection)
        {
            command.Connection = dbConnection;
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }
    }
}
