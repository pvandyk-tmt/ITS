package za.co.kapsch.iticket.WebAccess;

import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;

/**
 * Created by csenekal on 2016-09-07.
 */
public class CoreGatewayUrls {

    private final static String AuthenticationUrl = "/api/Authentication";
    private final static String PersonInfoUrl = "/api/Search/PersonInfo?IdNumber=%s";
    private final static String GeoGpsInfoUrl = "/api/Configuration/GeoLocation?latitude=%s&longitude=%s";
    private final static String GeoAddressInfoUrl = "/api/Configuration/GeoLocation?partialAddress=%s";
    private final static String MobileDeviceItemUrl = "/api/MobileDevice/MobileDeviceItem?deviceID=%s";
    private final static String CourtDetailUrl = "/api/Configuration/Court/PaginatedList?filterJoin=%d&asc=%s&orderPropertyName=%s&pageIndex=%d&pageSize=%d";
    private final static String PublicHolidayUrl = "/api/Configuration/PublicHoliday/PaginatedList?filterJoin=%d&asc=%s&orderPropertyName=%s&pageIndex=%d&pageSize=%d";
    private final static String InfringementLocationtUrl = "/api/Configuration/InfringementLocation/PaginatedList?filterJoin=%d&asc=%s&orderPropertyName=%s&pageIndex=%d&pageSize=%d";
    private final static String UserActivityLogUrl = "/api/MobileDevice/UserMobileDeviceActivity";
    private final static String IdentificationTypeUrl = "/api/Configuration/IdentificationType";
    private final static String CountryUrl = "/api/Configuration/Country/PaginatedList?filterJoin=%d&asc=%s&orderPropertyName=%s&pageIndex=%d&pageSize=%d";

    private static String combineUrl(String baseUrl, String apiUrl){
        return String.format(String.format("http://%s%s", baseUrl, "%s"), apiUrl);
    }

    public static String getSessionUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), AuthenticationUrl));
    }

    public static String personInfoUrl(String idNumber){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), PersonInfoUrl), idNumber);
    }

    public static String geoInfoUrl(String partialAddress){
            return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), GeoAddressInfoUrl), partialAddress);
    }

    public static String geoInfoUrl(String latitude, String longitude){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), GeoGpsInfoUrl), latitude, longitude);
    }

    public static String mobileDeviceItemUrl(String deviceID){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), MobileDeviceItemUrl), deviceID);
    }

    public static String getCourtDetailUrl(int filterJoin, boolean asc, String orderPropertyName, int pageIndex, int pageSize){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), CourtDetailUrl), filterJoin, asc ? "true" : "false", orderPropertyName, pageIndex, pageSize);
    }

    public static String getPublicHolidayUrl(int filterJoin, boolean asc, String orderPropertyName, int pageIndex, int pageSize) {
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), PublicHolidayUrl), filterJoin, asc ? "true" : "false", orderPropertyName, pageIndex, pageSize);
    }

    public static String getInfringementLocationUrl(int filterJoin, boolean asc, String orderPropertyName, int pageIndex, int pageSize) {
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), InfringementLocationtUrl), filterJoin, asc ? "true" : "false", orderPropertyName, pageIndex, pageSize);
    }

    public static String getUserActivityLogUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), UserActivityLogUrl));
    }

    public static String getIdentificationTypeUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), IdentificationTypeUrl));
    }

    public static String getCountryUrl(int filterJoin, boolean asc, String orderPropertyName, int pageIndex, int pageSize){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), CountryUrl), filterJoin, asc ? "true" : "false", orderPropertyName, pageIndex, pageSize);
    }
}
