
namespace TMT.Build.OracleTableTypeClasses
{
	    
	//using Oracle.DataAccess.Client;
    using Oracle.DataAccess.Types;
    using System;

    public class InfringementEvidenceType : Oracle.DataAccess.Types.INullable, Oracle.DataAccess.Types.IOracleCustomType
    {   
  
        private readonly bool isNull = false;

		bool INullable.IsNull
        {
            get
            {
                return this.isNull;
            }
        }

				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("REFERENCE_NUMBER")]
        public string REFERENCE_NUMBER {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("EVIDENCE_TYPE")]
        public int EVIDENCE_TYPE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("MIME_TYPE")]
        public string MIME_TYPE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("DISTRICT_ID")]
        public long DISTRICT_ID {get;set;}
        
		          public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, System.IntPtr pUdt)
        {
		            OracleUdt.SetValue(con, pUdt, "REFERENCE_NUMBER", REFERENCE_NUMBER);
		             OracleUdt.SetValue(con, pUdt, "EVIDENCE_TYPE", EVIDENCE_TYPE);
		             OracleUdt.SetValue(con, pUdt, "MIME_TYPE", MIME_TYPE);
		             OracleUdt.SetValue(con, pUdt, "DISTRICT_ID", DISTRICT_ID);
		 		  }
     
	 public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, System.IntPtr pUdt)
        {
					REFERENCE_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "REFERENCE_NUMBER");
		 			EVIDENCE_TYPE = (int)OracleUdt.GetValue(con, pUdt, "EVIDENCE_TYPE");
		 			MIME_TYPE = (string)OracleUdt.GetValue(con, pUdt, "MIME_TYPE");
		 			DISTRICT_ID = (long)OracleUdt.GetValue(con, pUdt, "DISTRICT_ID");
		 		  }
    }

    [Oracle.DataAccess.Types.OracleCustomTypeMappingAttribute("ITS.INFRINGEMENT_EVIDENCE_TYPE")]
    public class InfringementEvidenceTypeFactory : Oracle.DataAccess.Types.IOracleCustomTypeFactory
    {

        public virtual Oracle.DataAccess.Types.IOracleCustomType CreateObject()
        {
            return new InfringementEvidenceType();
        }
    }

    [Oracle.DataAccess.Types.OracleCustomTypeMappingAttribute("ITS.INFRINGEMENT_EVIDENCE_TABLE_TYPE")]
    public class InfringementEvidenceTypeArrayFactory : Oracle.DataAccess.Types.IOracleArrayTypeFactory
    {

        public System.Array CreateArray(int numElems)
        {
            return new InfringementEvidenceType[numElems];
        }

        public System.Array CreateStatusArray(int numElems)
        {
            return null;
        }
    }
}