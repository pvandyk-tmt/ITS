package za.co.kapsch.ivehicletest.General;

import android.app.Activity;

import com.google.gson.reflect.TypeToken;

import java.util.List;

import za.co.kapsch.ivehicletest.Enums.InspectionEvidenceType;
import za.co.kapsch.ivehicletest.Models.ConfigItemModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionQueryModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionQuestionModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultsModel;
import za.co.kapsch.ivehicletest.Models.VehicleLookUpsModel;
import za.co.kapsch.ivehicletest.WebAccess.CoreGatewayUrls;
import za.co.kapsch.ivehicletest.WebAccess.EVRGatewayUrls;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.WebAccess.DataService;

/**
 * Created by CSenekal on 2017/01/21.
 */
public class DataServiceRequest {

    public static void configItemRequest(IAsyncProcessCallBack caller, Activity activity) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                CoreGatewayUrls.getMobileDeviceItemUrl(Utilities.getDeviceId()),
                null,
                null,
                Constants.PROCESS_ID_GET_DEVICE_CONFIG_ITEM,
                new TypeToken<List<ConfigItemModel>>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void vehicleLookupsRequest(IAsyncProcessCallBack caller, Activity activity, boolean make, boolean model, boolean modelNumber) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                EVRGatewayUrls.getVehicleLookupsUrl(make, model, modelNumber),
                null,
                null,
                Constants.PROCESS_ID_DOWNLOAD_VEHICLE_LOOKUP,
                new TypeToken<VehicleLookUpsModel>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void vehicleInspectionRequest(IAsyncProcessCallBack caller, Activity activity, String bookingReference) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                EVRGatewayUrls.getVehicleInspectionUrl(bookingReference),
                null,
                null,
                Constants.PROCESS_ID_GET_VEHICLE_INSPECTION,
                new TypeToken<VehicleInspectionQueryModel>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void vehicleInspectionResultUploadRequest(IAsyncProcessCallBack caller, Activity activity, VehicleInspectionResultsModel vehicleInspectionResults) {

//        VehicleInspectionResultsModel vehicleInspectionResults = new VehicleInspectionResultsModel();
//        vehicleInspectionResults.setCredentialID(SessionModel.getInstance().getUser().getCredentialID());
//        vehicleInspectionResults.setVehicleInspectionResultList(vehicleInspectionResultList);

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                EVRGatewayUrls.getVehicleInspectionResultUrl(),
                vehicleInspectionResults,
                null,
                Constants.PROCESS_ID_UPLOAD_VEHICLE_INSPECTION_RESULTS,
                new TypeToken<VehicleInspectionResultsModel>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void userActivityLogUploadRequest(IAsyncProcessCallBack caller, Activity activity, List<UserActivityLogModel> userActivityLogs) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.getUserActivityLogUrl(),
                userActivityLogs,
                null,
                Constants.PROCESS_ID_UPLOAD_USER_ACTIVITY_LOG,
                null,
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void evidenceUploadRequest(IAsyncProcessCallBack caller, Activity activity, long bookingID, long siteID, InspectionEvidenceType evidenceType, String mimeType, byte[] evidenceData){

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                EVRGatewayUrls.getEvidenceUrl(bookingID, siteID, evidenceType, mimeType),
                null,
                evidenceData,
                Constants.PROCESS_ID_UPLOAD_EVIDENCE,
                null,
                true,
                null,
                false,
                true,
                false,
                false);
    }

}
