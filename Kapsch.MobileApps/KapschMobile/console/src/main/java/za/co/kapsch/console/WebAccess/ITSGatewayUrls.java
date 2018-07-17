package za.co.kapsch.console.WebAccess;

import za.co.kapsch.console.Models.ConfigItemModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;

/**
 * Created by CSenekal on 2017/02/02.
 */
public class ITSGatewayUrls {

    private final static String ProcessingGatewayUrl(){

        return EndPointConfigModel.getInstance().getITSGateway();
    }

    private final static String OfficersUrl = "/api/Ticket/Officer";
    private final static String UpdateApkUrl = "/api/Ticket/Apk?deviceId=%s&androidPackageName=%s&version=%d";

    private static String combineUrl(String baseUrl, String apiUrl){
        return String.format(String.format("http://%s%s", baseUrl, "%s"), apiUrl);
    }

    public static String officersUrl(){
        return String.format(combineUrl(ProcessingGatewayUrl(), OfficersUrl));
    }

    public static String getUpdateApkUrl(String deviceId, String androidPackageName, int versionCode){
        return String.format(combineUrl(ProcessingGatewayUrl(), UpdateApkUrl), deviceId, androidPackageName, versionCode);
    }

//    private final static String FieldDeviceUrl = "/api/iTicket/FieldDevice?deviceId=%s&districtId=%d";
//    private final static String DistrictsUrl = "/api/iTicket/Districts";
//    private final static String DeviceConfigurationUrl = "/api/iTicket/FieldDeviceConfiguration?deviceId=%s";
//    private final static String OfficersUrl = "/api/iTicket/Officers?districtId=%s";
//

//
//    public static String getFieldDeviceUrl(String deviceId, long districtId){
//        return String.format(combineUrl(ProcessingGatewayUrl(), FieldDeviceUrl), deviceId, districtId);
//    }
//
//    public static String getDistrictsUrl(){
//        return String.format(combineUrl(ProcessingGatewayUrl(), DistrictsUrl));
//    }
//
//    public static String getDeviceConfigurationUrl(String deviceId){
//        return String.format(combineUrl(ProcessingGatewayUrl(), DeviceConfigurationUrl), deviceId);
//    }
//
//    public static String getOfficersUrl(long districtCode){
//        return String.format(combineUrl(ProcessingGatewayUrl(), OfficersUrl), districtCode);
//    }
}
