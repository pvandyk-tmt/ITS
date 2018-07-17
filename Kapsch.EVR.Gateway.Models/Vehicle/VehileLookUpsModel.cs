using System.Collections.Generic;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class VehicleLookUpsModel
    {
        public List<VehicleMakeModel> vehicleMakes = new List<VehicleMakeModel>();
        public List<VehicleModelModel> vehicleModels = new List<VehicleModelModel>();
        public List<VehicleModelNumberModel> vehicleModelNumbers = new List<VehicleModelNumberModel>();
    }
}
