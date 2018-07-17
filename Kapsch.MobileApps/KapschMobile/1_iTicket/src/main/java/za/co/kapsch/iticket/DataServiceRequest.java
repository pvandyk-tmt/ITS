package za.co.kapsch.iticket;

import android.app.Activity;

import com.google.gson.reflect.TypeToken;

import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Enums.EntityReferenceType;
import za.co.kapsch.iticket.Enums.EvidenceType;
import za.co.kapsch.iticket.Google.GoogleGeoCodeReponse;
import za.co.kapsch.iticket.Models.CountryModel;
import za.co.kapsch.iticket.Models.IdentificationTypeModel;
import za.co.kapsch.iticket.Models.InfringementLocationModel;
import za.co.kapsch.shared.Models.FineModel;
import za.co.kapsch.iticket.Models.VosiActionCaptureModel;
import za.co.kapsch.iticket.Models.VosiActionModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Enums.RegisterStatus;
import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.iticket.Models.CourtsInfoModel;
import za.co.kapsch.iticket.Models.DistanceOverTimeSectionConfigurationModel;
import za.co.kapsch.iticket.Models.EnatisVehicleResponse;
import za.co.kapsch.iticket.Models.HandWrittenModel;
import za.co.kapsch.iticket.Models.ChargeInfoModel;
import za.co.kapsch.iticket.Models.PersonAddressInfo;
import za.co.kapsch.iticket.Models.PublicHolidayModel;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.iticket.Models.TicketNumberModel;
import za.co.kapsch.iticket.Models.VosiResponse;
import za.co.kapsch.iticket.WebAccess.CoreGatewayUrls;
import za.co.kapsch.iticket.WebAccess.DistanceOverTimeGatewayUrls;
import za.co.kapsch.iticket.WebAccess.GatewayUrls;
import za.co.kapsch.iticket.WebAccess.ITSGatewayUrls;
import za.co.kapsch.shared.Enums.FilterJoin;
import za.co.kapsch.shared.Models.FilterModel;
import za.co.kapsch.shared.Models.PaginationListModel;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.WebAccess.DataService;

import static za.co.kapsch.shared.Constants.PROCESS_ID_GOOGLE_ADDRESS_SEARCH_BY_GPS;

/**
 * Created by CSenekal on 2017/01/21.
 */
public class DataServiceRequest {

    //This is the default district - mobile device is no longer associated with a specific district
    private static long DEVICE_DISTRICT_ID = 0;

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

