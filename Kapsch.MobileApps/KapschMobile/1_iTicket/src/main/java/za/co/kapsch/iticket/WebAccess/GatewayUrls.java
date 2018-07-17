package za.co.kapsch.iticket.WebAccess;

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Enums.TicketType;
import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;

/**
 * Created by CSenekal on 2017/02/02.
 */
public class GatewayUrls {

    private final static String ProcessingGatewayUrl(){
         return EndPointConfigModel.getEnforcementGateway();
   }

    //TODO CHANGE ENFORCEMENT WEB API CALLS TO LOOK LIKE THESE CALLS THEY WILL CURRENTLY FAIL.
    private final static String AccountAuthenticateUrl = "/api/Account/Authenticate";
    private final static String RegisterFieldDeviceUrl = "/api/iTicket/FieldDevice?deviceId=%s&technovolveDeviceId=%s&serialNumber=%s&districtId=%d";
    private final static String FieldDeviceUrl = "/api/iTicket/FieldDevice?deviceId=%s&districtId=%d";
    private final static String DistrictsUrl = "/api/iTicket/Districts";
    private final static String CourtsUrl = "/api/iTicket/Courts?districtId=%s";
    private final static String CourtsInfoUrl = "/api/iTicket/CourtsInfo?districtId=%s";
    private final static String VehiclesInfoUrl = "/api/iTicket/VehiclesInfo";
    private final static String OffenceCodeUrl = "/api/iTicket/OffenceCodes?districtId=%d";
    //private final static String UploadfileUrl = "/api/iTicket/PutFile?filename=%s&deviceId=%s";
    private final static String EvidenceFileUrl = "/api/iTicket/EvidenceFile?noticeNumber=%s&evidenceType=%d&mimeType=%s";
    private final static String NoticeNumbersUrl = "/api/iTicket/NoticeNumbers?districtId=%d&numberTickets=%d&ticketType=%d&deviceId=%s";
    private final static String TicketNumberRangeUrl = "/api/iTicket/TicketNumberRange?districtCode=%d&numberTickets=%d&InfringementSection=%d&deviceId=%s";
    private final static String UpdateFieldDeviceGpsUrl = "/api/iTicket/Gps";
    private final static String Section56Url = "/api/iTicket/Section56";
    private final static String Section341Url = "/api/iTicket/Section341?offenceSet=%s";
    private final static String UpdateApkUrl = "/api/iTicket/Apk?deviceId=%s&versionCode=%d";
    private final static String DlcSerializerRsaLicUrl = "/api/iTicket/DlcSerializerRsaLic?deviceId=%s";
    private final static String GeoAddressInfoUrl = "/api/Search/GeoInfo?partialAddress=%s";
    private final static String TicketCompleteUrl = "/api/iTicket/TicketComplete?ticketType=%d&noticeNumber=%s";
    private final static String DeviceConfigurationUrl = "/api/iTicket/FieldDeviceConfiguration?deviceId=%s";
    private final static String VosiUrl = "/api/iTicket/VOSI?licenceNumber=%s&deviceId=%s";
    private final static String EnatisVehicleUrl = "/api/iTicket/EnatisVehicle?licenceNumber=%s";
    private final static String OfficersUrl = "/api/iTicket/Officers?districtId=%s";
    private final static String PublicHolidaysUrl = "/api/iTicket/PublicHolidays";

    public static String geoInfoUrl(String partialAddress){
        return String.format(combineUrl(ProcessingGatewayUrl(), GeoAddressInfoUrl), partialAddress);
    }

    private static String combineUrl(String baseUrl, String apiUrl){
        return String.format(String.format("http://%s%s", baseUrl, "%s"), apiUrl);
    }

    public static String getAccountAuthenticateUrl(){
        return String.format(combineUrl(ProcessingGatewayUrl(), AccountAuthenticateUrl));
    }

    public static String registerDeviceUrl(String deviceId, String technovolveDeviceId, String serialNumber, long districtId){
        return String.format(combineUrl(ProcessingGatewayUrl(), RegisterFieldDeviceUrl), deviceId, technovolveDeviceId, serialNumber, districtId);
    }

