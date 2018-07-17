using System;
using Oracle.DataAccess.Types;

namespace TMT.Build.OracleTableTypeClasses
{
    public class BookingSearchType : INullable, IOracleCustomType
    {
        private readonly bool mIsNull;

        public bool IsNull
        {
            get { return mIsNull; }
        }

        public BookingSearchType()
        {
        }


        [OracleObjectMappingAttribute("BOOKING_DATE")]
        public DateTime BookingDate { get; set; }

        [OracleObjectMappingAttribute("TEST_CATEGORY_ID")]
        public int TestCategoryID { get; set; }

        [OracleObjectMappingAttribute("IS_PASSED")]
        public int IsPassed { get; set; }

        [OracleObjectMappingAttribute("ENGINE_NUMBER")]
        public string EngineNumber { get; set; }

        [OracleObjectMappingAttribute("VEHICLE_IDENTIFICATION_NUMBER")]
        public string VehicleIdentificationNumber { get; set; }

        [OracleObjectMappingAttribute("VLN")]
        public string VLN { get; set; }

        [OracleObjectMappingAttribute("BOOKING_REFERENCE")]
        public string BookingReference { get; set; }

        [OracleObjectMappingAttribute("DATE_INDICATOR")]
        public int DateIndicator { get; set; }

        [OracleObjectMappingAttribute("QUANTITY")]
        public int Quantity { get; set; }

        [OracleObjectMappingAttribute("PAGE_NUMBER")]
        public int PageNumber { get; set; }


        public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            BookingDate = (DateTime)OracleUdt.GetValue(con, pUdt, "BOOKING_DATE");
            TestCategoryID = (int)OracleUdt.GetValue(con, pUdt, "TEST_CATEGORY_ID");
            IsPassed = (int)OracleUdt.GetValue(con, pUdt, "IS_PASSED");
            EngineNumber = (string)OracleUdt.GetValue(con, pUdt, "ENGINE_NUMBER");
            VehicleIdentificationNumber = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_IDENTIFICATION_NUMBER");
            VLN = (string)OracleUdt.GetValue(con, pUdt, "VLN");
            BookingReference = (string)OracleUdt.GetValue(con, pUdt, "BOOKING_REFERENCE");
            DateIndicator = (int)OracleUdt.GetValue(con, pUdt, "DATE_INDICATOR");
            Quantity = (int)OracleUdt.GetValue(con, pUdt, "QUANTITY");
            PageNumber = (int)OracleUdt.GetValue(con, pUdt, "PAGE_NUMBER");
        }


        public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, "BOOKING_DATE", BookingDate);
            OracleUdt.SetValue(con, pUdt, "TEST_CATEGORY_ID", TestCategoryID);
            OracleUdt.SetValue(con, pUdt, "IS_PASSED", IsPassed);
            OracleUdt.SetValue(con, pUdt, "ENGINE_NUMBER", EngineNumber);
            OracleUdt.SetValue(con, pUdt, "VEHICLE_IDENTIFICATION_NUMBER", VehicleIdentificationNumber);
            OracleUdt.SetValue(con, pUdt, "VLN", VLN);
            OracleUdt.SetValue(con, pUdt, "BOOKING_REFERENCE", BookingReference);
            OracleUdt.SetValue(con, pUdt, "DATE_INDICATOR", DateIndicator);
            OracleUdt.SetValue(con, pUdt, "QUANTITY", Quantity);
            OracleUdt.SetValue(con, pUdt, "PAGE_NUMBER", PageNumber);
        }
    }


    [OracleCustomTypeMappingAttribute("TIS.BOOKING_SEARCH_TYPE")]
    public class BookingSearchTypeItemFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new BookingSearchType();
        }
    }


    [OracleCustomTypeMappingAttribute("TIS.TABLE_BOOKING_SEARCH_TYPE")]
    public class BookingSearchTypeItemFactoryArray : IOracleArrayTypeFactory
    {
        public Array CreateArray(int numElems)
        {
            return new BookingSearchType[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }
    }
}
