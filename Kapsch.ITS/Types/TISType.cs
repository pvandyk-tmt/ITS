
namespace Kapsch.ITS.Types
{
		
	//using Oracle.DataAccess.Client;
	using Oracle.DataAccess.Types;
	using System;

	public class TISType : Oracle.DataAccess.Types.INullable, Oracle.DataAccess.Types.IOracleCustomType
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
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_REGISTRATION_NUMBER")]
		public string VEHICLE_REGISTRATION_NUMBER {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_MAKE_ID")]
		public string VEHICLE_MAKE_ID {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_MAKE")]
		public string VEHICLE_MAKE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_MODEL_ID")]
		public string VEHICLE_MODEL_ID {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_MODEL")]
		public string VEHICLE_MODEL {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_TYPE_ID")]
		public string VEHICLE_TYPE_ID {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_TYPE")]
		public string VEHICLE_TYPE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_USAGE_ID")]
		public string VEHICLE_USAGE_ID {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_COLOUR_ID")]
		public string VEHICLE_COLOUR_ID {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("YEAR_OF_MAKE")]
		public string YEAR_OF_MAKE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("LICENSE_EXPIRE_DATE")]
		public DateTime? LICENSE_EXPIRE_DATE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("CLEARENCE_CERT_NO")]
		public string CLEARENCE_CERT_NO {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_ID")]
		public string OWNER_ID {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_ID_TYPE")]
		public string OWNER_ID_TYPE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_NAME")]
		public string OWNER_NAME {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_INIT")]
		public string OWNER_INIT {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_SURNAME")]
		public string OWNER_SURNAME {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_GENDER")]
		public string OWNER_GENDER {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_POSTAL")]
		public string OWNER_POSTAL {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_POSTAL_STREET")]
		public string OWNER_POSTAL_STREET {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_POSTAL_SUBURB")]
		public string OWNER_POSTAL_SUBURB {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_POSTAL_TOWN")]
		public string OWNER_POSTAL_TOWN {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_POSTAL_CODE")]
		public string OWNER_POSTAL_CODE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_PHYS")]
		public string OWNER_PHYS {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_PHYS_STREET")]
		public string OWNER_PHYS_STREET {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_PHYS_SUBURB")]
		public string OWNER_PHYS_SUBURB {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_PHYS_TOWN")]
		public string OWNER_PHYS_TOWN {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_PHYS_CODE")]
		public string OWNER_PHYS_CODE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_TELEPHONE")]
		public string OWNER_TELEPHONE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_CELLPHONE")]
		public string OWNER_CELLPHONE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("OWNER_COMPANY")]
		public string OWNER_COMPANY {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("DATE_OF_OWNERSHIP")]
		public DateTime? DATE_OF_OWNERSHIP {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("IMPORT_FILE_NAME")]
		public string IMPORT_FILE_NAME {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("NATURE_OF_OWNERSHIP")]
		public string NATURE_OF_OWNERSHIP {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("PROXY_INDICATOR")]
		public string PROXY_INDICATOR {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("EMAIL_ADDRESS")]
		public string EMAIL_ADDRESS {get;set;}
		
				  public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, System.IntPtr pUdt)
		{
					OracleUdt.SetValue(con, pUdt, "REFERENCE_NUMBER", REFERENCE_NUMBER);
		 			OracleUdt.SetValue(con, pUdt, "VEHICLE_REGISTRATION_NUMBER", VEHICLE_REGISTRATION_NUMBER);
		 			OracleUdt.SetValue(con, pUdt, "VEHICLE_MAKE_ID", VEHICLE_MAKE_ID);
		 			OracleUdt.SetValue(con, pUdt, "VEHICLE_MAKE", VEHICLE_MAKE);
		 			OracleUdt.SetValue(con, pUdt, "VEHICLE_MODEL_ID", VEHICLE_MODEL_ID);
		 			OracleUdt.SetValue(con, pUdt, "VEHICLE_MODEL", VEHICLE_MODEL);
		 			OracleUdt.SetValue(con, pUdt, "VEHICLE_TYPE_ID", VEHICLE_TYPE_ID);
		 			OracleUdt.SetValue(con, pUdt, "VEHICLE_TYPE", VEHICLE_TYPE);
		 			OracleUdt.SetValue(con, pUdt, "VEHICLE_USAGE_ID", VEHICLE_USAGE_ID);
		 			OracleUdt.SetValue(con, pUdt, "VEHICLE_COLOUR_ID", VEHICLE_COLOUR_ID);
		 			OracleUdt.SetValue(con, pUdt, "YEAR_OF_MAKE", YEAR_OF_MAKE);
		 			OracleUdt.SetValue(con, pUdt, "LICENSE_EXPIRE_DATE", LICENSE_EXPIRE_DATE);
		 			OracleUdt.SetValue(con, pUdt, "CLEARENCE_CERT_NO", CLEARENCE_CERT_NO);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_ID", OWNER_ID);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_ID_TYPE", OWNER_ID_TYPE);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_NAME", OWNER_NAME);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_INIT", OWNER_INIT);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_SURNAME", OWNER_SURNAME);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_GENDER", OWNER_GENDER);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_POSTAL", OWNER_POSTAL);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_POSTAL_STREET", OWNER_POSTAL_STREET);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_POSTAL_SUBURB", OWNER_POSTAL_SUBURB);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_POSTAL_TOWN", OWNER_POSTAL_TOWN);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_POSTAL_CODE", OWNER_POSTAL_CODE);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_PHYS", OWNER_PHYS);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_PHYS_STREET", OWNER_PHYS_STREET);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_PHYS_SUBURB", OWNER_PHYS_SUBURB);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_PHYS_TOWN", OWNER_PHYS_TOWN);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_PHYS_CODE", OWNER_PHYS_CODE);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_TELEPHONE", OWNER_TELEPHONE);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_CELLPHONE", OWNER_CELLPHONE);
		 			OracleUdt.SetValue(con, pUdt, "OWNER_COMPANY", OWNER_COMPANY);
		 			OracleUdt.SetValue(con, pUdt, "DATE_OF_OWNERSHIP", DATE_OF_OWNERSHIP);
		 			OracleUdt.SetValue(con, pUdt, "IMPORT_FILE_NAME", IMPORT_FILE_NAME);
		 			OracleUdt.SetValue(con, pUdt, "NATURE_OF_OWNERSHIP", NATURE_OF_OWNERSHIP);
		 			OracleUdt.SetValue(con, pUdt, "PROXY_INDICATOR", PROXY_INDICATOR);
		 			OracleUdt.SetValue(con, pUdt, "EMAIL_ADDRESS", EMAIL_ADDRESS);
		 		  }
	 
	 public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, System.IntPtr pUdt)
		{
					REFERENCE_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "REFERENCE_NUMBER");
		 			VEHICLE_REGISTRATION_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_REGISTRATION_NUMBER");
		 			VEHICLE_MAKE_ID = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_MAKE_ID");
		 			VEHICLE_MAKE = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_MAKE");
		 			VEHICLE_MODEL_ID = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_MODEL_ID");
		 			VEHICLE_MODEL = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_MODEL");
		 			VEHICLE_TYPE_ID = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_TYPE_ID");
		 			VEHICLE_TYPE = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_TYPE");
		 			VEHICLE_USAGE_ID = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_USAGE_ID");
		 			VEHICLE_COLOUR_ID = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_COLOUR_ID");
		 			YEAR_OF_MAKE = (string)OracleUdt.GetValue(con, pUdt, "YEAR_OF_MAKE");
		 			LICENSE_EXPIRE_DATE = (DateTime?)OracleUdt.GetValue(con, pUdt, "LICENSE_EXPIRE_DATE");
		 			CLEARENCE_CERT_NO = (string)OracleUdt.GetValue(con, pUdt, "CLEARENCE_CERT_NO");
		 			OWNER_ID = (string)OracleUdt.GetValue(con, pUdt, "OWNER_ID");
		 			OWNER_ID_TYPE = (string)OracleUdt.GetValue(con, pUdt, "OWNER_ID_TYPE");
		 			OWNER_NAME = (string)OracleUdt.GetValue(con, pUdt, "OWNER_NAME");
		 			OWNER_INIT = (string)OracleUdt.GetValue(con, pUdt, "OWNER_INIT");
		 			OWNER_SURNAME = (string)OracleUdt.GetValue(con, pUdt, "OWNER_SURNAME");
		 			OWNER_GENDER = (string)OracleUdt.GetValue(con, pUdt, "OWNER_GENDER");
		 			OWNER_POSTAL = (string)OracleUdt.GetValue(con, pUdt, "OWNER_POSTAL");
		 			OWNER_POSTAL_STREET = (string)OracleUdt.GetValue(con, pUdt, "OWNER_POSTAL_STREET");
		 			OWNER_POSTAL_SUBURB = (string)OracleUdt.GetValue(con, pUdt, "OWNER_POSTAL_SUBURB");
		 			OWNER_POSTAL_TOWN = (string)OracleUdt.GetValue(con, pUdt, "OWNER_POSTAL_TOWN");
		 			OWNER_POSTAL_CODE = (string)OracleUdt.GetValue(con, pUdt, "OWNER_POSTAL_CODE");
		 			OWNER_PHYS = (string)OracleUdt.GetValue(con, pUdt, "OWNER_PHYS");
		 			OWNER_PHYS_STREET = (string)OracleUdt.GetValue(con, pUdt, "OWNER_PHYS_STREET");
		 			OWNER_PHYS_SUBURB = (string)OracleUdt.GetValue(con, pUdt, "OWNER_PHYS_SUBURB");
		 			OWNER_PHYS_TOWN = (string)OracleUdt.GetValue(con, pUdt, "OWNER_PHYS_TOWN");
		 			OWNER_PHYS_CODE = (string)OracleUdt.GetValue(con, pUdt, "OWNER_PHYS_CODE");
		 			OWNER_TELEPHONE = (string)OracleUdt.GetValue(con, pUdt, "OWNER_TELEPHONE");
		 			OWNER_CELLPHONE = (string)OracleUdt.GetValue(con, pUdt, "OWNER_CELLPHONE");
		 			OWNER_COMPANY = (string)OracleUdt.GetValue(con, pUdt, "OWNER_COMPANY");
		 			DATE_OF_OWNERSHIP = (DateTime?)OracleUdt.GetValue(con, pUdt, "DATE_OF_OWNERSHIP");
		 			IMPORT_FILE_NAME = (string)OracleUdt.GetValue(con, pUdt, "IMPORT_FILE_NAME");
		 			NATURE_OF_OWNERSHIP = (string)OracleUdt.GetValue(con, pUdt, "NATURE_OF_OWNERSHIP");
		 			PROXY_INDICATOR = (string)OracleUdt.GetValue(con, pUdt, "PROXY_INDICATOR");
		 			EMAIL_ADDRESS = (string)OracleUdt.GetValue(con, pUdt, "EMAIL_ADDRESS");
		 		  }
	}

	[Oracle.DataAccess.Types.OracleCustomTypeMappingAttribute("ITS.NATIS_VEHICLE_DETAIL_TYPE")]
	public class TISTypeFactory : Oracle.DataAccess.Types.IOracleCustomTypeFactory
	{

		public virtual Oracle.DataAccess.Types.IOracleCustomType CreateObject()
		{
			return new TISType();
		}
	}

	[Oracle.DataAccess.Types.OracleCustomTypeMappingAttribute("ITS.TABLE_NATIS_VEH_DETAIL_TYPE")]
	public class TISTypeArrayFactory : Oracle.DataAccess.Types.IOracleArrayTypeFactory
	{

		public System.Array CreateArray(int numElems)
		{
			return new TISType[numElems];
		}

		public System.Array CreateStatusArray(int numElems)
		{
			return null;
		}
	}
}