    public static String getFieldDeviceUrl(String deviceId, long districtId){
        return String.format(combineUrl(ProcessingGatewayUrl(), FieldDeviceUrl), deviceId, districtId);
    }

    public static String getEvidenceFileUrl(String noticeNumber, int evidenceType, String mimeType){
        return String.format(combineUrl(ProcessingGatewayUrl(), EvidenceFileUrl), noticeNumber, evidenceType, mimeType);
    }

    public static String getDistrictsUrl(){
        return String.format(combineUrl(ProcessingGatewayUrl(), DistrictsUrl));
    }

    public static String getCourtsUrl(long districtCode){
        return String.format(combineUrl(ProcessingGatewayUrl(), CourtsUrl), districtCode);
    }

    public static String getCourtsInfoUrl(long districtCode){
        return String.format(combineUrl(ProcessingGatewayUrl(), CourtsInfoUrl), districtCode);
    }

    public static String getVehiclesInfoUrl(){
        return String.format(combineUrl(ProcessingGatewayUrl(), VehiclesInfoUrl));
    }

    public static String getOffenceCodesUrl(long districtCode){
        return String.format(combineUrl(ProcessingGatewayUrl(), OffenceCodeUrl), districtCode);
    }

    public static String getSection56Url(){
        return String.format(combineUrl(ProcessingGatewayUrl(), Section56Url));
    }

    public static String getSection341Url(int offenceSet){
        return String.format(combineUrl(ProcessingGatewayUrl(), Section341Url), offenceSet);
    }

    public static String getTicketNumberURL(long districtCode, int numberOfTickets, DocumentType documentType, String deviceId){
        return String.format(combineUrl(ProcessingGatewayUrl(), NoticeNumbersUrl), districtCode, numberOfTickets, documentType, deviceId);
    }

    public static String getTicketNumberRangeURL(long districtCode, int numberOfTickets, TicketType tickeType, String deviceId){
        return String.format(combineUrl(ProcessingGatewayUrl(), TicketNumberRangeUrl), districtCode, numberOfTickets, tickeType.getNumValue(), deviceId);
    }

    public static String getUpdateFieldDeviceGpsUrl() {
        return String.format(combineUrl(ProcessingGatewayUrl(), UpdateFieldDeviceGpsUrl));
    }

//    public static String getUploadfileUrl(String filename, String deviceId){
//        return String.format(combineUrl(ProcessingGatewayUrl(), UploadfileUrl), filename, deviceId);
//    }

    public static String getUpdateApkUrl(String deviceId, int versionCode){
        return String.format(combineUrl(ProcessingGatewayUrl(), UpdateApkUrl), deviceId, versionCode);
    }

    public static String getDlcSerializerRsaLic(String deviceId){
        return String.format(combineUrl(ProcessingGatewayUrl(), DlcSerializerRsaLicUrl), deviceId);
    }

    public static String getTicketCompleteUrl(TicketType tickeType, String ticketNumber){
        return String.format(combineUrl(ProcessingGatewayUrl(), TicketCompleteUrl), tickeType.getNumValue(), ticketNumber);
    }

    public static String getDeviceConfigurationUrl(String deviceId){
        return String.format(combineUrl(ProcessingGatewayUrl(), DeviceConfigurationUrl), deviceId);
    }

    public static String getVosiUrl(String licenceNumber, String deviceId){
        return String.format(combineUrl(ProcessingGatewayUrl(), VosiUrl), licenceNumber, deviceId);
    }

    public static String getEnatisVehicleUrl(String licenceNumber){
        return String.format(combineUrl(ProcessingGatewayUrl(), EnatisVehicleUrl), licenceNumber);
    }

    public static String getOfficersUrl(long districtCode){
        return String.format(combineUrl(ProcessingGatewayUrl(), OfficersUrl), districtCode);
    }

    public static String getPublicHolidaysUrl(){
        return String.format(combineUrl(ProcessingGatewayUrl(), PublicHolidaysUrl));
    }
}
