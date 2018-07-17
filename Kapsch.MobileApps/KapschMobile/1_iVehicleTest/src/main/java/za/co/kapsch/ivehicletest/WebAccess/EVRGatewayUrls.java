package za.co.kapsch.ivehicletest.WebAccess;

import za.co.kapsch.ivehicletest.Enums.InspectionEvidenceType;
import za.co.kapsch.shared.Models.EndPointConfigModel;

/**
 * Created by csenekal on 2017/11/28.
 */

public class EVRGatewayUrls {

    private final static String VehicleLookupsUrl = "/api/Vehicle/GetVehicleLookUps?getMake=%s&getModel=%s&getModelNumber=%s";
    private final static String VehicleInspectionUrl = "/api/Vehicle/Question?bookingRef=%s";
    private final static String VehicleInspectionResultUrl = "/api/Vehicle/Result";
    private final static String EvidenceUrl = "/api/Vehicle/Evidence?bookingID=%d&siteID=%d&evidenceType=%d&mimeType=%s";

    private static String combineUrl(String baseUrl, String apiUrl){
        return String.format(String.format("http://%s%s", baseUrl, "%s"), apiUrl);
    }

    public static String getVehicleLookupsUrl(boolean make, boolean model, boolean modelNumber){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getEVRGateway(), VehicleLookupsUrl), make ? "true" : "false", model ? "true" : "false", modelNumber ? "true" : "false");
    }

    public static String getVehicleInspectionUrl(String bookingReference){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getEVRGateway(), VehicleInspectionUrl), bookingReference);
    }

    public static String getVehicleInspectionResultUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getEVRGateway(), VehicleInspectionResultUrl));
    }

    public static String getEvidenceUrl(long bookingID, long siteID, InspectionEvidenceType evidenceType, String mimeType){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getEVRGateway(), EvidenceUrl), bookingID, siteID, evidenceType.getCode(), mimeType);
    }
}
