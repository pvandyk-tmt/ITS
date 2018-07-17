package za.co.kapsch.iticket;

/**
 * Created by csenekal on 2016-07-13.
 */

public class Constants {

    public static final String ID_TYPE_RSA = App.getContext().getResources().getString(R.string.enum_offender_id_type_rsa);
    public static final String ID_TYPE_NO_ID  = App.getContext().getResources().getString(R.string.enum_offender_id_type_no_id);
    public static final String ID_TYPE_PASSPORT  = App.getContext().getResources().getString(R.string.enum_offender_id_type_passport);

    public static final String VOSI_ACTION_CANCELLED = App.getContext().getResources().getString(R.string.enum_vosi_action_cancelled);
    public static final String VOSI_ACTION_ARREST = App.getContext().getResources().getString(R.string.enum_vosi_action_arrest);
    public static final String VOSI_ACTION_IMPOUND  = App.getContext().getResources().getString(R.string.enum_vosi_action_impound);
    public static final String VOSI_ACTION_PAYMENT  = App.getContext().getResources().getString(R.string.enum_vosi_action_payment);
    public static final String VOSI_ACTION_TYPE = "VosiActionType";
    //public static final String SELECT = "Select";

    public static final String DEBUG_USER = "csenekal@tmtservices.co.za";
    public static final String PRODUCTION_ENVIRONMENT = "Production Environment";
    public static final String PRODUCTION_ENVIRONMENT_MATCH_ERROR = "Production Environment match error";

    public static final String NAG = "NAG";

    public static final String ITICKET_REGISTRATION = "ITICKET_REGISTRATION";
    public static final String REQUEST_METHOD_GET = "GET";
    public static final String REQUEST_METHOD_POST = "POST";
    public static final String SAVED_PRINTER_FIELD_SEPERATOR = "-seperator-";
    public static final String PRINTER_MAC_ADDRESS = "printerMacAddress";
    public static final String SA_DRIVERS_DESERIALIZER_KEY = "SaDriversLicenceDeserializerKey";
    public static final String SA_DESERIALIZER_LIC_OYKV8PMC = "oBim66srcCnUbb3vv4YYXSRv82bsqA3BiVuB+NMTkYCuQiXlbKcCAdYVRetiPcYisEw5xl05vz3qlPIS3cSHOlppanrnK8W7i5nBK1rgQgO2htn8pBuevIRI9kXmaJ5tOL8iYprfXRaa9mqUimLVEJ+WLhmbTKDdvAJYoiNkDJ4=";
    public static final String INDEX_TEXT = "index";
//    public static final String BARCODE_SCAN_RESULT = "barcodeScanResult";
    public static final String ADDRESS_SEARCH_RESULT = "addressSearchResult";
//    public static final String DRIVERS_CARD_SCAN_RESULT = "driversCardScanResult";
//    public static final String CHARGE_QUERY_ONE_RESULT = "chargeQueryOneResult";
//    public static final String CHARGE_QUERY_TWO_RESULT = "chargeQueryTwoResult";
    public static final String CHARGE_QUERY_RESULT = "chargeQueryResult";
    public static final String DISTRICT_RESULT = "districtResult";
    public static final String REGISTER_DEVICE = "registerDevice";
    public static final String DELIMITATION_DATA = "delimitationData";
    public static final String ADDRESS_TYPE = "addressType";
    public static final String DISTANCE_OVER_TIME_RESULT = "distanceOverTimeResult";
    public static final String OUTSTANDING_PERSON_VIOLATIONS = "OutstandingPersonViolations";
    public static final String OUTSTANDING_VEHICLE_VIOLATIONS = "OutstandingVehicleViolations";
    public static final String VEHICLE_CATEGORY = "VehicleCategory";

//    public static final int SCAN_REQUEST_CODE = 1;
    public static final int ADDRESS_LIST_REQUEST_CODE = 2;
    public static final int CHARGE_REQUEST = 3;
    public static final int CHARGE_ONE_REQUEST = 4;
    public static final int CHARGE_TWO_REQUEST = 5;
    public static final int CHARGE_THREE_REQUEST = 24;
    public static final int DAYS_60 = 60;
    public static final int VEHICLE_DISK_DATA_FIELD_COUNT = 15;
    public static final String EMPTY_STRING = "";
    public static final int SIGNATURE_REQUEST_CODE = 6;
    public static final int COURT_REQUEST_CODE = 7;
    public static final int DATA_SYNC_REQUEST_CODE = 8;
    public static final int LOGIN_REQUEST_CODE = 9;
    public static final int WIZARD_CANCELLATION_REQUEST_CODE = 10;
    public static final int LOCATION_REQUEST_CODE = 11;
    public static final int REQUEST_TAKE_PHOTO = 12;
    public static final int REQUEST_RECORD_AUDIO = 13;
    public static final int PLACEHOLDER_REPLACED_CHARGE_DESC = 14;
    public static final int EVIDENCE_BITMAP_MAX_SIZE = 1000; //width or height
    public static final int DISTRICT_REQUEST_CODE = 15;
    public static final int ICAM_REQUEST_CODE = 16;
    public static final int DELIMITATION_DATA_REQUEST = 17;
    public static final int OPUS_CHARGE_ONE_REQUEST = 18;
    public static final int OPUS_CHARGE_TWO_REQUEST = 19;
    public static final int VEHICLES_REQUEST_CODE = 20;
    public static final int PLACEHOLDER_REPLACED_CHARGE_DESC_ONE = 21;
    public static final int PLACEHOLDER_REPLACED_CHARGE_DESC_TWO = 22;
    public static final int PLACEHOLDER_REPLACED_CHARGE_DESC_THREE = 25;
    public static final int BLUETOOTH_REQUEST_CODE = 23;
    public static final int DISTANCE_OVER_TIME_REQUEST_CODE = 26;

