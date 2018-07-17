using System;
using Oracle.DataAccess.Types;

namespace TMT.Build.OracleTableTypeClasses
{
    public class VehicleBooking : INullable, IOracleCustomType
    {
        private readonly bool mIsNull;

        public bool IsNull
        {
            get { return mIsNull; }
        }


        [OracleObjectMappingAttribute("VEHICLE_CATEGORY_ID")]
        public int VehicleCategoryID { get; set; }
        
        public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            VehicleCategoryID = (int)OracleUdt.GetValue(con, pUdt, "VEHICLE_CATEGORY_ID");
        }

        public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {  
            OracleUdt.SetValue(con, pUdt, "VEHICLE_CATEGORY_ID", VehicleCategoryID);
        }
    }

    


    [OracleCustomTypeMappingAttribute("TIS.VEHICLE_TYPE")]
    public class VehicleItemFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new VehicleBooking();
        }
    }





    [OracleCustomTypeMappingAttribute("TIS.TABLE_VEHICLE_TYPE")]
    public class VehicleBookingItemFactoryArray : IOracleArrayTypeFactory
    {
        public Array CreateArray(int numElems)
        {
            return new VehicleBooking[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }
    }
}
