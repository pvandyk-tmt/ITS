package za.co.kapsch.ipayment.WebAccess;

import za.co.kapsch.shared.Enums.RegisterStatus;
import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.ipayment.Models.ConfigItemModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/02/02.
 */
public class ITSGatewayUrls {

//    private final static String ProcessingGatewayUrl(){
//
//        return ConfigItemModel.getITSGateway();
//    }

    private final static String FineUrl = "/api/Fine?searchCriteria=%d&searchValue=%s&includeAccount=%s&status=%d&onlyImageEvidence=%s&onlyPayable=%s";

    private static String combineUrl(String baseUrl, String apiUrl){
        return String.format(String.format("http://%s%s", baseUrl, "%s"), apiUrl);
    }

    public static String getFineUrl(SearchFinesCriteriaType criteriaType, String searchValue, boolean includeAccount, RegisterStatus registerStatus){
        try{
            return String.format(
                    combineUrl(EndPointConfigModel.getInstance().getITSGateway(), FineUrl),
                    criteriaType.getCode(),
                    searchValue,
                    includeAccount == true ? "true" : "false",
                    registerStatus.getCode(),
                    "false",
                    "true");
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ITSGatewayUrls::getFineUrl()"), ErrorSeverity.High);
            return null;
        }
    }
}
