
namespace TMT.Build.OracleTableTypeClasses
{
	    
	//using Oracle.DataAccess.Client;
    using Oracle.DataAccess.Types;
    using System;

    public class HandWrittenCapture : Oracle.DataAccess.Types.INullable, Oracle.DataAccess.Types.IOracleCustomType
    {   
  
        private readonly bool isNull = false;

		bool INullable.IsNull
        {
            get
            {
                return this.isNull;
            }
        }

				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("TICKET_NUMBER")]
        public string TICKET_NUMBER {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("PERSON_INFO_ID")]
        public long? PERSON_INFO_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("TITLE")]
        public string TITLE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("FIRST_NAME")]
        public string FIRST_NAME {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("MIDDLE_NAMES")]
        public string MIDDLE_NAMES {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("SURNAME")]
        public string SURNAME {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("INITIALS")]
        public string INITIALS {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("IDENTIFICATION_NUMBER")]
        public string IDENTIFICATION_NUMBER {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("IDENTIFICATION_TYPE_ID")]
        public long? IDENTIFICATION_TYPE_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("IDENTIFICATION_COUNTRY_ID")]
        public long? IDENTIFICATION_COUNTRY_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CITIZEN_TYPE_ID")]
        public long? CITIZEN_TYPE_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("GENDER")]
        public string GENDER {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("AGE")]
        public int? AGE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("BIRTHDATE")]
        public DateTime? BIRTHDATE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("OCCUPATION")]
        public string OCCUPATION {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("TELEPHONE")]
        public string TELEPHONE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("MOBILE_NUMBER")]
        public string MOBILE_NUMBER {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("FAX")]
        public string FAX {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("EMAIL")]
        public string EMAIL {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("COMPANY")]
        public string COMPANY {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("BUSINESS_TELEPHONE")]
        public string BUSINESS_TELEPHONE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("PHYSICAL_ADDRESS_INFO_ID")]
        public long? PHYSICAL_ADDRESS_INFO_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("PHYSICAL_ADDRESS_TYPE_ID")]
        public long? PHYSICAL_ADDRESS_TYPE_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("PHYSICAL_STREET_1")]
        public string PHYSICAL_STREET_1 {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("PHYSICAL_STREET_2")]
        public string PHYSICAL_STREET_2 {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("PHYSICAL_SUBURB")]
        public string PHYSICAL_SUBURB {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("PHYSICAL_TOWN")]
        public string PHYSICAL_TOWN {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("PHYSICAL_CODE")]
        public string PHYSICAL_CODE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("POSTAL_ADDRESS_INFO_ID")]
        public long? POSTAL_ADDRESS_INFO_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("POSTAL_ADDRESS_TYPE_ID")]
        public long? POSTAL_ADDRESS_TYPE_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("POSTAL_PO_BOX")]
        public string POSTAL_PO_BOX {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("POSTAL_STREET")]
        public string POSTAL_STREET {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("POSTAL_SUBURB")]
        public string POSTAL_SUBURB {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("POSTAL_TOWN")]
        public string POSTAL_TOWN {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("POSTAL_CODE")]
        public string POSTAL_CODE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("OFFENCE_LOCATION_STREET")]
        public string OFFENCE_LOCATION_STREET {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("OFFENCE_LOCATION_SUBURB")]
        public string OFFENCE_LOCATION_SUBURB {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("OFFENCE_LOCATION_TOWN")]
        public string OFFENCE_LOCATION_TOWN {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("OFFENCE_LOCATION_LATITUDE")]
        public decimal? OFFENCE_LOCATION_LATITUDE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("OFFENCE_LOCATION_LONGITUDE")]
        public decimal? OFFENCE_LOCATION_LONGITUDE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_REGISTRATION_MAIN")]
        public string VEHICLE_REGISTRATION_MAIN {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_REGISTRATION_NO_2")]
        public string VEHICLE_REGISTRATION_NO_2 {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_REGISTRATION_NO_3")]
        public string VEHICLE_REGISTRATION_NO_3 {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_MAKE_MAIN")]
        public string VEHICLE_MAKE_MAIN {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_MODEL_MAIN")]
        public string VEHICLE_MODEL_MAIN {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_TYPE_MAIN")]
        public string VEHICLE_TYPE_MAIN {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_LICENCE_EXPIRY_DATE")]
        public DateTime? VEHICLE_LICENCE_EXPIRY_DATE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_COLOUR")]
        public string VEHICLE_COLOUR {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_REGISTER_NUMBER")]
        public string VEHICLE_REGISTER_NUMBER {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_ENGINE_NUMBER")]
        public string VEHICLE_ENGINE_NUMBER {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("VEHICLE_CHASSIS_NUMBER")]
        public string VEHICLE_CHASSIS_NUMBER {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("GUARDIAN")]
        public string GUARDIAN {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("DIRECTION")]
        public string DIRECTION {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("METER_NUMBER")]
        public string METER_NUMBER {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CASE_NUMBER")]
        public string CASE_NUMBER {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CC_NUMBER")]
        public string CC_NUMBER {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CHARGE_CODE_1")]
        public string CHARGE_CODE_1 {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CHARGE_CODE_1_ID")]
        public long? CHARGE_CODE_1_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("AMOUNT_1")]
        public decimal? AMOUNT_1 {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CHARGE_CODE_2")]
        public string CHARGE_CODE_2 {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CHARGE_CODE_2_ID")]
        public long? CHARGE_CODE_2_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("AMOUNT_2")]
        public decimal? AMOUNT_2 {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CHARGE_CODE_3")]
        public string CHARGE_CODE_3 {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CHARGE_CODE_3_ID")]
        public long? CHARGE_CODE_3_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("AMOUNT_3")]
        public decimal? AMOUNT_3 {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("HAS_ALTERNATIVE_CHARGE")]
        public long? HAS_ALTERNATIVE_CHARGE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("OFFENCE_DATE")]
        public DateTime? OFFENCE_DATE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("COURT_DATE")]
        public DateTime? COURT_DATE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("COURT_NAME")]
        public string COURT_NAME {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("COURT_ROOM")]
        public string COURT_ROOM {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("DISTRICT_NAME")]
        public string DISTRICT_NAME {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("PAYMENT_PLACE")]
        public string PAYMENT_PLACE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("PAYMENT_DATE")]
        public DateTime? PAYMENT_DATE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("OFFICER_CREDENTIAL_ID")]
        public long? OFFICER_CREDENTIAL_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CAPTURED_DATE")]
        public DateTime? CAPTURED_DATE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CAPTURED_CREDENTIAL_ID")]
        public long? CAPTURED_CREDENTIAL_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("LICENCE_CODE")]
        public string LICENCE_CODE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("LICENCE_TYPE")]
        public string LICENCE_TYPE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("DRIVER_LICENCE_CERTIFICATE_NO")]
        public string DRIVER_LICENCE_CERTIFICATE_NO {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("MODIFIED_DATE")]
        public DateTime? MODIFIED_DATE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("MODIFIED_CREDENTIAL_ID")]
        public long? MODIFIED_CREDENTIAL_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("SPEED")]
        public long? SPEED {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("MASS_PERCENTAGE")]
        public decimal? MASS_PERCENTAGE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("IS_CANCELLED")]
        public long? IS_CANCELLED {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CANCEL_REASON")]
        public string CANCEL_REASON {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("SEND_TO_COURT_ROLE")]
        public long? SEND_TO_COURT_ROLE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("NOTES")]
        public string NOTES {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("APPLICATION_AND_VERSION")]
        public string APPLICATION_AND_VERSION {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("ISSUED_DATE")]
        public DateTime? ISSUED_DATE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("MACHINE_IDENTIFIER")]
        public string MACHINE_IDENTIFIER {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CAMERA_HWID")]
        public string CAMERA_HWID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("EVENT_ID")]
        public string EVENT_ID {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("INFRINGEMENT_LOCATION_CODE")]
        public string INFRINGEMENT_LOCATION_CODE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("EXTERNAL_TOKEN")]
        public string EXTERNAL_TOKEN {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("EXTERNAL_REFERENCE")]
        public string EXTERNAL_REFERENCE {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CHARGE_DESCRIPTION_1")]
        public string CHARGE_DESCRIPTION_1 {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CHARGE_DESCRIPTION_2")]
        public string CHARGE_DESCRIPTION_2 {get;set;}
        
				 
		
        [Oracle.DataAccess.Types.OracleObjectMappingAttribute("CHARGE_DESCRIPTION_3")]
        public string CHARGE_DESCRIPTION_3 {get;set;}
        
		          public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, System.IntPtr pUdt)
        {
		            OracleUdt.SetValue(con, pUdt, "TICKET_NUMBER", TICKET_NUMBER);
		             OracleUdt.SetValue(con, pUdt, "PERSON_INFO_ID", PERSON_INFO_ID);
		             OracleUdt.SetValue(con, pUdt, "TITLE", TITLE);
		             OracleUdt.SetValue(con, pUdt, "FIRST_NAME", FIRST_NAME);
		             OracleUdt.SetValue(con, pUdt, "MIDDLE_NAMES", MIDDLE_NAMES);
		             OracleUdt.SetValue(con, pUdt, "SURNAME", SURNAME);
		             OracleUdt.SetValue(con, pUdt, "INITIALS", INITIALS);
		             OracleUdt.SetValue(con, pUdt, "IDENTIFICATION_NUMBER", IDENTIFICATION_NUMBER);
		             OracleUdt.SetValue(con, pUdt, "IDENTIFICATION_TYPE_ID", IDENTIFICATION_TYPE_ID);
		             OracleUdt.SetValue(con, pUdt, "IDENTIFICATION_COUNTRY_ID", IDENTIFICATION_COUNTRY_ID);
		             OracleUdt.SetValue(con, pUdt, "CITIZEN_TYPE_ID", CITIZEN_TYPE_ID);
		             OracleUdt.SetValue(con, pUdt, "GENDER", GENDER);
		             OracleUdt.SetValue(con, pUdt, "AGE", AGE);
		             OracleUdt.SetValue(con, pUdt, "BIRTHDATE", BIRTHDATE);
		             OracleUdt.SetValue(con, pUdt, "OCCUPATION", OCCUPATION);
		             OracleUdt.SetValue(con, pUdt, "TELEPHONE", TELEPHONE);
		             OracleUdt.SetValue(con, pUdt, "MOBILE_NUMBER", MOBILE_NUMBER);
		             OracleUdt.SetValue(con, pUdt, "FAX", FAX);
		             OracleUdt.SetValue(con, pUdt, "EMAIL", EMAIL);
		             OracleUdt.SetValue(con, pUdt, "COMPANY", COMPANY);
		             OracleUdt.SetValue(con, pUdt, "BUSINESS_TELEPHONE", BUSINESS_TELEPHONE);
		             OracleUdt.SetValue(con, pUdt, "PHYSICAL_ADDRESS_INFO_ID", PHYSICAL_ADDRESS_INFO_ID);
		             OracleUdt.SetValue(con, pUdt, "PHYSICAL_ADDRESS_TYPE_ID", PHYSICAL_ADDRESS_TYPE_ID);
		             OracleUdt.SetValue(con, pUdt, "PHYSICAL_STREET_1", PHYSICAL_STREET_1);
		             OracleUdt.SetValue(con, pUdt, "PHYSICAL_STREET_2", PHYSICAL_STREET_2);
		             OracleUdt.SetValue(con, pUdt, "PHYSICAL_SUBURB", PHYSICAL_SUBURB);
		             OracleUdt.SetValue(con, pUdt, "PHYSICAL_TOWN", PHYSICAL_TOWN);
		             OracleUdt.SetValue(con, pUdt, "PHYSICAL_CODE", PHYSICAL_CODE);
		             OracleUdt.SetValue(con, pUdt, "POSTAL_ADDRESS_INFO_ID", POSTAL_ADDRESS_INFO_ID);
		             OracleUdt.SetValue(con, pUdt, "POSTAL_ADDRESS_TYPE_ID", POSTAL_ADDRESS_TYPE_ID);
		             OracleUdt.SetValue(con, pUdt, "POSTAL_PO_BOX", POSTAL_PO_BOX);
		             OracleUdt.SetValue(con, pUdt, "POSTAL_STREET", POSTAL_STREET);
		             OracleUdt.SetValue(con, pUdt, "POSTAL_SUBURB", POSTAL_SUBURB);
		             OracleUdt.SetValue(con, pUdt, "POSTAL_TOWN", POSTAL_TOWN);
		             OracleUdt.SetValue(con, pUdt, "POSTAL_CODE", POSTAL_CODE);
		             OracleUdt.SetValue(con, pUdt, "OFFENCE_LOCATION_STREET", OFFENCE_LOCATION_STREET);
		             OracleUdt.SetValue(con, pUdt, "OFFENCE_LOCATION_SUBURB", OFFENCE_LOCATION_SUBURB);
		             OracleUdt.SetValue(con, pUdt, "OFFENCE_LOCATION_TOWN", OFFENCE_LOCATION_TOWN);
		             OracleUdt.SetValue(con, pUdt, "OFFENCE_LOCATION_LATITUDE", OFFENCE_LOCATION_LATITUDE);
		             OracleUdt.SetValue(con, pUdt, "OFFENCE_LOCATION_LONGITUDE", OFFENCE_LOCATION_LONGITUDE);
		             OracleUdt.SetValue(con, pUdt, "VEHICLE_REGISTRATION_MAIN", VEHICLE_REGISTRATION_MAIN);
		             OracleUdt.SetValue(con, pUdt, "VEHICLE_REGISTRATION_NO_2", VEHICLE_REGISTRATION_NO_2);
		             OracleUdt.SetValue(con, pUdt, "VEHICLE_REGISTRATION_NO_3", VEHICLE_REGISTRATION_NO_3);
		             OracleUdt.SetValue(con, pUdt, "VEHICLE_MAKE_MAIN", VEHICLE_MAKE_MAIN);
		             OracleUdt.SetValue(con, pUdt, "VEHICLE_MODEL_MAIN", VEHICLE_MODEL_MAIN);
		             OracleUdt.SetValue(con, pUdt, "VEHICLE_TYPE_MAIN", VEHICLE_TYPE_MAIN);
		             OracleUdt.SetValue(con, pUdt, "VEHICLE_LICENCE_EXPIRY_DATE", VEHICLE_LICENCE_EXPIRY_DATE);
		             OracleUdt.SetValue(con, pUdt, "VEHICLE_COLOUR", VEHICLE_COLOUR);
		             OracleUdt.SetValue(con, pUdt, "VEHICLE_REGISTER_NUMBER", VEHICLE_REGISTER_NUMBER);
		             OracleUdt.SetValue(con, pUdt, "VEHICLE_ENGINE_NUMBER", VEHICLE_ENGINE_NUMBER);
		             OracleUdt.SetValue(con, pUdt, "VEHICLE_CHASSIS_NUMBER", VEHICLE_CHASSIS_NUMBER);
		             OracleUdt.SetValue(con, pUdt, "GUARDIAN", GUARDIAN);
		             OracleUdt.SetValue(con, pUdt, "DIRECTION", DIRECTION);
		             OracleUdt.SetValue(con, pUdt, "METER_NUMBER", METER_NUMBER);
		             OracleUdt.SetValue(con, pUdt, "CASE_NUMBER", CASE_NUMBER);
		             OracleUdt.SetValue(con, pUdt, "CC_NUMBER", CC_NUMBER);
		             OracleUdt.SetValue(con, pUdt, "CHARGE_CODE_1", CHARGE_CODE_1);
		             OracleUdt.SetValue(con, pUdt, "CHARGE_CODE_1_ID", CHARGE_CODE_1_ID);
		             OracleUdt.SetValue(con, pUdt, "AMOUNT_1", AMOUNT_1);
		             OracleUdt.SetValue(con, pUdt, "CHARGE_CODE_2", CHARGE_CODE_2);
		             OracleUdt.SetValue(con, pUdt, "CHARGE_CODE_2_ID", CHARGE_CODE_2_ID);
		             OracleUdt.SetValue(con, pUdt, "AMOUNT_2", AMOUNT_2);
		             OracleUdt.SetValue(con, pUdt, "CHARGE_CODE_3", CHARGE_CODE_3);
		             OracleUdt.SetValue(con, pUdt, "CHARGE_CODE_3_ID", CHARGE_CODE_3_ID);
		             OracleUdt.SetValue(con, pUdt, "AMOUNT_3", AMOUNT_3);
		             OracleUdt.SetValue(con, pUdt, "HAS_ALTERNATIVE_CHARGE", HAS_ALTERNATIVE_CHARGE);
		             OracleUdt.SetValue(con, pUdt, "OFFENCE_DATE", OFFENCE_DATE);
		             OracleUdt.SetValue(con, pUdt, "COURT_DATE", COURT_DATE);
		             OracleUdt.SetValue(con, pUdt, "COURT_NAME", COURT_NAME);
		             OracleUdt.SetValue(con, pUdt, "COURT_ROOM", COURT_ROOM);
		             OracleUdt.SetValue(con, pUdt, "DISTRICT_NAME", DISTRICT_NAME);
		             OracleUdt.SetValue(con, pUdt, "PAYMENT_PLACE", PAYMENT_PLACE);
		             OracleUdt.SetValue(con, pUdt, "PAYMENT_DATE", PAYMENT_DATE);
		             OracleUdt.SetValue(con, pUdt, "OFFICER_CREDENTIAL_ID", OFFICER_CREDENTIAL_ID);
		             OracleUdt.SetValue(con, pUdt, "CAPTURED_DATE", CAPTURED_DATE);
		             OracleUdt.SetValue(con, pUdt, "CAPTURED_CREDENTIAL_ID", CAPTURED_CREDENTIAL_ID);
		             OracleUdt.SetValue(con, pUdt, "LICENCE_CODE", LICENCE_CODE);
		             OracleUdt.SetValue(con, pUdt, "LICENCE_TYPE", LICENCE_TYPE);
		             OracleUdt.SetValue(con, pUdt, "DRIVER_LICENCE_CERTIFICATE_NO", DRIVER_LICENCE_CERTIFICATE_NO);
		             OracleUdt.SetValue(con, pUdt, "MODIFIED_DATE", MODIFIED_DATE);
		             OracleUdt.SetValue(con, pUdt, "MODIFIED_CREDENTIAL_ID", MODIFIED_CREDENTIAL_ID);
		             OracleUdt.SetValue(con, pUdt, "SPEED", SPEED);
		             OracleUdt.SetValue(con, pUdt, "MASS_PERCENTAGE", MASS_PERCENTAGE);
		             OracleUdt.SetValue(con, pUdt, "IS_CANCELLED", IS_CANCELLED);
		             OracleUdt.SetValue(con, pUdt, "CANCEL_REASON", CANCEL_REASON);
		             OracleUdt.SetValue(con, pUdt, "SEND_TO_COURT_ROLE", SEND_TO_COURT_ROLE);
		             OracleUdt.SetValue(con, pUdt, "NOTES", NOTES);
		             OracleUdt.SetValue(con, pUdt, "APPLICATION_AND_VERSION", APPLICATION_AND_VERSION);
		             OracleUdt.SetValue(con, pUdt, "ISSUED_DATE", ISSUED_DATE);
		             OracleUdt.SetValue(con, pUdt, "MACHINE_IDENTIFIER", MACHINE_IDENTIFIER);
		             OracleUdt.SetValue(con, pUdt, "CAMERA_HWID", CAMERA_HWID);
		             OracleUdt.SetValue(con, pUdt, "EVENT_ID", EVENT_ID);
		             OracleUdt.SetValue(con, pUdt, "INFRINGEMENT_LOCATION_CODE", INFRINGEMENT_LOCATION_CODE);
		             OracleUdt.SetValue(con, pUdt, "EXTERNAL_TOKEN", EXTERNAL_TOKEN);
		             OracleUdt.SetValue(con, pUdt, "EXTERNAL_REFERENCE", EXTERNAL_REFERENCE);
		             OracleUdt.SetValue(con, pUdt, "CHARGE_DESCRIPTION_1", CHARGE_DESCRIPTION_1);
		             OracleUdt.SetValue(con, pUdt, "CHARGE_DESCRIPTION_2", CHARGE_DESCRIPTION_2);
		             OracleUdt.SetValue(con, pUdt, "CHARGE_DESCRIPTION_3", CHARGE_DESCRIPTION_3);
		 		  }
     
	 public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, System.IntPtr pUdt)
        {
					TICKET_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "TICKET_NUMBER");
		 			PERSON_INFO_ID = (long?)OracleUdt.GetValue(con, pUdt, "PERSON_INFO_ID");
		 			TITLE = (string)OracleUdt.GetValue(con, pUdt, "TITLE");
		 			FIRST_NAME = (string)OracleUdt.GetValue(con, pUdt, "FIRST_NAME");
		 			MIDDLE_NAMES = (string)OracleUdt.GetValue(con, pUdt, "MIDDLE_NAMES");
		 			SURNAME = (string)OracleUdt.GetValue(con, pUdt, "SURNAME");
		 			INITIALS = (string)OracleUdt.GetValue(con, pUdt, "INITIALS");
		 			IDENTIFICATION_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "IDENTIFICATION_NUMBER");
		 			IDENTIFICATION_TYPE_ID = (long?)OracleUdt.GetValue(con, pUdt, "IDENTIFICATION_TYPE_ID");
		 			IDENTIFICATION_COUNTRY_ID = (long?)OracleUdt.GetValue(con, pUdt, "IDENTIFICATION_COUNTRY_ID");
		 			CITIZEN_TYPE_ID = (long?)OracleUdt.GetValue(con, pUdt, "CITIZEN_TYPE_ID");
		 			GENDER = (string)OracleUdt.GetValue(con, pUdt, "GENDER");
		 			AGE = (int?)OracleUdt.GetValue(con, pUdt, "AGE");
		 			BIRTHDATE = (DateTime?)OracleUdt.GetValue(con, pUdt, "BIRTHDATE");
		 			OCCUPATION = (string)OracleUdt.GetValue(con, pUdt, "OCCUPATION");
		 			TELEPHONE = (string)OracleUdt.GetValue(con, pUdt, "TELEPHONE");
		 			MOBILE_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "MOBILE_NUMBER");
		 			FAX = (string)OracleUdt.GetValue(con, pUdt, "FAX");
		 			EMAIL = (string)OracleUdt.GetValue(con, pUdt, "EMAIL");
		 			COMPANY = (string)OracleUdt.GetValue(con, pUdt, "COMPANY");
		 			BUSINESS_TELEPHONE = (string)OracleUdt.GetValue(con, pUdt, "BUSINESS_TELEPHONE");
		 			PHYSICAL_ADDRESS_INFO_ID = (long?)OracleUdt.GetValue(con, pUdt, "PHYSICAL_ADDRESS_INFO_ID");
		 			PHYSICAL_ADDRESS_TYPE_ID = (long?)OracleUdt.GetValue(con, pUdt, "PHYSICAL_ADDRESS_TYPE_ID");
		 			PHYSICAL_STREET_1 = (string)OracleUdt.GetValue(con, pUdt, "PHYSICAL_STREET_1");
		 			PHYSICAL_STREET_2 = (string)OracleUdt.GetValue(con, pUdt, "PHYSICAL_STREET_2");
		 			PHYSICAL_SUBURB = (string)OracleUdt.GetValue(con, pUdt, "PHYSICAL_SUBURB");
		 			PHYSICAL_TOWN = (string)OracleUdt.GetValue(con, pUdt, "PHYSICAL_TOWN");
		 			PHYSICAL_CODE = (string)OracleUdt.GetValue(con, pUdt, "PHYSICAL_CODE");
		 			POSTAL_ADDRESS_INFO_ID = (long?)OracleUdt.GetValue(con, pUdt, "POSTAL_ADDRESS_INFO_ID");
		 			POSTAL_ADDRESS_TYPE_ID = (long?)OracleUdt.GetValue(con, pUdt, "POSTAL_ADDRESS_TYPE_ID");
		 			POSTAL_PO_BOX = (string)OracleUdt.GetValue(con, pUdt, "POSTAL_PO_BOX");
		 			POSTAL_STREET = (string)OracleUdt.GetValue(con, pUdt, "POSTAL_STREET");
		 			POSTAL_SUBURB = (string)OracleUdt.GetValue(con, pUdt, "POSTAL_SUBURB");
		 			POSTAL_TOWN = (string)OracleUdt.GetValue(con, pUdt, "POSTAL_TOWN");
		 			POSTAL_CODE = (string)OracleUdt.GetValue(con, pUdt, "POSTAL_CODE");
		 			OFFENCE_LOCATION_STREET = (string)OracleUdt.GetValue(con, pUdt, "OFFENCE_LOCATION_STREET");
		 			OFFENCE_LOCATION_SUBURB = (string)OracleUdt.GetValue(con, pUdt, "OFFENCE_LOCATION_SUBURB");
		 			OFFENCE_LOCATION_TOWN = (string)OracleUdt.GetValue(con, pUdt, "OFFENCE_LOCATION_TOWN");
		 			OFFENCE_LOCATION_LATITUDE = (decimal?)OracleUdt.GetValue(con, pUdt, "OFFENCE_LOCATION_LATITUDE");
		 			OFFENCE_LOCATION_LONGITUDE = (decimal?)OracleUdt.GetValue(con, pUdt, "OFFENCE_LOCATION_LONGITUDE");
		 			VEHICLE_REGISTRATION_MAIN = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_REGISTRATION_MAIN");
		 			VEHICLE_REGISTRATION_NO_2 = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_REGISTRATION_NO_2");
		 			VEHICLE_REGISTRATION_NO_3 = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_REGISTRATION_NO_3");
		 			VEHICLE_MAKE_MAIN = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_MAKE_MAIN");
		 			VEHICLE_MODEL_MAIN = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_MODEL_MAIN");
		 			VEHICLE_TYPE_MAIN = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_TYPE_MAIN");
		 			VEHICLE_LICENCE_EXPIRY_DATE = (DateTime?)OracleUdt.GetValue(con, pUdt, "VEHICLE_LICENCE_EXPIRY_DATE");
		 			VEHICLE_COLOUR = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_COLOUR");
		 			VEHICLE_REGISTER_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_REGISTER_NUMBER");
		 			VEHICLE_ENGINE_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_ENGINE_NUMBER");
		 			VEHICLE_CHASSIS_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "VEHICLE_CHASSIS_NUMBER");
		 			GUARDIAN = (string)OracleUdt.GetValue(con, pUdt, "GUARDIAN");
		 			DIRECTION = (string)OracleUdt.GetValue(con, pUdt, "DIRECTION");
		 			METER_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "METER_NUMBER");
		 			CASE_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "CASE_NUMBER");
		 			CC_NUMBER = (string)OracleUdt.GetValue(con, pUdt, "CC_NUMBER");
		 			CHARGE_CODE_1 = (string)OracleUdt.GetValue(con, pUdt, "CHARGE_CODE_1");
		 			CHARGE_CODE_1_ID = (long?)OracleUdt.GetValue(con, pUdt, "CHARGE_CODE_1_ID");
		 			AMOUNT_1 = (decimal?)OracleUdt.GetValue(con, pUdt, "AMOUNT_1");
		 			CHARGE_CODE_2 = (string)OracleUdt.GetValue(con, pUdt, "CHARGE_CODE_2");
		 			CHARGE_CODE_2_ID = (long?)OracleUdt.GetValue(con, pUdt, "CHARGE_CODE_2_ID");
		 			AMOUNT_2 = (decimal?)OracleUdt.GetValue(con, pUdt, "AMOUNT_2");
		 			CHARGE_CODE_3 = (string)OracleUdt.GetValue(con, pUdt, "CHARGE_CODE_3");
		 			CHARGE_CODE_3_ID = (long?)OracleUdt.GetValue(con, pUdt, "CHARGE_CODE_3_ID");
		 			AMOUNT_3 = (decimal?)OracleUdt.GetValue(con, pUdt, "AMOUNT_3");
		 			HAS_ALTERNATIVE_CHARGE = (long?)OracleUdt.GetValue(con, pUdt, "HAS_ALTERNATIVE_CHARGE");
		 			OFFENCE_DATE = (DateTime?)OracleUdt.GetValue(con, pUdt, "OFFENCE_DATE");
		 			COURT_DATE = (DateTime?)OracleUdt.GetValue(con, pUdt, "COURT_DATE");
		 			COURT_NAME = (string)OracleUdt.GetValue(con, pUdt, "COURT_NAME");
		 			COURT_ROOM = (string)OracleUdt.GetValue(con, pUdt, "COURT_ROOM");
		 			DISTRICT_NAME = (string)OracleUdt.GetValue(con, pUdt, "DISTRICT_NAME");
		 			PAYMENT_PLACE = (string)OracleUdt.GetValue(con, pUdt, "PAYMENT_PLACE");
		 			PAYMENT_DATE = (DateTime?)OracleUdt.GetValue(con, pUdt, "PAYMENT_DATE");
		 			OFFICER_CREDENTIAL_ID = (long?)OracleUdt.GetValue(con, pUdt, "OFFICER_CREDENTIAL_ID");
		 			CAPTURED_DATE = (DateTime?)OracleUdt.GetValue(con, pUdt, "CAPTURED_DATE");
		 			CAPTURED_CREDENTIAL_ID = (long?)OracleUdt.GetValue(con, pUdt, "CAPTURED_CREDENTIAL_ID");
		 			LICENCE_CODE = (string)OracleUdt.GetValue(con, pUdt, "LICENCE_CODE");
		 			LICENCE_TYPE = (string)OracleUdt.GetValue(con, pUdt, "LICENCE_TYPE");
		 			DRIVER_LICENCE_CERTIFICATE_NO = (string)OracleUdt.GetValue(con, pUdt, "DRIVER_LICENCE_CERTIFICATE_NO");
		 			MODIFIED_DATE = (DateTime?)OracleUdt.GetValue(con, pUdt, "MODIFIED_DATE");
		 			MODIFIED_CREDENTIAL_ID = (long?)OracleUdt.GetValue(con, pUdt, "MODIFIED_CREDENTIAL_ID");
		 			SPEED = (long?)OracleUdt.GetValue(con, pUdt, "SPEED");
		 			MASS_PERCENTAGE = (decimal?)OracleUdt.GetValue(con, pUdt, "MASS_PERCENTAGE");
		 			IS_CANCELLED = (long?)OracleUdt.GetValue(con, pUdt, "IS_CANCELLED");
		 			CANCEL_REASON = (string)OracleUdt.GetValue(con, pUdt, "CANCEL_REASON");
		 			SEND_TO_COURT_ROLE = (long?)OracleUdt.GetValue(con, pUdt, "SEND_TO_COURT_ROLE");
		 			NOTES = (string)OracleUdt.GetValue(con, pUdt, "NOTES");
		 			APPLICATION_AND_VERSION = (string)OracleUdt.GetValue(con, pUdt, "APPLICATION_AND_VERSION");
		 			ISSUED_DATE = (DateTime?)OracleUdt.GetValue(con, pUdt, "ISSUED_DATE");
		 			MACHINE_IDENTIFIER = (string)OracleUdt.GetValue(con, pUdt, "MACHINE_IDENTIFIER");
		 			CAMERA_HWID = (string)OracleUdt.GetValue(con, pUdt, "CAMERA_HWID");
		 			EVENT_ID = (string)OracleUdt.GetValue(con, pUdt, "EVENT_ID");
		 			INFRINGEMENT_LOCATION_CODE = (string)OracleUdt.GetValue(con, pUdt, "INFRINGEMENT_LOCATION_CODE");
		 			EXTERNAL_TOKEN = (string)OracleUdt.GetValue(con, pUdt, "EXTERNAL_TOKEN");
		 			EXTERNAL_REFERENCE = (string)OracleUdt.GetValue(con, pUdt, "EXTERNAL_REFERENCE");
		 			CHARGE_DESCRIPTION_1 = (string)OracleUdt.GetValue(con, pUdt, "CHARGE_DESCRIPTION_1");
		 			CHARGE_DESCRIPTION_2 = (string)OracleUdt.GetValue(con, pUdt, "CHARGE_DESCRIPTION_2");
		 			CHARGE_DESCRIPTION_3 = (string)OracleUdt.GetValue(con, pUdt, "CHARGE_DESCRIPTION_3");
		 		  }
    }

    [Oracle.DataAccess.Types.OracleCustomTypeMappingAttribute("ITS.TYPE_HANDWRITTEN_CAPTURE")]
    public class HandWrittenCaptureFactory : Oracle.DataAccess.Types.IOracleCustomTypeFactory
    {

        public virtual Oracle.DataAccess.Types.IOracleCustomType CreateObject()
        {
            return new HandWrittenCapture();
        }
    }

    [Oracle.DataAccess.Types.OracleCustomTypeMappingAttribute("ITS.TABLE_TYPE_HANDWRITTEN_CAPTURE")]
    public class HandWrittenCaptureArrayFactory : Oracle.DataAccess.Types.IOracleArrayTypeFactory
    {

        public System.Array CreateArray(int numElems)
        {
            return new HandWrittenCapture[numElems];
        }

        public System.Array CreateStatusArray(int numElems)
        {
            return null;
        }
    }
}