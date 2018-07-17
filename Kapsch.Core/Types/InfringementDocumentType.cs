
namespace TMT.Build.OracleTableTypeClasses
{
	    
	//using Oracle.DataAccess.Client;
    using Oracle.DataAccess.Types;
    using System;

    public class InfringementDocumentType : Oracle.DataAccess.Types.INullable, Oracle.DataAccess.Types.IOracleCustomType
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
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("DOCUMENT_TYPE_ID")]
        public long DOCUMENT_TYPE_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("MIME_TYPE")]
        public string MIME_TYPE {get;set;}
        
		          public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, System.IntPtr pUdt)
        {
		            OracleUdt.SetValue(con, pUdt, "REFERENCE_NUMBER", REFERENCE_NUMBER);
		             OracleUdt.SetValue(con, pUdt, "DOCUMENT_TYPE_ID", DOCUMENT_TYPE_ID);
		             OracleUdt.SetValue(con, pUdt, "MIME_TYPE", MIME_TYPE);
		 		  }
     
	 public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, System.IntPtr pUdt)
        {
					REFERENCE_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "REFERENCE_NUMBER");
		 			DOCUMENT_TYPE_ID = (long)OracleUdt.GetValue(con, pUdt, "DOCUMENT_TYPE_ID");
		 			MIME_TYPE = (string)OracleUdt.GetValue(con, pUdt, "MIME_TYPE");
		 		  }
    }

    [Oracle.DataAccess.Types.OracleCustomTypeMappingAttribute("ITS.INFRINGEMENT_DOCUMENT_TYPE")]
    public class InfringementDocumentTypeFactory : Oracle.DataAccess.Types.IOracleCustomTypeFactory
    {

        public virtual Oracle.DataAccess.Types.IOracleCustomType CreateObject()
        {
            return new InfringementDocumentType();
        }
    }

    [Oracle.DataAccess.Types.OracleCustomTypeMappingAttribute("ITS.INFRINGEMENT_DOCUMENT_TABLE_TYPE")]
    public class InfringementDocumentTypeArrayFactory : Oracle.DataAccess.Types.IOracleArrayTypeFactory
    {

        public System.Array CreateArray(int numElems)
        {
            return new InfringementDocumentType[numElems];
        }

        public System.Array CreateStatusArray(int numElems)
        {
            return null;
        }
    }
}