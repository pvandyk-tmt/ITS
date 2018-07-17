using Kapsch.Core.Data;
using Kapsch.Core.Gateway.Models.Data;
using Kapsch.Gateway.Shared;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.Gateway.Shared.Filters;
using System.Configuration;

namespace Kapsch.Core.Gateway.Controllers
{
    [SessionAuthorize]
    [UsageLog]
    public class DataController : BaseController
    {
        public static string DataContextConnectionString = ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString;
     
        [HttpGet]        
        public IHttpActionResult Get(string query)
        {
            var words = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(f => f.ToUpper());
            if (words.Any(f => f == "DROP" || f == "DELETE" || f == "UPDATE" || f == "INSERT" || f == "EXEC"))
                return this.BadRequestEx(Error.DataQueryIllegalKeyword);

            using (var connection = new Oracle.DataAccess.Client.OracleConnection(DataContextConnectionString))
            {
                try
                {
                    return ExecuteQuery(query, connection);
                }
                catch (Exception ex)
                {
                    return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
                }
            }
        }

        private IHttpActionResult ExecuteQuery(string query, IDbConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            
            using (var command = new OracleCommand(query, (OracleConnection)connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    DataTable dataTable = new DataTable("resultSet");
                    dataTable.Load(reader);

                    return Ok(new DataModel { Data = dataTable });
                }
            }
        }
    }
}
