package za.co.kapsch.shared.WebAccess;

import za.co.kapsch.shared.Models.EndPointConfigModel;

/**
 * Created by csenekal on 2016-09-07.
 */
public class CoreGatewayUrls {

    private final static String AuthenticationUrl = "/api/Authentication";

    private static String combineUrl(String baseUrl, String apiUrl){
        return String.format(String.format("http://%s%s", baseUrl, "%s"), apiUrl);
    }

    public static String getSessionUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), AuthenticationUrl));
    }
}