    public static void chargeInfoRequest(IAsyncProcessCallBack caller, Activity activity) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                ITSGatewayUrls.getChargeInfoUrl(SessionModel.getInstance().getDistrict().getID()),
                null,
                null,
                Constants.PROCESS_ID_DOWNLOAD_CHARGE_INFO,
                new TypeToken<List<ChargeInfoModel>>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void courtsInfoRequest(IAsyncProcessCallBack caller, Activity activity) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                ITSGatewayUrls.getCourtsInfoUrl(SessionModel.getInstance().getDistrict().getID()),
                null,
                null,
                Constants.PROCESS_ID_DOWNLOAD_COURTS_INFO,
                new TypeToken<CourtsInfoModel>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void courtsRequest(IAsyncProcessCallBack caller, Activity activity) {

        //For the device this default filter list is always used.
        List<FilterModel> filterList = new ArrayList<>();

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.getCourtDetailUrl(FilterJoin.And.getNumValue(), true, "ID", 1, 1000000),
                filterList,
                null,
                Constants.PROCESS_ID_DOWNLOAD_COURT_DETAIL,
                new TypeToken<PaginationListModel<CourtsInfoModel>>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void publicHolidayRequest(IAsyncProcessCallBack caller, Activity activity) {

        //For the device this default filter list is always used.
        List<FilterModel> filterList = new ArrayList<>();

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.getPublicHolidayUrl(FilterJoin.And.getNumValue(), true, "ID", 1, 1000000),
                filterList,
                null,
                Constants.PROCESS_ID_DOWNLOAD_PUBLIC_HOLIDAYS,
                new TypeToken<PaginationListModel<PublicHolidayModel>>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void vosiActionRequest(IAsyncProcessCallBack caller, Activity activity) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                ITSGatewayUrls.getVosiActionUrl(),
                null,
                null,
                Constants.PROCESS_ID_DOWNLOAD_VOSI_ACTION,
                new TypeToken<List<VosiActionModel>>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void identificationTypeRequest(IAsyncProcessCallBack caller, Activity activity) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                CoreGatewayUrls.getIdentificationTypeUrl(),
                null,
                null,
                Constants.PROCESS_ID_DOWNLOAD_IDENTIFICATION_TYPE,
                new TypeToken<List<IdentificationTypeModel>>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void ticketNumbersRequest(IAsyncProcessCallBack caller, Activity activity, DocumentType documentType) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                ITSGatewayUrls.getTicketNumberUrl(
                        DEVICE_DISTRICT_ID,
                        EntityReferenceType.toInteger(EntityReferenceType.TrafficViolations),
                        DocumentType.toInteger(DocumentType.RoadSideDriver),
                        Utilities.getDeviceId(),
                        ConfigItemModel.getInstance().getTicketBookSize(),
                        true),
                null,
                null,
                Constants.PROCESS_ID_DOWNLOAD_TICKET_NUMBERS,
                new TypeToken<List<TicketNumberModel>>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void googleAddressSearchRequest(IAsyncProcessCallBack caller, Activity activity, String partialAddress) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                CoreGatewayUrls.geoInfoUrl(URLEncoder.encode(partialAddress)),
                null,
                null,
                Constants.PROCESS_ID_GOOGLE_ADDRESS_SEARCH_BY_ADDRESS,
                GoogleGeoCodeReponse.class,
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void googleAddressSearchRequest(IAsyncProcessCallBack caller, Activity activity, String latitude, String longitude) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                CoreGatewayUrls.geoInfoUrl(latitude, longitude),
                null,
                null,
                PROCESS_ID_GOOGLE_ADDRESS_SEARCH_BY_GPS,
                GoogleGeoCodeReponse.class,
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void personInfoRequest(IAsyncProcessCallBack caller, Activity activity, String idNumber) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                CoreGatewayUrls.personInfoUrl(idNumber),
                null,
                null,
                Constants.PROCESS_ID_PERSON_ADDRESS_INFO_SEARCH,
                PersonAddressInfo.class,
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void handWrittenUploadRequest(IAsyncProcessCallBack caller, Activity activity, HandWrittenModel handWrittenModel) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                ITSGatewayUrls.getHandWrittenCaptureUrl(handWrittenModel.getOffenceSetID(), false),
                handWrittenModel,
                null,
                Constants.PROCESS_ID_UPLOAD_HANDWRITTEN,
                null,
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void vosiActionCaptureUploadRequest(IAsyncProcessCallBack caller, Activity activity, VosiActionCaptureModel vosiActionCaptureModel) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                ITSGatewayUrls.getVosiActionCaptureUrl(),
                vosiActionCaptureModel,
                null,
                Constants.PROCESS_ID_UPLOAD_VOSI_ACTION_CAPTURE,
                null,
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void evidenceUploadRequest(IAsyncProcessCallBack caller, Activity activity, String ticketNumber, long districtID, EvidenceType evidenceType, String mimeType, byte[] evidenceData){

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                ITSGatewayUrls.getEvidenceUrl(ticketNumber, districtID, evidenceType, mimeType),
                null,
                evidenceData,
                Constants.PROCESS_ID_UPLOAD_EVIDENCE,
                null,
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void vosiRequest(IAsyncProcessCallBack caller, Activity activity, String licenceNumber) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                GatewayUrls.getVosiUrl(licenceNumber, Utilities.getDeviceId()),
                null,
                null,
                Constants.PROCESS_ID_VOSI_LOOKUP,
                VosiResponse.class,
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void eNatisVehicleRequest(IAsyncProcessCallBack caller, Activity activity, String licenceNumber) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                GatewayUrls.getEnatisVehicleUrl(licenceNumber),
                null,
                null,
                Constants.PROCESS_ID_ENATIS_VEHICLE_LOOKUP,
                EnatisVehicleResponse.class,
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void distanceOverTimeServiceRequest(IAsyncProcessCallBack caller, Activity activity, DistanceOverTimeSectionConfigurationModel distanceOverTimeSectionConfigurationModel) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                DistanceOverTimeGatewayUrls.getServiceRequestUrl(),
                distanceOverTimeSectionConfigurationModel,
                null,
                Constants.PROCESS_ID_DISTANCE_OVER_TIME_SERVICE_REQUEST,
                null,
                true,
                null,
                false,
                false,
                false,
                false);
    }

    public static void infringementLocationRequest(IAsyncProcessCallBack caller, Activity activity) {

        List<FilterModel> filterList = new ArrayList<>();

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.getInfringementLocationUrl(0, true, "Code", 1, 1000000),
                filterList,
                null,
                Constants.PROCESS_ID_DOWNLOAD_INFRINGEMENT_LOCATIONS,
                new TypeToken<PaginationListModel<InfringementLocationModel>>(){}.getType(),
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

    public static void finesRequest(IAsyncProcessCallBack caller, Activity activity, SearchFinesCriteriaType criteriaType, String searchValue, boolean showBusyIndicator) {

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
                showBusyIndicator,
                true,
                false);
    }

    public static void countryRequest(IAsyncProcessCallBack caller, Activity activity) {

        //For the device this default filter list is always used.
        List<FilterModel> filterList = new ArrayList<>();

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_POST,
                CoreGatewayUrls.getCountryUrl(FilterJoin.And.getNumValue(), true, "ID", 1, 1000000),
                filterList,
                null,
                Constants.PROCESS_ID_DOWNLOAD_COUNTRY,
                new TypeToken<PaginationListModel<CountryModel>>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                false);
    }

    public static void getEvidenceRequest(IAsyncProcessCallBack caller, Activity activity, long evidenceID, int processID) {

        DataService dataService = new DataService(caller, activity);
        dataService.request(
                Constants.REQUEST_METHOD_GET,
                ITSGatewayUrls.getGetEvidenceUrl(evidenceID),
                null,
                null,
                processID,
                new TypeToken<byte[]>(){}.getType(),
                true,
                null,
                false,
                true,
                false,
                true);
    }
}