    //Table column names
    public static final String TABLE_USER_USERNAME_COLUMN = "UserName";
    public static final String TABLE_USER_PASSWORD_COLUMN = "Password";
    public static final String TABLE_USER_INFRASTRUCTURE_NUMBER_COLUMN = "InfrastructureNumber";

    public static final String TABLE_DEVICE_ITEM_DESC_COLUMN = "Desc";
    public static final String TABLE_GPS_LOG_IS_SYNCED_COLUMN = "IsSynced";
    public static final String TABLE_GPS_ID_COLUMN = "ID";
    public static final String TABLE_OPUS_COURTROOM_REF_COURT_COLUMN = "RefCourt";

    public static final String TABLE_COURT_DATE_ROOM_ID_COLUMN = "CourtRoomId";
    public static final String TABLE_COURT_DATE_DATE_COLUMN = "Date";
    public static final String TABLE_COURT_ROOM_COURT_ID_COLUMN = "CourtId";
    public static final String TABLE_OPUS_OFFENCE_GROUP_OPUS_OFFENCE_TYPE_ID_COLUMN = "OpusOffenceTypeId";

    public static final String TABLE_VEHICLE_TYPE_GROUP_ID_COLUMN = "VehicleTypeGroupId";
    public static final String TABLE_VEHICLE_TYPE_ID_COLUMN = "VehicleTypeId";

    public static final String ALL_TABLE_ID_COLUMN = "ID";
    public static final String TABLE_OPUS_NOTICE_NUMBER_STATUS_COLUMN = "Status";
    public static final String TABLE_OPUS_NOTICE_NUMBER_NOTICE_TYPE_COLUMN = "NoticeType";
    public static final String TABLE_OPUS_NOTICE_NUMBER_NOTICE_NUMBER_COLUMN = "NoticeNumber";

    public static final String TABLE_OPUS_VEHICLE_GROUP_TYPE_ID_COLUMN = "VehicleTypeGroupId";
    public static final String TABLE_OPUS_VEHICLE_TYPE_ID_COLUMN = "VehicleTypeId";

    public static final String TABLE_CONFIGURATION_DESCRIPTION_COLUMN = "Description";

    //Activity Titles
    public static final String LOGIN_SCREEN_TITLE = "Login";

