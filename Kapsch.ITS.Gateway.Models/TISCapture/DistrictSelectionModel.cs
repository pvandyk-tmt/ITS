using Kapsch.Core.Gateway.Models.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.ITS.Gateway.Models.TISCapture
{
    public class DistrictSelectionModel
    {
        public List<DistrictModel> Districts = new List<DistrictModel>();
        //public long NumberToExport { get; set; }
    }
}
