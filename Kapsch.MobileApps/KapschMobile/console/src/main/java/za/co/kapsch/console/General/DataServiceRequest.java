package za.co.kapsch.console.General;

import android.app.Activity;

import com.google.gson.reflect.TypeToken;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.console.*;
import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.orm.DistrictRepository;
import za.co.kapsch.shared.*;
import za.co.kapsch.shared.Enums.FilterJoin;
import za.co.kapsch.shared.Enums.Operation;

import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.console.Models.ConfigItemModel;
import za.co.kapsch.shared.Models.CredentialModel;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.FilterModel;
import za.co.kapsch.console.Models.InsertResultModel;
import za.co.kapsch.console.Models.MobileDeviceApplicationModel;
import za.co.kapsch.console.Models.MobileDeviceLocationModel;
import za.co.kapsch.shared.Models.MobileDeviceModel;
import za.co.kapsch.shared.Models.PaginationListModel;

import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Models.SystemFunctionModel;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.console.WebAccess.CoreGatewayUrls;
import za.co.kapsch.console.WebAccess.ITSGatewayUrls;
import za.co.kapsch.shared.WebAccess.DataService;
import za.co.kapsch.shared.MessageManager;

/**
 * Created by CSenekal on 2017/01/21.
 */
public class DataServiceRequest {

    //This is the default district - mobile device is no longer associated with a specific district
    private static long DEVICE_DISTRICT_ID = 0;