    public static final String WIZARD_START_PAGE_INDEX = "WizardStartPageIndex";
    public static final String TICKET_WIZARD_EXTENTION = "TicketWizardExtension";
    public static final String TICKET_MODEL = "TicketModel";
    public static final String ICAM_VLNS = "iCamVlns";
    public static final String TICKET_TYPE = "TicketType";
    public static final String DOCUMENT_TYPE = "DocumentType";
    public static final String SIGNATURE = "Signature";
    public static final String COURT = "Court";
    public static final String VEHICLE = "Vehicle";
    public static final String DELIMITATION = "Delimitation";
    public static final String COURT_INFO = "CourtInfo";
    //public static final String USER = "User";
    //public static final String DISTRICT = "District";
    public static final String WIZARD_CANCELLATION = "WizardCancellation";
    public static final String EVIDENCE_AUDIO = "EvidenceAudio";
    public static final String TICKET_NUMBER = "TicketNunmber";
    public static final String EVIDENCE = "Evidence";
    public static final String CHARGE_SPEED = "ChargeSpeed";
    public static final String CHARGE_DESC = "ChargeDesc";
    public static final String CHARGE_MODEL = "ChargeModel";
    public static final String CHARGE_PRINT_DESC = "ChargePrintDesc";
    public static final String CHARGE_VEHICLE_LICENCE_NUMBER = "VehicleLicenceNumber";
    public static final String COURT_VERIFICATION = "CourtVerification";
    public static final String ICAM_INFRINGEMENT = "iCamInfringement";
    public static final String ICAM_EVENT_LIST = "iCamEventList";
    public static final String SECTION_MODEL = "SectionModel";

    public static final String TIME_FORMAT = "HH:mm a";
    public static final String DATE_FORMAT = "dd/MM/yyyy";
    public static final String DATETIME_FORMAT = "dd/MM/yyyy HH:mm a";
    public static final String DATETIME_FORMAT_EX = "yyyy/MM/dd HH:mm:ss";
    public static final String VEHICLE_DISC_DATE_FORMAT = "yyyy-MM-dd";
    //public static final String VEHICLE_DISC_DATE_FORMAT = "dd/MM/yyyy";

    public static final String ADDRESS_LIST_EXTRA = "addressList";
    public static final String PRINTER_LIST_EXTRA = "printerList";
    public static final String CHARGE_CODE = "chargeCode";
    public static final String CHARGE_QUERY_TYPE = "chargeQueryType";
    public static final String MEDIA_PLAYER_TYPE = "chargeQueryType";

    public static final String AND = "&";
    public static final String REG_EXPRESSION_OR = "\\|";
    public static final String REG_EXPRESSION_AND_OR = "&|\\|";
    public static final String OR = "|";
    public static final String PERCENTAGE = "%";

    public static final String CHARGE_REQUEST_DESC = "chargeRequest";

    public static final String INFRINGEMENT_CHARGE = "infringementCharge";
    //public static final String OFFENCE_CODE = "OffenceCode";

    public static final String DEVICE_ID = "deviceId";
    public static final String APP_VERSION = "appVersion";
    public static final String OFFENCE_SET_LK_VERSION = "offenceSetLkVersion";
    public static final String OFFICER_LK_VERSION = "officerLkVersion";
    public static final String COURTS_LK_VERSION = "courtsLkVersion";
    public static final String VEHICLES_LK_VERSION = "vehiclesLkVersion";
    public static final String DATABASE_UPDATE_VERSION = "databaseUpdateVersion";
    public static final String PUBLIC_HOLIDAY_LK_VERSION = "publicHolidayLkVersion";
    public static final String DLC_SERIALIZER_RSA_LIC = "dlcSerializerRsaLic";
    public static final String DEVICE_CONFIGURATION_VERSION = "deviceConfigurationVersion";
    public static final String TOGGLE_WIFI = "toggleWIfi";

    public static final int YES = -1;
    public static final int NO = -2;

    public static final String LOCATION_ACTION = "location_action";
    public static final String LOCATION_RESULT = "location_result";
    public static final String LOCATION_ERROR = "error";

    public static final String DOT_INFRINGEMENT_ACTION = "dot_infringement_action";
    public static final String DOT_INFRINGEMENT_RESULT = "dot_infringement_restult";

    public static final String DOT_INFRINGEMENT_RESTART_ACTION = "dot_infringement_restart_action";
    public static final String DOT_INFRINGEMENT_RESTART_RESULT = "dot_infringement_restart_restult";

