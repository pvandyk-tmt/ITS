package za.co.kapsch.ipayment.General;

import android.app.Activity;

import com.google.gson.reflect.TypeToken;

import java.util.List;

import za.co.kapsch.ipayment.Models.ConfigItemModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Enums.RegisterStatus;
import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.shared.Models.FineModel;
import za.co.kapsch.ipayment.Models.TransactionModel;
import za.co.kapsch.ipayment.WebAccess.CoreGatewayUrls;
import za.co.kapsch.ipayment.WebAccess.ITSGatewayUrls;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.WebAccess.DataService;

/**
 * Created by csenekal on 2017/08/23.
 */
public class DataServiceRequest {

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


    public static void finesRequest(IAsyncProcessCallBack caller, Activity activity, SearchFinesCriteriaType criteriaType, String searchValue) {

        int processId = -1;

        switch (criteriaType) {
            case ID:
                processId = Constants.PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_ID;
                break;
            case RefNumber:
                processId = Constants.PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_REF_NUMBER;
                break;
            case TransactionToken:
                processId = Constants.PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_TOKEN;
                break;
            case VLN:
                processId = Constants.PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_VLN;
                break;
        }

        if (processId == -1) {
            MessageManager.showMessage("finesRequest process id does not have valid value.", ErrorSeverity.High);
            return;
        }

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                ITSGatewayUrls.getFineUrl(criteriaType, searchValue, false, RegisterStatus.Open),
                null,
                null,
                processId,
                new TypeToken<List<FineModel>>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void transactionRegisterRequest(IAsyncProcessCallBack caller, Activity activity, TransactionModel transaction) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.getTransactionUrl(),
                transaction,
                null,
                Constants.PROCESS_ID_REGISTER_TRANSACTION,
                new TypeToken<String>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

//    public static void transactionSettleRequest(IAsyncProcessCallBack caller, Activity activity, TransactionModel transaction) {
//
//        DataService dataService = new DataService(caller, activity);
//        dataService.request(
//                Constants.REQUEST_METHOD_PUT,
//                CoreGatewayUrls.getTransactionUrl(),
//                transaction,
//                null,
//                Constants.PROCESS_ID_SETTLE_TRANSACTION,
//                new TypeToken<String>(){}.getType(),
//                true,
//                null,
//                false,
//                true);
//    }

    public static void transactionDeleteRequest(IAsyncProcessCallBack caller, Activity activity, String receipt) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_DELETE,
                CoreGatewayUrls.getTransactionDeleteUrl(receipt),
                null,
                null,
                Constants.PROCESS_ID_DELETE_TRANSACTION,
                new TypeToken<String>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void queryTransactionRequest(IAsyncProcessCallBack caller, Activity activity, String receiptOrTransactionTokenNumber) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                CoreGatewayUrls.getQueryTransactionUrl(receiptOrTransactionTokenNumber),
                null,
                null,
                Constants.PROCESS_ID_QUERY_TRANSACTION,
                new TypeToken<TransactionModel>(){}.getType(),
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
}