    public static void registerDevice(IAsyncProcessCallBack caller, Activity activity) {

        MobileDeviceModel mobileDevice = new MobileDeviceModel();
        mobileDevice.setDeviceID(Utilities.getDeviceId());
        mobileDevice.setDistrictID(DEVICE_DISTRICT_ID);

        String serialNumber = "00000000000000000"; //Utilities.getDeviceSerialNumber();
        if (serialNumber == null){
            MessageManager.showMessage(App.getContext().getResources().getString(za.co.kapsch.console.R.string.failed_to_obtain_serial_number), za.co.kapsch.shared.Enums.ErrorSeverity.High);
            return;
        }

        mobileDevice.setSerialNumber(serialNumber);

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.registerDeviceUrl(),
                mobileDevice,
                null,
                Constants.PROCESS_ID_REGISTER_DEVICE,
                new TypeToken<MobileDeviceModel>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void districtsRequest(IAsyncProcessCallBack caller, Activity activity) {

        //For the device this default filter list is always used.
        List<FilterModel> filterList = new ArrayList<>();
//        FilterModel filter = new FilterModel();
//        filter.setPropertyName("ID");
//        filter.setOperation(Operation.Equals.getNumValue());
//        filter.setValue(1);
//        filterList.add(filter);

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.getDistrictsUrl(0, true, "Town", 1, 1000000),
                filterList,
                null,
                Constants.PROCESS_ID_DOWNLOAD_DISTRICTS,
                new TypeToken<PaginationListModel<DistrictModel>>(){}.getType(), //new TypeToken<OpusResponseModel<OpusAuthenticationResponseModel>>() {}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void districtRequest(IAsyncProcessCallBack caller, Activity activity, long districtID) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.getDistrictUrl(districtID),
                null,
                null,
                Constants.PROCESS_ID_DOWNLOAD_DISTRICT,
                new TypeToken<DistrictModel>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void systemFunctionRequest(IAsyncProcessCallBack caller, Activity activity) {

        //For the device this default filter list is always used.
        List<FilterModel> filterList = new ArrayList<>();
//        FilterModel filter = new FilterModel();
//        filter.setPropertyName("ID");
//        filter.setOperation(Operation.Equals.getNumValue());
//        filter.setValue(1);
//        filterList.add(filter);

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.getSystemFunctionUrl(FilterJoin.And.getNumValue(), true, "ID", 1, 1000000),
                filterList,
                null,
                Constants.PROCESS_ID_DOWNLOAD_SYSTEM_FUNCTIONS,
                new TypeToken<PaginationListModel<SystemFunctionModel>>(){}.getType(), //new TypeToken<OpusResponseModel<OpusAuthenticationResponseModel>>() {}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void mobileDeviceApplicationListRequest(IAsyncProcessCallBack caller, Activity activity) {

        //For the device this default filter list is always used.
        List<FilterModel> filterList = new ArrayList<>();
//        FilterModel filter = new FilterModel();
//        filter.setPropertyName("ID");
//        filter.setOperation(Operation.Equals.getNumValue());
//        filter.setValue(1);
//        filterList.add(filter);

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.getMobileDeviceApplicationListUrl(FilterJoin.And.getNumValue(), true, "ID", 1, 1000000),
                filterList,
                null,
                Constants.PROCESS_ID_MOBILE_DEVICE_APPLICATION,
                new TypeToken<PaginationListModel<MobileDeviceApplicationModel>>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void configItemRequest(IAsyncProcessCallBack caller, Activity activity) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                CoreGatewayUrls.mobileDeviceItemUrl(Utilities.getDeviceId()),
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

//    public static void officersRequest(IAsyncProcessCallBack caller, Activity activity) {
//
//        DataService dataService = new DataService(caller, activity);
//        dataService.request(
//                Constants.REQUEST_METHOD_GET,
//                ITSGatewayUrls.officersUrl(DistrictRepository.getDistrictID()),
//                null,
//                null,
//                Constants.PROCESS_ID_DOWNLOAD_OFFICERS,
//                new TypeToken<List<UserModel>>(){}.getType(),
//                true,
//                null,
//                false,
//                true);
//    }

    public static void usersRequest(IAsyncProcessCallBack caller, Activity activity) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                ITSGatewayUrls.officersUrl(),
                null,
                null,
                Constants.PROCESS_ID_DOWNLOAD_USERS,
                new TypeToken<List<UserModel>>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void authenticateRequest(IAsyncProcessCallBack caller, Activity activity, String username, String password) {

        CredentialModel credential = new CredentialModel(username, password);

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.getAuthenticationUrl(),
                credential,
                null,
                Constants.PROCESS_ID_AUTHENTICATE,
                new TypeToken<SessionModel>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void apkRequest(IAsyncProcessCallBack caller, Activity activity, String androidPackageName ,int versionCode) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                //ITSGatewayUrls.getUpdateApkUrl(Utilities.getDeviceId(), androidPackageName, versionCode),
                CoreGatewayUrls.getMobileDeviceApplicationUrl(Utilities.getDeviceId(), androidPackageName, versionCode),
                null,
                null,
                Constants.PROCESS_ID_DOWNLOAD_APK,
                new TypeToken<String>(){}.getType(),
                true,
                androidPackageName,
                false,
                true,
                false,
                false);
    }

    public static void dlcSerializerRsaLicRequest(IAsyncProcessCallBack caller, Activity activity) {

//        DataService dataService = new DataService(caller, activity);
//        dataService.request(
//                Constants.REQUEST_METHOD_GET,
//                GatewayUrls.getDlcSerializerRsaLic(Utilities.getDeviceId()),
//                null,
//                null,
//                Constants.PROCESS_ID_DOWNLOAD_RSA_DESERIALISER_LIC,
//                String.class,
//                true,
//                null,
//                false,
//                true);
    }

    public static void gpsLogUploadRequest(IAsyncProcessCallBack caller, Activity activity, List<MobileDeviceLocationModel> mobileDeviceLocationList ) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.mobileDeviceLocationUrl(Utilities.getDeviceId()) ,
                mobileDeviceLocationList,
                null,
                Constants.PROCESS_ID_UPLOAD_GPS_LOGS,
                new TypeToken<List<InsertResultModel>>() {}.getType(),
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

//    public static void deviceConfiguration(IAsyncProcessCallBack caller, Activity activity) {

//        DataService dataService = new DataService(caller, activity);
//        dataService.request(
//                Constants.REQUEST_METHOD_GET,
//                GatewayUrls.getDeviceConfigurationUrl(Utilities.getDeviceId()),
//                null,
//                null,
//                Constants.PROCESS_ID_GET_DEVICE_CONFIGURATION,
//                new TypeToken<List<ConfigItemModel>>(){}.getType(),
//                true,
//                null,
//                false,
//                true);
//    }

    public static void backendFieldDevice(IAsyncProcessCallBack caller, Activity activity) {

//        DataService dataService = new DataService(caller, activity);
//        dataService.request(
//                Constants.REQUEST_METHOD_GET,
//                GatewayUrls.getFieldDeviceUrl(Utilities.getDeviceId(), getDistrictCode()),
//                null,
//                null,
//                Constants.PROCESS_ID_GET_FIELD_DEVICE,
//                new TypeToken<FieldDeviceModel>(){}.getType(),
//                true,
//                null,
//                false,
//                true);
    }
}
