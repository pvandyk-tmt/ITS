using Kapsch.ITS.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Ticket
{
    public class EvidenceTypeModel
    {
        public string NoticeNumber { get; set; }
        public EvidenceType EvidenceType { get; set; }
        public string MimeType { get; set; }
        public byte[] MimeData { get; set; }
        public long DistrictID { get; set; }
    }
}
