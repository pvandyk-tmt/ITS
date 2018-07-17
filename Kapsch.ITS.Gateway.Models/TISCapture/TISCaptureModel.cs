using Kapsch.Core.Data;
using Kapsch.Core.Gateway.Models.Configuration;
using Kapsch.EVR.Gateway.Models.Vehicle;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.ITS.Gateway.Models.TISCapture
{
    public class TISCaptureModel
    {
        public IList<TISDataModel> TISData { get; set; }
        public IList<VehicleModelModel> VehicleModels { get; set; }
        public IList<VehicleMakeModel> VehicleMakes { get; set; }
        public IList<VehicleTypeModel> VehicleTypes { get; set; }
        public IList<VehicleColorModel> VehicleColours { get; set; }
        
    }
}

