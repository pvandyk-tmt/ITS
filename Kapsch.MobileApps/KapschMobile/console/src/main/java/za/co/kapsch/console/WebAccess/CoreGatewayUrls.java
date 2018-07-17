package za.co.kapsch.console.WebAccess;

import za.co.kapsch.console.Models.ConfigItemModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;

/**
 * Created by csenekal on 2016-09-07.
 */
public class CoreGatewayUrls {

    private final static String AuthenticationUrl = "/api/Authentication";
    private final static String RegisterMobileDeviceUrl = "/api/MobileDevice";
    private final static String MobileDeviceLocationUrl = "/api/MobileDevice/MobileDeviceLocation?deviceID=%s";
    private final static String MobileDeviceItemUrl = "/api/MobileDevice/MobileDeviceItem?deviceID=%s";
    private final static String DistrictsUrl = "/api/Configuration/District/PaginatedList?filterJoin=%d&asc=%s&orderPropertyName=%s&pageIndex=%d&pageSize=%d";
    private final static String DistrictUrl = "/api/Configuration/District?id=%d";
    private final static String SystemFunctionUrl = "/api/Configuration/SystemFunction/PaginatedList?filterJoin=%d&asc=%s&orderPropertyName=%s&pageIndex=%d&pageSize=%d";
    private final static String MobileDeviceApplicationListUrl = "/api/MobileDevice/MobileDeviceApplication/PaginatedList?filterJoin=%d&asc=%s&orderPropertyName=%s&pageIndex=%d&pageSize=%d";
    private final static String MobileDeviceApplicationUrl = "/api/MobileDevice/MobileDeviceApplication?deviceID=%s&androidPackageName=%s&version=%d";
    private final static String UserActivityLogUrl = "/api/MobileDevice/UserMobileDeviceActivity";
    private final static String UsersUrl = "/api/User/District?districtID=%d";

    private static String combineUrl(String baseUrl, String apiUrl){
        return String.format(String.format("http://%s%s", baseUrl, "%s"), apiUrl);
    }

    public static String getAuthenticationUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), AuthenticationUrl));
    }

    public static String registerDeviceUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), RegisterMobileDeviceUrl));
    }

    public static String mobileDeviceLocationUrl(String deviceID){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), MobileDeviceLocationUrl), deviceID);
    }

    public static String mobileDeviceItemUrl(String deviceID){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), MobileDeviceItemUrl), deviceID);
    }

    public static String getDistrictsUrl(int filterJoin, boolean asc, String orderPropertyName, int pageIndex, int pageSize){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), DistrictsUrl), filterJoin, asc ? "true" : "false", orderPropertyName, pageIndex, pageSize);
    }

    public static String getDistrictUrl(long districtID){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), DistrictUrl), districtID);
    }

    public static String getSystemFunctionUrl(int filterJoin, boolean asc, String orderPropertyName, int pageIndex, int pageSize){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), SystemFunctionUrl), filterJoin, asc ? "true" : "false", orderPropertyName, pageIndex, pageSize);
    }

    public static String getMobileDeviceApplicationListUrl(int filterJoin, boolean asc, String orderPropertyName, int pageIndex, int pageSize){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), MobileDeviceApplicationListUrl), filterJoin, asc ? "true" : "false", orderPropertyName, pageIndex, pageSize);
    }

    public static String getMobileDeviceApplicationUrl(String deviceId, String androidPackageName, int versionCode){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), MobileDeviceApplicationUrl), deviceId, androidPackageName, versionCode);
    }

    public static String getUserActivityLogUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), UserActivityLogUrl));
    }

    public static String getUsersUrl(String districtID){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), UsersUrl), districtID);
    }
}
