﻿using System;


namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class VehicleSearchModel
    {
        public DateTime BookingDate { get; set; }

        public int TestCategoryID { get; set; }

        public int IsPassed { get; set; }

        public string EngineNumber { get; set; }

        public string VehicleIdentificationNumber { get; set; }

        public string VLN { get; set; }
        
        public int DateIndicator { get; set; }

        public int Quantity { get; set; }

        public int PageNumber { get; set; }
    }
}
