
namespace TMT.Enforcement.iLog.Persistence.OracleTableTypeClasses
{
		
	//using Oracle.DataAccess.Client;
	using Oracle.DataAccess.Types;
	using System;

	public class CameraStatistics : Oracle.DataAccess.Types.INullable, Oracle.DataAccess.Types.IOracleCustomType
	{   
  
		private readonly bool isNull = false;

		bool INullable.IsNull
		{
			get
			{
				return this.isNull;
			}
		}

				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("STATS_FILE_NAME")]
		public string STATS_FILE_NAME {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("MACHINE_ID")]
		public string MACHINE_ID {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("LOCATION_CODE")]
		public string LOCATION_CODE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("RUN_DATE")]
		public string RUN_DATE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("TIME")]
		public string TIME {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("SPEED")]
		public long? SPEED {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("ZONE")]
		public long? ZONE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("LANE")]
		public long? LANE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("TYPE")]
		public string TYPE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("DISTANCE")]
		public long? DISTANCE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("DIRECTION")]
		public string DIRECTION {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("CLASSIFICATION")]
		public string CLASSIFICATION {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("CAPTURED")]
		public string CAPTURED {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("ENC_FILE_NAME")]
		public string ENC_FILE_NAME {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("ERROR_MESSAGE")]
		public string ERROR_MESSAGE {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("SMD_STRING")]
		public string SMD_STRING {get;set;}
		
				 
		
		[Oracle.DataAccess.Types.OracleObjectMappingAttribute("PLATES")]
		public string PLATES {get;set;}
		
				  public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, System.IntPtr pUdt)
		{
					OracleUdt.SetValue(con, pUdt, "STATS_FILE_NAME", STATS_FILE_NAME);
		 			OracleUdt.SetValue(con, pUdt, "MACHINE_ID", MACHINE_ID);
		 			OracleUdt.SetValue(con, pUdt, "LOCATION_CODE", LOCATION_CODE);
		 			OracleUdt.SetValue(con, pUdt, "RUN_DATE", RUN_DATE);
		 			OracleUdt.SetValue(con, pUdt, "TIME", TIME);
		 			OracleUdt.SetValue(con, pUdt, "SPEED", SPEED);
		 			OracleUdt.SetValue(con, pUdt, "ZONE", ZONE);
		 			OracleUdt.SetValue(con, pUdt, "LANE", LANE);
		 			OracleUdt.SetValue(con, pUdt, "TYPE", TYPE);
		 			OracleUdt.SetValue(con, pUdt, "DISTANCE", DISTANCE);
		 			OracleUdt.SetValue(con, pUdt, "DIRECTION", DIRECTION);
		 			OracleUdt.SetValue(con, pUdt, "CLASSIFICATION", CLASSIFICATION);
		 			OracleUdt.SetValue(con, pUdt, "CAPTURED", CAPTURED);
		 			OracleUdt.SetValue(con, pUdt, "ENC_FILE_NAME", ENC_FILE_NAME);
		 			OracleUdt.SetValue(con, pUdt, "ERROR_MESSAGE", ERROR_MESSAGE);
		 			OracleUdt.SetValue(con, pUdt, "SMD_STRING", SMD_STRING);
		 			OracleUdt.SetValue(con, pUdt, "PLATES", PLATES);
		 		  }
	 
	 public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, System.IntPtr pUdt)
		{
					STATS_FILE_NAME = (string)OracleUdt.GetValue(con, pUdt, "STATS_FILE_NAME");
		 			MACHINE_ID = (string)OracleUdt.GetValue(con, pUdt, "MACHINE_ID");
		 			LOCATION_CODE = (string)OracleUdt.GetValue(con, pUdt, "LOCATION_CODE");
		 			RUN_DATE = (string)OracleUdt.GetValue(con, pUdt, "RUN_DATE");
		 			TIME = (string)OracleUdt.GetValue(con, pUdt, "TIME");
		 			SPEED = (long?)OracleUdt.GetValue(con, pUdt, "SPEED");
		 			ZONE = (long?)OracleUdt.GetValue(con, pUdt, "ZONE");
		 			LANE = (long?)OracleUdt.GetValue(con, pUdt, "LANE");
		 			TYPE = (string)OracleUdt.GetValue(con, pUdt, "TYPE");
		 			DISTANCE = (long?)OracleUdt.GetValue(con, pUdt, "DISTANCE");
		 			DIRECTION = (string)OracleUdt.GetValue(con, pUdt, "DIRECTION");
		 			CLASSIFICATION = (string)OracleUdt.GetValue(con, pUdt, "CLASSIFICATION");
		 			CAPTURED = (string)OracleUdt.GetValue(con, pUdt, "CAPTURED");
		 			ENC_FILE_NAME = (string)OracleUdt.GetValue(con, pUdt, "ENC_FILE_NAME");
		 			ERROR_MESSAGE = (string)OracleUdt.GetValue(con, pUdt, "ERROR_MESSAGE");
		 			SMD_STRING = (string)OracleUdt.GetValue(con, pUdt, "SMD_STRING");
		 			PLATES = (string)OracleUdt.GetValue(con, pUdt, "PLATES");
		 		  }
	}

	[Oracle.DataAccess.Types.OracleCustomTypeMappingAttribute("REPORTING.CAMERA_STATISTICS_TYPE")]
	public class CameraStatisticsFactory : Oracle.DataAccess.Types.IOracleCustomTypeFactory
	{

		public virtual Oracle.DataAccess.Types.IOracleCustomType CreateObject()
		{
			return new CameraStatistics();
		}
	}

	[Oracle.DataAccess.Types.OracleCustomTypeMappingAttribute("REPORTING.TABLE_CAMERA_STATISTICS_TYPE")]
	public class CameraStatisticsArrayFactory : Oracle.DataAccess.Types.IOracleArrayTypeFactory
	{

		public System.Array CreateArray(int numElems)
		{
			return new CameraStatistics[numElems];
		}

		public System.Array CreateStatusArray(int numElems)
		{
			return null;
		}
	}
}