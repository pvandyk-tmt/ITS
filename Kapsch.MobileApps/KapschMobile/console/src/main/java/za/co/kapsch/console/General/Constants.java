package za.co.kapsch.console.General;

/**
 * Created by csenekal on 2016-07-13.
 */

public class Constants {

    public static final String DEBUG_USER = "csenekal@tmtservices.co.za";
    public static final String PRODUCTION_ENVIRONMENT = "Production Environment";
    public static final String PRODUCTION_ENVIRONMENT_MATCH_ERROR = "Production Environment match error";
    public static final String APK_EXTENTION = ".apk";

    public static final String ITICKET_REGISTRATION = "ITICKET_REGISTRATION";
    public static final String REQUEST_METHOD_GET = "GET";
    public static final String REQUEST_METHOD_POST = "POST";

    public static final String DISTRICT_RESULT = "districtResult";
    public static final String REGISTER_DEVICE = "registerDevice";

    public static final String EMPTY_STRING = "";

    //Activity Titles
    public static final String LOGIN_SCREEN_TITLE = "Login";

    public static final String TIME_FORMAT = "HH:mm a";
    public static final String DATE_FORMAT = "dd/MM/yyyy";
    public static final String DATETIME_FORMAT = "dd/MM/yyyy HH:mm a";
    public static final String VEHICLE_DISC_DATE_FORMAT = "yyyy-MM-dd";

    public static final String AND = "&";
    public static final String REG_EXPRESSION_OR = "\\|";
    public static final String REG_EXPRESSION_AND_OR = "&|\\|";
    public static final String OR = "|";
    public static final String PERCENTAGE = "%";

    public static final String ITICKET_FOLDER = "TMT iTicket";
    public static final String UPDATE_APK_FILENAME = "iTicket.apk";


    public static final int PROCESS_ID_QUERY_UPDATES = 1;
    public static final int PROCESS_ID_EXECUTE_UPDATES = 2;

    public static final int PROCESS_ID_UPLOAD_GPS_LOGS = 3;
    public static final int PROCESS_ID_DOWNLOAD_APK = 4;
    public static final int PROCESS_ID_DOWNLOAD_DISTRICTS = 5;
    public static final int PROCESS_ID_REGISTER_DEVICE = 6;

    public static final int PROCESS_ID_GET_FIELD_DEVICE = 7;

    //public static final int PROCESS_ID_DOWNLOAD_OFFICERS = 8;
    public static final int PROCESS_ID_DOWNLOAD_RSA_DESERIALISER_LIC = 9;
    public static final int PROCESS_ID_GET_DEVICE_CONFIG_ITEM = 10;

    public static final int PROCESS_ID_DOWNLOAD_SYSTEM_FUNCTIONS = 11;

    //activity requestcodes must be 1000 or higher to ensure they clash with application request codes
    public static final int LOGIN_REQUEST_CODE = 12;
    public static final int LOCATION_REQUEST_CODE = 13;
    public static final int DISTRICT_REQUEST_CODE = 14;
    public static final int DATA_SYNC_REQUEST_CODE = 15;
    public static final int INSTALL_APP_REQUEST_CODE = 16;
    public static final int SCAN_REQUEST_CODE = 18;
    public static final int PROCESS_ID_MOBILE_DEVICE_APPLICATION = 19;
    public static final int SIGNATURE_REQUEST_CODE = 20;
    public static final int ACTIVTIY_FAILED_WITH_INVALID_LOGIN_CREDENTIALS = 21;
    public static final int PROCESS_ID_UPLOAD_USER_ACTIVITY_LOG = 22;
    public static final int CONFIG_REQUEST_CODE = 23;
    public static final int PROCESS_ID_DOWNLOAD_USERS = 24;
    public static final int PROCESS_ID_DOWNLOAD_DISTRICT = 25;
    public static final int PROCESS_ID_AUTHENTICATE = 26;

    public static final String SYNCHRONISATION_REQUIRED = "SYNCHRONISATION_REQUIRED";
    public static final String FINISHED_MESSAGE = "FINISHED";
    public static final String FAILED_MESSAGE = "FAILED";
    public static final String SIGNATURE = "Signature";
    public static final String VALIDATE_LOCALLY = "VALIDATE_LOCALLY";
    public static final String KAPSCH_ANDROID_PACKAGE_NAME = "za.co.kapsch";
}
