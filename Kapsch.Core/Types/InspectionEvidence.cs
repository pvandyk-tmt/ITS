
namespace TMT.Build.OracleTableTypeClasses
{
	    
	//using Oracle.DataAccess.Client;
    using Oracle.DataAccess.Types;
    using System;

    public class InspectionEvidence : Oracle.DataAccess.Types.INullable, Oracle.DataAccess.Types.IOracleCustomType
    {   
  
        private readonly bool isNull = false;

		bool INullable.IsNull
        {
            get
            {
                return this.isNull;
            }
        }

				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_TEST_BOOKING_ID")]
        public long VEHICLE_TEST_BOOKING_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("INSPECTION_EVIDENCE_TYPE_ID")]
        public long INSPECTION_EVIDENCE_TYPE_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("MIME_TYPE")]
        public string MIME_TYPE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("SITE_ID")]
        public long SITE_ID {get;set;}
        
		          public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, System.IntPtr pUdt)
        {
		            OracleUdt.SetValue(con, pUdt, "VEHICLE_TEST_BOOKING_ID", VEHICLE_TEST_BOOKING_ID);
		             OracleUdt.SetValue(con, pUdt, "INSPECTION_EVIDENCE_TYPE_ID", INSPECTION_EVIDENCE_TYPE_ID);
		             OracleUdt.SetValue(con, pUdt, "MIME_TYPE", MIME_TYPE);
		             OracleUdt.SetValue(con, pUdt, "SITE_ID", SITE_ID);
		 		  }
     
	 public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, System.IntPtr pUdt)
        {
					VEHICLE_TEST_BOOKING_ID = (long)OracleUdt.GetValue(con, pUdt, "VEHICLE_TEST_BOOKING_ID");
		 			INSPECTION_EVIDENCE_TYPE_ID = (long)OracleUdt.GetValue(con, pUdt, "INSPECTION_EVIDENCE_TYPE_ID");
		 			MIME_TYPE = (string)OracleUdt.GetValue(con, pUdt, "MIME_TYPE");
		 			SITE_ID = (long)OracleUdt.GetValue(con, pUdt, "SITE_ID");
		 		  }
    }

    [Oracle.DataAccess.Types.OracleCustomTypeMappingAttribute("TIS.INSPECTION_EVIDENCE_TYPE")]
    public class InspectionEvidenceFactory : Oracle.DataAccess.Types.IOracleCustomTypeFactory
    {

        public virtual Oracle.DataAccess.Types.IOracleCustomType CreateObject()
        {
            return new InspectionEvidence();
        }
    }

    [Oracle.DataAccess.Types.OracleCustomTypeMappingAttribute("TIS.INSPECTION_EVIDENCE_TABLE_TYPE")]
    public class InspectionEvidenceArrayFactory : Oracle.DataAccess.Types.IOracleArrayTypeFactory
    {

        public System.Array CreateArray(int numElems)
        {
            return new InspectionEvidence[numElems];
        }

        public System.Array CreateStatusArray(int numElems)
        {
            return null;
        }
    }
}