    //public static final String TEMP_EVIDENCE_FOLDER = "TicketEvidence";
    //public static final String ITICKET_FOLDER = "TMT iTicket";
    public static final String UPDATE_APK_FILENAME = "iTicket.apk";

    public static final String PICTURE = "picture";
    public static final String VOICE = "voice";
    public static final String OFFICER_SIGNATURE = "officer signature";
    public static final String PERSON_SIGNATURE = "person signature";

    //public static final String NO_EVIDENCE_MESSAGE = "This ticket does not have evidence";

    public static final String AUDIO_FILE_DATA = "audioFileData";
    public static final String IMAGE_FILE_DATA = "imageFileData";
    public static final String TEMP_VOICE_EVIDENCE_FILENAME = "ticketVoiceEvidence.mp4";
    public static final String TEMP_PICTURE_EVIDENCE_FILENAME = "ticketPictureEvidence.jpeg";
    public static final String TEMP_DRIVER_IMAGE_FILENAME = "tempDriverImage.jpeg";

    public static final int AUDIO_CAPTURE_PROGRESS = 0;
    public static final int AUDIO_REPLAY_PROGRESS = 1;

    public static final String DRIVER_LICENSE_LICENCE = "driver&license|licence";
    public static final String VEHICLE_LICENSE_LICENCE = "vehicle&license|licence";
    public static final String STOP = "stop";
    public static final String SIGN_ZONE = "sign|zone";
    public static final String BELT = "belt";
    public static final String TYRE = "tyre";
    public static final String CELL_MOBILE = "cell|mobile";
    public static final String ROADWORTHY = "roadworthy";
    public static final String CODE = "Code";
    public static final String FAVOURITES = "Favourites";
    public static final String DESC = "Desc";

    public static final String SOUTH_AFRICA = "South Africa";
    public static final String BASIC_GOOGLE_SEARCH_STRING = "%s, South Africa";

    public static final int PROCESS_ID_ICAM_VLNS = 1;
    public static final int PROCESS_ID_ICAM_INFRINGEMENT = 2;
    public static final int PROCESS_ID_ICAM_CONNECTION = 3;
    public static final int PROCESS_ID_GOOGLE = 4;
    public static final int PROCESS_ID_OPUS_NOTICE_NUMBER_REQUEST = 5;
    public static final int PROCESS_ID_OPUS_AUTHENTICATION = 6;
    public static final int PROCESS_ID_QUERY_UPDATES = 7;
    public static final int PROCESS_ID_EXECUTE_UPDATES = 8;
    public static final int PROCESS_ID_GOOGLE_ADDRESS_SEARCH_BY_ADDRESS = 11;
    public static final int PROCESS_ID_PERSON_ADDRESS_INFO_SEARCH = 13;
    public static final int PROCESS_ID_UPLOAD_GPS_LOGS = 14;
    public static final int PROCESS_ID_DOWNLOAD_APK = 15;
    public static final int PROCESS_ID_DOWNLOAD_DISTRICTS = 16;
    public static final int PROCESS_ID_REGISTER_DEVICE = 17;
    public static final int PROCESS_ID_DOWNLOAD_COURTS_INFO = 18;

    public static final int PROCESS_ID_ASYNC_PROCESS_PRINT = 19;
    public static final int PROCESS_ID_UPLOAD_SECTION_56 = 20;
    public static final int PROCESS_ID_UPLOAD_SECTION_341 = 21;

    public static final int PROCESS_ID_GET_FIELD_DEVICE = 22;

