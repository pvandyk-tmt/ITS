package za.co.kapsch.iticket.WebAccess;

import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;

/**
 * Created by CSenekal on 2017/05/31.
 */
public class DistanceOverTimeGatewayUrls {

    private final static String DistanceOverTimeSignalRHubUrl = "/SignalR/hubs/";
    private final static String ServiceRequestUrl = "/api/dot/section/ticketing/Post";

    private static String CombineUrl(String baseUrl, String apiUrl){
        return String.format(String.format("http://%s%s", baseUrl, "%s"), apiUrl);
    }

    public static String getServiceRequestUrl(){
        return String.format(CombineUrl(EndPointConfigModel.getDistanceOverTimeGateway(), ServiceRequestUrl));
    }

    public static String getDistanceOverTimeSignalRHubUrl(){
        return String.format(CombineUrl(EndPointConfigModel.getDistanceOverTimeGateway(), DistanceOverTimeSignalRHubUrl));
    }
}
