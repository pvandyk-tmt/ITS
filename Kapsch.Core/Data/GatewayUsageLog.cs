using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("GATEWAY_USAGE_LOG", Schema = "CREDENTIALS")]
    [Serializable]
    public class GatewayUsageLog
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("CREATED_TIMESTAMP")]
        public DateTime CreatedTimestamp { get; set; }

        [Column("SESSION_TOKEN")]
        public string SessionToken { get; set; }

        [Column("CLIENT_IP_ADDRESS")]
        public string ClientIPAddress { get; set; }

        [Column("METHOD_CALL")]
        public string Method { get; set; }

        [Column("CONTROLLER_NAME")]
        public string ControllerName { get; set; }

        [Column("ARGUMENTS")]
        public string Arguments { get; set; }

        [Column("RESPONSE_CODE")]
        public int ResponseCode { get; set; }

        [Column("EXCEPTION_MESSAGE")]
        public string Exception { get; set; }

        [Column("RESPONSE_TYPE")]
        public string ResponseType { get; set; }

        [Column("DURATION_IN_MILLISECONDS")]
        public decimal DurationInMilliSeconds { get; set; }
    }
}