    public static final int PROCESS_ID_DOWNLOAD_OFFENCE_CODES = 23;
    public static final int PROCESS_ID_DOWNLOAD_TICKET_NUMBERS = 24;
    public static final int PROCESS_ID_DOWNLOAD_USERS = 25;
    public static final int PROCESS_ID_DOWNLOAD_PUBLIC_HOLIDAYS = 26;
    public static final int PROCESS_ID_DOWNLOAD_COURT_DETAIL = 27;
    public static final int PROCESS_ID_DOWNLOAD_RSA_DESERIALISER_LIC = 29;
    public static final int PROCESS_ID_UPLOAD_EVIDENCE = 30;
    public static final int PROCESS_ID_DOWNLOAD_VEHICLES_INFO = 31;
    public static final int PROCESS_ID_SET_TICKET56_COMPLETE = 32;
    public static final int PROCESS_ID_SET_TICKET341_COMPLETE = 33;
    public static final int PROCESS_ID_GET_DEVICE_CONFIGURATION = 34;
    public static final int PROCESS_ID_VOSI_LOOKUP = 35;
    public static final int PROCESS_ID_ENATIS_VEHICLE_LOOKUP = 36;
    public static final int PROCESS_ID_DISTANCE_OVER_TIME_SERVER = 37;
    public static final int PROCESS_ID_DISTANCE_OVER_TIME_SERVICE_REQUEST = 38;
    public static final int PROCESS_ID_DOWNLOAD_TICKET_NUMBER_RANGE = 39;
    public static final int PROCESS_ID_GET_DEVICE_CONFIG_ITEM = 40;
    public static final int PROCESS_ID_DOWNLOAD_CHARGE_INFO = 41;
    public static final int PROCESS_ID_UPLOAD_HANDWRITTEN = 42;
    public static final int PROCESS_ID_DOWNLOAD_VOSI_ACTION = 43;
    public static final int PROCESS_ID_UPLOAD_VOSI_ACTION_CAPTURE = 44;
    public static final int PROCESS_ID_ISSUE_NOTICE = 45;
    public static final int PROCESS_ID_DOWNLOAD_INFRINGEMENT_LOCATIONS = 46;
    public static final int TICKET_WIZARD_REQUEST_CODE = 47;
    public static final int PROCESS_ID_UPLOAD_USER_ACTIVITY_LOG = 48;
    public static final int PROCESS_ID_DOWNLOAD_IDENTIFICATION_TYPE = 49;
    public static final int PROCESS_ID_DOWNLOAD_OUTSTANDING_VIOLATIONS = 50;
    public static final int PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_ID = 51;
    public static final int PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_VLN = 52;
    public static final int PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_REF_NUMBER= 53;
    public static final int PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_TOKEN = 54;
    public static final int PROCESS_ID_DOWNLOAD_COUNTRY = 55;
    public static final int PROCESS_ID_VOSI_ACTION = 56;
    public static final int PROCESS_ID_GET_OFFICER_SIGNATURE_EVIDENCE = 57;
    public static final int PROCESS_ID_GET_OFFENDER_SIGNATURE_EVIDENCE = 58;

    public static final int IDENTIFICATION_TYPE_UNKNOWN_ID = 0;

    public static final String MIME_TYPE_JPEG = "image/jpeg";
    public static final String MIME_TYPE_PNG = "image/png";
    public static final String MIME_TYPE_MP4 = "audio/mp4a";

    public static final String Split = "[$plit@]";

    public static final String REG_EXPRESSION_CHARGE_PLACEHOLDER_PATTERN = "\\[.+?]";

    public static final String OPUS_SPEED_PLACE_HOLDER = "(X~3)"; //SPEED NUMBER
    public static final String OPUS_LIC_NUM_PLACE_HOLDER = "(X~1)"; //REG NUMBER
    public static final String OPUS_PLACE_HOLDER = "*043"; //REG NUMBER

    public static final String ZONE_PLACE_HOLDER = "[ZONE]";
    public static final String SPEED_PLACE_HOLDER = "[SPEED]";
    public static final String VEHREG_PLACE_HOLDER = "[VEHREG]";
    public static final String VEHMAKE_PLACE_HOLDER = "[VEHMAKE]";
    public static final String VEHMODEL_PLACE_HOLDER = "[VEHMODEL]";

     //public static final String[] OPUS_CHARGE_PLACE_HOLDERS = { OPUS_SPEED_PLACE_HOLDER, OPUS_LIC_NUM_PLACE_HOLDER };

    public static final String SYNCHRONISATION_REQUIRED = "SYNCHRONISATION_REQUIRED";
    public static final String FINISHED_MESSAGE = "FINISHED";
    public static final String FAILED_MESSAGE = "FAILED";
    public static final String VERSION = "VERSION";

}
