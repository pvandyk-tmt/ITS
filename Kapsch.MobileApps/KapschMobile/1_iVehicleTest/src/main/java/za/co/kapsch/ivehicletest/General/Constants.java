package za.co.kapsch.ivehicletest.General;

/**
 * Created by csenekal on 2017/11/27.
 */

public class Constants {

    public static final int OK = -1;
    public static final int YES = -1;
    public static final int NO = -2;

    public static final int WIZARD_CANCELLATION_REQUEST_CODE = 1;
    public static final int PROCESS_ID_GET_DEVICE_CONFIG_ITEM = 2;
    public static final int PROCESS_ID_UPLOAD_USER_ACTIVITY_LOG = 3;
    public static final int PROCESS_ID_DOWNLOAD_VEHICLE_LOOKUP = 4;
    public static final int PROCESS_ID_GET_VEHICLE_INSPECTION = 5;
    public static final int PROCESS_ID_UPLOAD_VEHICLE_INSPECTION_RESULTS = 6;
    public static final int DATA_SYNC_REQUEST_CODE = 7;
    public static final int PROCESS_ID_ASYNC_PROCESS_PRINT = 8;
    public static final int PROCESS_ID_UPLOAD_EVIDENCE = 9;

    public static final int REQUEST_TAKE_PHOTO = 10;
    public static final int EVIDENCE_BITMAP_MAX_SIZE = 1000; //width or height
    public static final int MAX_EVIDENCE_IMAGES = 3;
    public static final int TERMINATE_INSPECTION_WEIGHT = 200;

    public static final String EMPTY_STRING = "";
    public static final String INDEX_TEXT = "index";
    public static final String WIZARD_START_PAGE_INDEX = "WizardStartPageIndex";

    public static final String VEHICLE_INSPECTION_FRAGMENT_QUESTION = "VehicleInspectionFragmentQuestion";
    //public static final String VEHICLE_INSPECTION_PARAMATERS_LIST = "VehicleInspectionParametersList";
    public static final String VEHICLE_INSPECTION_QUESTION_LIST = "VehicleInspectionQuestionList";
    public static final String VEHICLE_INSPECTION_QUERY = "VehicleInspectionQuery";
    public static final String LOAD_WIZARDPAGE_DYNAMICALLY = "LoadWizardPageDynamically";
    public static final String BOOKING_REFERENCE_NUMBER = "BookingReferenceNumber";
    public static final String HAS_FINAL_OPRATION = "HasFinalOperation";
    public static final String FINAL_OPERATION_TEXT = "FinalOperationText";

    public static final String SYNCHRONISATION_REQUIRED = "SYNCHRONISATION_REQUIRED";
    public static final String FINISHED_MESSAGE = "FINISHED";
    public static final String FAILED_MESSAGE = "FAILED";
    public static final String VERSION = "VERSION";

    public static final String REQUEST_METHOD_GET = "GET";
    public static final String REQUEST_METHOD_POST = "POST";
    public static final String ADDITIONAL_WIZARD_INFORMATION = "AdditionalWizardInformation";
    public static final String TEMP_PICTURE_EVIDENCE_FILENAME = "tempPictureEvidence.jpeg";
    public static final String MIME_TYPE_JPEG = "image/jpeg";

}
