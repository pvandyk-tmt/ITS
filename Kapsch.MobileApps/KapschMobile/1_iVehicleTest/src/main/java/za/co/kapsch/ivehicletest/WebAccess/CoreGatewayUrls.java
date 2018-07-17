package za.co.kapsch.ivehicletest.WebAccess;

import za.co.kapsch.shared.Models.EndPointConfigModel;

/**
 * Created by csenekal on 2016-09-07.
 */
public class CoreGatewayUrls {

    private final static String AuthenticationUrl = "/api/Authentication";
    private final static String MobileDeviceItemUrl = "/api/MobileDevice/MobileDeviceItem?deviceID=%s";
    private final static String UserActivityLogUrl = "/api/MobileDevice/UserMobileDeviceActivity";

    private static String combineUrl(String baseUrl, String apiUrl){
        return String.format(String.format("http://%s%s", baseUrl, "%s"), apiUrl);
    }

    public static String getSessionUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), AuthenticationUrl));
    }

    public static String getMobileDeviceItemUrl(String deviceID){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), MobileDeviceItemUrl), deviceID);
    }

    public static String getUserActivityLogUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), UserActivityLogUrl));
    }
}
