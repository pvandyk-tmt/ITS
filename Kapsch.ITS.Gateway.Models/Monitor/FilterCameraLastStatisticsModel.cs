using Kapsch.Core.Gateway.Models.Enums;
using System.Collections.Generic;

namespace Kapsch.ITS.Gateway.Models.Monitor
{
    public class FilterCameraLastStatisticsModel
    {
        public long? RegionID { get; set; }
        public long? DistrictID { get; set; }
        public IList<CameraStatusType> CameraStatusTypes { get; set; }
    }
}
