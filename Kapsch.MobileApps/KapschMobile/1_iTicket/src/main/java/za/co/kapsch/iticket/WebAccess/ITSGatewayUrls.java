package za.co.kapsch.iticket.WebAccess;

import za.co.kapsch.iticket.Enums.EvidenceType;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Enums.RegisterStatus;
import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/07/28.
 */
public class ITSGatewayUrls {

    private final static String ChargeInfoUrl = "/api/Ticket/OffenceCode?districtID=%d";
    private final static String HandWrittenCaptureUrl = "/api/Ticket/HandWrittenCapture?offenceSetID=%d&performNatisRequest=%s";
    private final static String EvidenceUrl = "/api/Ticket/Evidence?noticeNumber=%s&districtID=%d&evidenceType=%d&mimeType=%s";
    private final static String ReferenceNumberUrl = "/api/Ticket/ReferenceNumber?districtID=%d&entityReferenceTypeID=%d&referenceDocumentTypeID=%d&deviceID=%s&numberCount=%d&requireTokens=%s";
    private final static String CourtInfoUrl = "/api/Ticket/CourtInfo?districtId=%s";
    private final static String VosiActionUrl = "/api/Ticket/VOSIAction";
    private final static String VosiActionCaptureUrl = "/api/Ticket/VosiActionCapture";
    private final static String FineUrl = "/api/Fine?searchCriteria=%d&searchValue=%s&includeAccount=%s&status=%d&onlyImageEvidence=%s&onlyPayable=%s";
    private final static String GetEvidenceUrl = "/api/Fine/Evidence?id=%d";

    private static String combineUrl(String baseUrl, String apiUrl){
        return String.format(String.format("http://%s%s", baseUrl, "%s"), apiUrl);
    }

    public static String getChargeInfoUrl(long districtID){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getITSGateway(), ChargeInfoUrl), districtID);
    }

    public static String getHandWrittenCaptureUrl(long offenceSet, boolean performNatisRequest){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getITSGateway(), HandWrittenCaptureUrl), offenceSet, performNatisRequest ? "true" : "false");
    }

    public static String getEvidenceUrl(String ticketNumber, long districtID, EvidenceType evidenceType, String mimeType){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getITSGateway(), EvidenceUrl), ticketNumber, districtID, evidenceType.getNumValue(), mimeType);
    }

    public static String getTicketNumberUrl(long districtID, long entityReferenceTypeID, long referenceDocumentTypeID, String deviceID, int numberCount, boolean requireTokens){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getITSGateway(), ReferenceNumberUrl), districtID, entityReferenceTypeID, referenceDocumentTypeID, deviceID, numberCount, requireTokens ? "true" : "false");
    }

    public static String getCourtsInfoUrl(long districtCode){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getITSGateway(), CourtInfoUrl), districtCode);
    }

    public static String getVosiActionUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getITSGateway(), VosiActionUrl));
    }

    public static String getVosiActionCaptureUrl(){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getITSGateway(), VosiActionCaptureUrl));
    }

    //public static String getFineUrl(SearchFinesCriteriaType criteriaType, String searchValue, boolean includeAccount, RegisterStatus registerStatus){
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

            //return String.format(combineUrl(EndPointConfigModel.getInstance().getITSGateway(), FineUrl), criteriaType.getCode(), searchValue, includeAccount == true ? "true" : "false", onlyPayable == true ? "true" : "false");
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ITSGatewayUrls::getFineUrl()"), ErrorSeverity.High);
            return null;
        }
    }

    public static String getGetEvidenceUrl(long evidenceID){
        return String.format(combineUrl(EndPointConfigModel.getInstance().getITSGateway(), GetEvidenceUrl), evidenceID);
    }
}
