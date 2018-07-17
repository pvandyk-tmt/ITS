package za.co.kapsch.ipayment.General;

/**
 * Created by csenekal on 2017/08/23.
 */
public class Constants {

    public static final String EMPTY_STRING = "";

    public static final String REQUEST_METHOD_GET = "GET";
    public static final String REQUEST_METHOD_POST = "POST";
    public static final String REQUEST_METHOD_PUT = "PUT";
    public static final String REQUEST_METHOD_DELETE = "DELETE";

    public static final int PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_ID = 1;
    public static final int PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_VLN = 2;
    public static final int PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_REF_NUMBER= 3;
    public static final int PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_TOKEN = 4;
    public static final int PROCESS_ID_REGISTER_TRANSACTION = 5;
    public static final int PROCESS_ID_QUERY_TRANSACTION = 6;
    public static final int PROCESS_ID_SETTLE_TRANSACTION = 7;
    public static final int PROCESS_ID_ASYNC_PROCESS_PRINT = 8;
    public static final int PROCESS_ID_DELETE_TRANSACTION = 9;
    public static final int PROCESS_ID_GET_DEVICE_CONFIG_ITEM = 11;
    public static final int PROCESS_ID_UPLOAD_USER_ACTIVITY_LOG = 12;


    public static final String TRANSACTION_CONFIRMATION_ACTION = "TransactionConfirmationAction";
    public static final String TRANSACTION_TOKEN = "TransactionToken";
    public static final String TRANSACTION_STATUS = "TransactionStatus";
    public static final String TRANSACTION_AMOUNT = "TransactionAmount";
    public static final String BROADCAST_SOURCE = "BroadcastSource";

    public static final String SEARCH_CRITERIA = "SearchCriteria";
    public static final String REF_NUMBER_FINE_LIST = "RefNumberFineList";
    public static final String ID_NUMBER_FINE_LIST = "IdNumberFineList";
    public static final String VEHICLE_FINE_LIST = "VehicleFineList";
    public static final String PAYMENT_METHOD = "PaymentMethod";
    public static final String FAILED_MESSAGE = "FAILED";
    public static final String VERSION = "VERSION";

    public static final String FINISHED_MESSAGE = "FINISHED";
}
