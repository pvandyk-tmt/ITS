using System;
using Oracle.DataAccess.Types;

namespace TMT.Build.OracleTableTypeClasses
{
    public class VehicleTestBooking : INullable, IOracleCustomType
    {
        private readonly bool mIsNull;

        public bool IsNull
        {
            get { return mIsNull; }
        }

        public VehicleTestBooking()
        {
        }


        [OracleObjectMappingAttribute("VEHICLE_IDENTIFICATION_NUMBER")]
        public string VIN { get; set; }

        [OracleObjectMappingAttribute("ENGINE_NUMBER")]
        public string EngineNumber { get; set; }

        [OracleObjectMappingAttribute("VEHICLE_CATEGORY_ID")]
        public int VehicleCategoryID { get; set; }

        [OracleObjectMappingAttribute("VEHICLE_TYPE_ID")]
        public int VehicleTypeID { get; set; }

        [OracleObjectMappingAttribute("VEHICLE_MAKE_ID")]
        public int VehicleMakeID { get; set; }

        [OracleObjectMappingAttribute("VEHICLE_MODEL_ID")]
        public int VehicleModelID { get; set; }

        [OracleObjectMappingAttribute("VEHICLE_MODEL_NUMBER_ID")]
        public int VehicleModelNumberID { get; set; }

        [OracleObjectMappingAttribute("YEAR_OF_MAKE")]
        public int YearOfMake { get; set; }

        [OracleObjectMappingAttribute("COLOUR_ID")]
        public int ColourID { get; set; }

        [OracleObjectMappingAttribute("VLN")]
        public string VLN { get; set; }

        [OracleObjectMappingAttribute("NET_WEIGHT")]
        public int NetWeight { get; set; }

        [OracleObjectMappingAttribute("GVM")]
        public int GVM { get; set; }

        [OracleObjectMappingAttribute("PROPELLED_BY_ID")]
        public int PropelledByID { get; set; }

        [OracleObjectMappingAttribute("FUEL_TYPE_ID")]
        public int FuelTypeID { get; set; }

        [OracleObjectMappingAttribute("REGISTRATION_STATUS_ID")]
        public int RegistrationStatusID { get; set; }

        [OracleObjectMappingAttribute("LICENCE_EXPIRY_DATE")]
        public DateTime? LicenceExpiryDate { get; set; }

        [OracleObjectMappingAttribute("ROADWORTHINESS_EXPIRY_DATE")]
        public DateTime? RoadworthyExpiryDate { get; set; }

        [OracleObjectMappingAttribute("INSURANCE_EXPIRY_DATE")]
        public DateTime? InsuranceExpiryDate { get; set; }

        [OracleObjectMappingAttribute("SEATING_CAPACITY")]
        public long SeatingCapacity { get; set; }

        public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            VIN = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_IDENTIFICATION_NUMBER");
            EngineNumber = (string)OracleUdt.GetValue(con, pUdt, "ENGINE_NUMBER");
            VehicleCategoryID = (int)OracleUdt.GetValue(con, pUdt, "VEHICLE_CATEGORY_ID");
            VehicleTypeID = (int)OracleUdt.GetValue(con, pUdt, "VEHICLE_TYPE_ID");
            VehicleMakeID = (int)OracleUdt.GetValue(con, pUdt, "VEHICLE_MAKE_ID");
            VehicleModelID = (int)OracleUdt.GetValue(con, pUdt, "VEHICLE_MODEL_ID");
            VehicleModelNumberID = (int)OracleUdt.GetValue(con, pUdt, "VEHICLE_MODEL_NUMBER_ID");
            YearOfMake = (int)OracleUdt.GetValue(con, pUdt, "YEAR_OF_MAKE");
            ColourID = (int)OracleUdt.GetValue(con, pUdt, "COLOUR_ID");
            VLN = (string)OracleUdt.GetValue(con, pUdt, "VLN");
            NetWeight = (int)OracleUdt.GetValue(con, pUdt, "NET_WEIGHT");
            GVM = (int)OracleUdt.GetValue(con, pUdt, "GVM");
            PropelledByID = (int)OracleUdt.GetValue(con, pUdt, "PROPELLED_BY_ID");
            FuelTypeID = (int)OracleUdt.GetValue(con, pUdt, "FUEL_TYPE_ID");
            RegistrationStatusID = (int)OracleUdt.GetValue(con, pUdt, "REGISTRATION_STATUS_ID");
            LicenceExpiryDate = (DateTime)OracleUdt.GetValue(con, pUdt, "LICENCE_EXPIRY_DATE");
            RoadworthyExpiryDate = (DateTime)OracleUdt.GetValue(con, pUdt, "ROADWORTHINESS_EXPIRY_DATE");
            InsuranceExpiryDate = (DateTime)OracleUdt.GetValue(con, pUdt, "INSURANCE_EXPIRY_DATE");
            SeatingCapacity = (long)OracleUdt.GetValue(con, pUdt, "SEATING_CAPACITY");
        }


        public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, "VEHICLE_IDENTIFICATION_NUMBER", VIN);
            OracleUdt.SetValue(con, pUdt, "ENGINE_NUMBER", EngineNumber);
            OracleUdt.SetValue(con, pUdt, "VEHICLE_CATEGORY_ID", VehicleCategoryID);
            OracleUdt.SetValue(con, pUdt, "VEHICLE_TYPE_ID", VehicleTypeID);
            OracleUdt.SetValue(con, pUdt, "VEHICLE_MAKE_ID", VehicleMakeID);
            OracleUdt.SetValue(con, pUdt, "VEHICLE_MODEL_ID", VehicleModelID);
            OracleUdt.SetValue(con, pUdt, "VEHICLE_MODEL_NUMBER_ID", VehicleModelNumberID);
            OracleUdt.SetValue(con, pUdt, "YEAR_OF_MAKE", YearOfMake);
            OracleUdt.SetValue(con, pUdt, "COLOUR_ID", ColourID);
            OracleUdt.SetValue(con, pUdt, "VLN", VLN);
            OracleUdt.SetValue(con, pUdt, "NET_WEIGHT", NetWeight);
            OracleUdt.SetValue(con, pUdt, "GVM", GVM);
            OracleUdt.SetValue(con, pUdt, "PROPELLED_BY_ID", PropelledByID);
            OracleUdt.SetValue(con, pUdt, "FUEL_TYPE_ID", FuelTypeID);
            OracleUdt.SetValue(con, pUdt, "REGISTRATION_STATUS_ID", RegistrationStatusID);
            OracleUdt.SetValue(con, pUdt, "LICENCE_EXPIRY_DATE", LicenceExpiryDate);
            OracleUdt.SetValue(con, pUdt, "ROADWORTHINESS_EXPIRY_DATE", RoadworthyExpiryDate);
            OracleUdt.SetValue(con, pUdt, "INSURANCE_EXPIRY_DATE", InsuranceExpiryDate);
            OracleUdt.SetValue(con, pUdt, "SEATING_CAPACITY", SeatingCapacity);           
        }
    }


    [OracleCustomTypeMappingAttribute("TIS.VEHICLE_DETAIL_TYPE")]
    public class VehicleTestBookingItemFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new VehicleTestBooking();
        }
    }


    [OracleCustomTypeMappingAttribute("TIS.TABLE_VEHICLE_DETAIL_TYPE")]
    public class VehicleTestBookingItemFactoryArray : IOracleArrayTypeFactory
    {
        public Array CreateArray(int numElems)
        {
            return new TMT.Build.OracleTableTypeClasses.VehicleTestBooking[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }
    }
}
