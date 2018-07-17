package za.co.kapsch.ipayment.WebAccess;

import za.co.kapsch.ipayment.Models.ConfigItemModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;

/**
 * Created by CSenekal on 2017/09/08.
 */
public class CoreGatewayUrls {

//    private final static String ProcessingGatewayUrl(){
//
//        return ConfigItemModel.getCoreGateway();
//    }

    private final static String TransactionUrl = "/api/Payment/Transaction";
    private final static String TransactionDeleteUrl = "/api/Payment/Transaction?transactionTokenOrReceipt=%s";
    private final static String PaymentTransactionSignalRHubUrl = "/signalr";
    private final static String queryTransactionUrl = "/api/Payment/Transaction?transactionTokenOrReceipt=%s";
    private final static String MobileDeviceItemUrl = "/api/MobileDevice/MobileDeviceItem?deviceID=%s";
    private final static String UserActivityLogUrl = "/api/MobileDevice/UserMobileDeviceActivity";

    private static String combineUrl(String baseUrl, String apiUrl){
        return String.format(String.format("http://%s%s", baseUrl, "%s"), apiUrl);
    }

    public static String mobileDeviceItemUrl(String deviceID){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), MobileDeviceItemUrl), deviceID);
    }

    public static String getTransactionUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), TransactionUrl));
    }

    public static String getTransactionDeleteUrl(String receipt){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), TransactionDeleteUrl), receipt);
    }

    public static String getPaymentTransactionSignalRHubUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), PaymentTransactionSignalRHubUrl));
    }

    public static String getQueryTransactionUrl(String receiptOrTransactionTokenNumber){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), queryTransactionUrl), receiptOrTransactionTokenNumber);
    }

    public static String getUserActivityLogUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getCoreGateway(), UserActivityLogUrl));
    }
}
