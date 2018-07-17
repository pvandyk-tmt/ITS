using Kapsch.ITS.Gateway.Models.Enums;
using System;


namespace Kapsch.ITS.Gateway.Models.TISCapture
{
    public class NatisExportModel
    {
        //public long ID { get; set; }
        public string VehicleRegistration { get; set; }
        public string InfringementDate { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime? ExportDate { get; set; }
        public long DistrictID { get; set; }
        public long? LockedByCredentialID { get; set; }

        public string DistrictName { get; set; }
        public string LockedByName { get; set; }
        public string FormattedExportDate
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ExportDate.ToString()))
                {
                    DateTime formattedExportDate = (DateTime)ExportDate;
                    return formattedExportDate.ToString("yyyy/MM/dd, HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                    return ExportDate.ToString();
            }
        }

    }
}
