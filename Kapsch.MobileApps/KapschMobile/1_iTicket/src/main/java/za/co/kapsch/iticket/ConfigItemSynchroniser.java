package za.co.kapsch.iticket;

import java.sql.SQLException;
import java.util.Calendar;
import java.util.List;

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Enums.IdentificationType;
import za.co.kapsch.iticket.Models.ChargeInfoModel;
import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.iticket.Models.CountryModel;
import za.co.kapsch.iticket.Models.CourtDetailModel;
import za.co.kapsch.iticket.Models.CourtsInfoModel;
import za.co.kapsch.iticket.Models.EvidenceModel;
import za.co.kapsch.iticket.Models.HandWrittenModel;
import za.co.kapsch.iticket.Models.IdentificationTypeModel;
import za.co.kapsch.iticket.Models.InfringementLocationModel;
import za.co.kapsch.iticket.Models.PublicHolidayModel;
import za.co.kapsch.iticket.Models.TicketNumberModel;
import za.co.kapsch.iticket.Models.VosiActionCaptureModel;
import za.co.kapsch.iticket.Models.VosiActionModel;
import za.co.kapsch.iticket.orm.ChargeInfoRepository;
import za.co.kapsch.iticket.orm.ConfigItemRepository;
import za.co.kapsch.iticket.orm.CountryRepository;
import za.co.kapsch.iticket.orm.CourtDateRepository;
import za.co.kapsch.iticket.orm.CourtDetailRepository;
import za.co.kapsch.iticket.orm.CourtRepository;
import za.co.kapsch.iticket.orm.CourtRoomRepository;
import za.co.kapsch.iticket.orm.DeviceItemRepository;
import za.co.kapsch.iticket.orm.EvidenceRepository;
import za.co.kapsch.iticket.orm.HandWrittenRepository;
import za.co.kapsch.iticket.orm.IdentificationTypeRepository;
import za.co.kapsch.iticket.orm.InfringementLocationRepository;
import za.co.kapsch.iticket.orm.PublicHolidayRepository;
import za.co.kapsch.iticket.orm.TicketNumberRepository;
import za.co.kapsch.iticket.orm.VosiActionCaptureRepository;
import za.co.kapsch.iticket.orm.VosiActionRepository;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.orm.UserActivityLogRepository;

/**
 * Created by CSenekal on 2017/01/26.
 */
public class ConfigItemSynchroniser {

    private static final String CHARGE_INFO_VERSION = "CHARGE_INFO_VERSION";
    private static final String COURT_DETAIL_VERSION = "COURT_DETAIL_VERSION";
    private static final String PUBLIC_HOLIDAY_VERSION = "PUBLIC_HOLIDAY_VERSION";
    private static final String VOSI_ACTION_VERSION = "VOSI_ACTION_VERSION";
    private static final String INFRINGEMENT_LOCATION_VERSION = "INFRINGEMENT_LOCATION_VERSION";
    private static final String IDENTIFICATION_TYPE_VERSION = "IDENTIFICATION_TYPE_VERSION";
    private static final String COUNTRY_VERSION = "COUNTRY_VERSION";

    private List<ConfigItemModel> mConfigItemList;

    public void setConfigItemList(List<ConfigItemModel> configItemList){

        mConfigItemList = configItemList;
    }

    public boolean queryForUpdates() throws Exception {

        boolean updateRequired;

        updateRequired =
                (doUpdateChargeInfo() ||
                 doUpdateCourtDetail() ||
                 doUpdatePublicHolidays() ||
                 doUpdateRoadSideTicketNumbers() ||
                 doUpdateTickets() ||
                 doUpdateIdentificationType()||
                 doUpdateCountry() ||
                 doUpdateEvidence());

        return updateRequired;
    }

    public boolean doUpdateChargeInfo() throws SQLException{
        return doUpdateConfigItem(CHARGE_INFO_VERSION);
    }

    public boolean doUpdateCourtDetail() throws SQLException{
        return doUpdateConfigItem(COURT_DETAIL_VERSION);
    }

    public boolean doUpdatePublicHolidays() throws SQLException{
        return doUpdateConfigItem(PUBLIC_HOLIDAY_VERSION);
    }

    public boolean doUpdateInfringementLocation() throws SQLException{
        return doUpdateConfigItem(INFRINGEMENT_LOCATION_VERSION);
    }

    public boolean doUpdateVosiAction() throws SQLException{
        return doUpdateConfigItem(VOSI_ACTION_VERSION);
    }

    public boolean doUpdateIdentificationType() throws SQLException{
        return doUpdateConfigItem(IDENTIFICATION_TYPE_VERSION);
    }

    public boolean doUpdateCountry() throws SQLException{
        return doUpdateConfigItem(COUNTRY_VERSION);
    }

    public boolean doUpdateRoadSideTicketNumbers() throws SQLException {
        return TicketNumberRepository.getAvailableTicketCount(DocumentType.RoadSideDriver) < ConfigItemModel.getInstance().getMinTickets();
    }

    private boolean doUpdateConfigItem(String congfigValue) throws SQLException{

        String localConfigItemValue = getConfigItemValue(getLocalConfigItem(congfigValue));

        if (localConfigItemValue == null) return true;

        return !localConfigItemValue.equals(centralConfigItemValue(congfigValue));
    }

    public boolean doUpdateTickets() throws SQLException {
        List<HandWrittenModel> handWrittenList = HandWrittenRepository.getUnSyncedTickets();
        return handWrittenList.size() > 0;
    }

    public boolean doUpdateVosiActionCapture() throws SQLException {
        List<VosiActionCaptureModel> vosiActionCaptureList = VosiActionCaptureRepository.getUnSyncedVosiActionCapture();
        return vosiActionCaptureList.size() > 0;
    }

    public boolean doUpdateUserActivityLog() throws SQLException {
        List<UserActivityLogModel> uerActivityLogList = UserActivityLogRepository.getUnSyncedUserActivityLogs();
        return uerActivityLogList.size() > 0;
    }

    public boolean doUpdateEvidence() throws SQLException {
        List<EvidenceModel> evidenceList = EvidenceRepository.getUnSyncedEvidenceEx();
        return evidenceList.size() > 0;
    }

    private String getConfigItemValue(ConfigItemModel configItem){
        return configItem == null ? null : configItem.getValue();
    }

    public String updateConfigItems(List<ConfigItemModel> centralConfigItemList) throws Exception {

        if (centralConfigItemList == null) {
            return Utilities.getString(R.string.downloading_configuration_info_failed);
        }

        for (ConfigItemModel centralConfigItem: centralConfigItemList) {
            //config items that are versions are not updated here
            if (centralConfigItem.getDescription().toUpperCase().contains(Constants.VERSION) == false) {
                ConfigItemModel localConfigItem  = ConfigItemRepository.getConfigItem(centralConfigItem.getDescription());
                if(localConfigItem != null) {
                    localConfigItem.setValue(centralConfigItem.getValue());
                    if (localConfigItem != null) {
                        if (ConfigItemRepository.update(localConfigItem) == false) {
                            return Utilities.getString(R.string.downloading_configuration_info_failed);
                        }
                    }
                }
            }
        }

        ConfigItemModel.getInstance().refreshConfigurationValues();

        return Utilities.getString(R.string.configuration_info_downloaded);
    }

    public String insertChargeInfo(List<ChargeInfoModel> chargeInfoList) throws Exception {

        if (chargeInfoList == null) {
            return Utilities.getString(R.string.downloading_charge_info_failed);
        }

        ChargeInfoRepository.cleanInsert(chargeInfoList);

        ConfigItemModel configItem = getLocalConfigItem(CHARGE_INFO_VERSION);
        configItem.setValue(centralConfigItemValue(CHARGE_INFO_VERSION));

        if (ConfigItemRepository.update(configItem) == false) {
            return Utilities.getString(R.string.downloading_charge_info_failed);
        }

        return Utilities.getString(R.string.charge_info_downloaded);
    }

//    public String insertCourtDetail(List<CourtDetailModel> courtDetailList) throws Exception {
//
//        if (courtDetailList == null) {
//            return Utilities.getString(R.string.downloading_charge_info_failed);
//        }
//
//        CourtDetailRepository.cleanInsert(courtDetailList);
//
//        ConfigItemModel configItem = getLocalConfigItem(COURT_DETAIL_VERSION);
//        configItem.setValue(centralConfigItemValue(COURT_DETAIL_VERSION));
//
//        if (ConfigItemRepository.update(configItem) == false) {
//            return Utilities.getString(R.string.downloading_charge_info_failed);
//        }
//
//        return Utilities.getString(R.string.downloading_charge_info_successfull);
//    }

    public String insertCourtsInfo(CourtsInfoModel courtsInfo) throws Exception {
        if (courtsInfo == null) {
            return App.getContext().getString(R.string.data_service_courts_update_failed_message);
        }

        CourtRepository.cleanInsert(courtsInfo.getCourts());
        CourtRoomRepository.cleanInsert(courtsInfo.getCourtRooms());
        CourtDateRepository.cleanInsert(courtsInfo.getCourtDates());

        ConfigItemModel configItem = getLocalConfigItem(COURT_DETAIL_VERSION);
        configItem.setValue(centralConfigItemValue(COURT_DETAIL_VERSION));

        if (ConfigItemRepository.update(configItem) == false) {
            return Utilities.getString(R.string.downloading_charge_info_failed);
        }

        return App.getContext().getResources().getString(R.string.data_service_courts_updated_message);
    }

    public String insertPublicHolidays(List<PublicHolidayModel> publicHolidayList) throws Exception {

        if (publicHolidayList == null) {
            return Utilities.getString(R.string.downloading_public_holidays_failed);
        }

        PublicHolidayRepository.cleanInsert(publicHolidayList);

        ConfigItemModel configItem = getLocalConfigItem(PUBLIC_HOLIDAY_VERSION);
        configItem.setValue(centralConfigItemValue(PUBLIC_HOLIDAY_VERSION));

        if (ConfigItemRepository.update(configItem) == false) {
            return Utilities.getString(R.string.downloading_public_holidays_failed);
        }

        return Utilities.getString(R.string.public_holidays_downloaded);
    }

    public String insertInfringementLocations(List<InfringementLocationModel> infringementLocationList) throws Exception {

        if (infringementLocationList == null) {
            return Utilities.getString(R.string.downloading_infringement_locations_failed);
        }

        InfringementLocationRepository.cleanInsert(infringementLocationList);

        ConfigItemModel configItem = getLocalConfigItem(INFRINGEMENT_LOCATION_VERSION);
        configItem.setValue(centralConfigItemValue(INFRINGEMENT_LOCATION_VERSION));

        if (ConfigItemRepository.update(configItem) == false) {
            return Utilities.getString(R.string.downloading_infringement_locations_failed);
        }

        return Utilities.getString(R.string.infringement_locations_downloaded);
    }

    public String insertVosiActions(List<VosiActionModel> vosiActionList) throws Exception {

        if (vosiActionList == null) {
            return Utilities.getString(R.string.downloading_vosi_action_failed);
        }

        VosiActionRepository.cleanInsert(vosiActionList);

        ConfigItemModel configItem = getLocalConfigItem(VOSI_ACTION_VERSION);
        configItem.setValue(centralConfigItemValue(VOSI_ACTION_VERSION));

        if (ConfigItemRepository.update(configItem) == false) {
            return Utilities.getString(R.string.downloading_vosi_action_failed);
        }

        return Utilities.getString(R.string.vosi_action_downloaded);
    }

    public String insertIdentificationTypes(List<IdentificationTypeModel> identificationTypeList) throws Exception {

        if (identificationTypeList == null) {
            return Utilities.getString(R.string.downloading_identification_type_failed);
        }

        IdentificationTypeRepository.cleanInsert(identificationTypeList);

        ConfigItemModel configItem = getLocalConfigItem(IDENTIFICATION_TYPE_VERSION);
        configItem.setValue(centralConfigItemValue(IDENTIFICATION_TYPE_VERSION));

        if (ConfigItemRepository.update(configItem) == false) {
            return Utilities.getString(R.string.downloading_identification_type_failed);
        }

        return Utilities.getString(R.string.identification_type_downloaded);
    }

    public String insertCountries(List<CountryModel> countryList) throws Exception {

        if (countryList == null) {
            return Utilities.getString(R.string.downloading_countries_failed);
        }

        CountryRepository.cleanInsert(countryList);

        ConfigItemModel configItem = getLocalConfigItem(COUNTRY_VERSION);
        configItem.setValue(centralConfigItemValue(COUNTRY_VERSION));

        if (ConfigItemRepository.update(configItem) == false) {
            return Utilities.getString(R.string.downloading_countries_failed);
        }

        return Utilities.getString(R.string.countries_downloaded);
    }

    public String insertTicketNumbers(List<TicketNumberModel> ticketNumberList) throws Exception {

        if (ticketNumberList == null) {
            return App.getContext().getResources().getString(R.string.downloading_ticket_numbers_failed);
        }

        TicketNumberRepository.insert(ticketNumberList);

        return App.getContext().getResources().getString(R.string.ticket_numbers_downloaded);
    }

    public String updateHandWrittenToUploaded(HandWrittenModel handWritten){

        try{
            handWritten.setUploaded(true);

            handWritten.setUploadDateTime(Calendar.getInstance().getTime());

            if (HandWrittenRepository.update(handWritten) == false) {
                return App.getContext().getResources().getString(R.string.uploading_ticket_failed);
            }

            return App.getContext().getResources().getString(R.string.uploading_ticket_successful);
        } catch (SQLException e) {
            return Utilities.exceptionMessage(e, null);
        }
    }

    public String updateVosiActionCaptureToUploaded(VosiActionCaptureModel vosiActionCapture){

        try{
            vosiActionCapture.setUploaded(true);

            if (VosiActionCaptureRepository.update(vosiActionCapture) == false) {
                return App.getContext().getResources().getString(R.string.uploading_vosi_action_capture_failed);
            }

            return App.getContext().getResources().getString(R.string.uploading_vosi_action_capture_successful);
        } catch (SQLException e) {
            return Utilities.exceptionMessage(e, null);
        }
    }

    public String updateUserActivityLogToUploaded(List<UserActivityLogModel> userActivityLogModelList){

        try{
            for (UserActivityLogModel userActivityLog : userActivityLogModelList) {
                userActivityLog.setUploaded(true);

                if (UserActivityLogRepository.update(userActivityLog) == false) {
                    return App.getContext().getResources().getString(R.string.uploading_user_activity_log_failed);
                }
            }

            return App.getContext().getResources().getString(R.string.uploading_user_activity_log_successful);
        } catch (SQLException e) {
            return Utilities.exceptionMessage(e, null);
        }
    }

    public String deleteEvidenceFile(EvidenceModel evidence) {

        try {
            if (EvidenceRepository.delete(evidence) == false) {
                return App.getContext().getResources().getString(R.string.delete_evidence_failed);
            }

        } catch (Exception e) {
            return Utilities.exceptionMessage(e, null);
        }

        return App.getContext().getResources().getString(R.string.uploading_evidence_successfull);
    }

    public String markEvidenceFileAsUpdated(EvidenceModel evidence) {

        try {
            evidence.setUploaded(true);
            evidence.setUploadDateTime(Calendar.getInstance().getTime());
            if (EvidenceRepository.update(evidence) == false) {
                return App.getContext().getResources().getString(R.string.uploading_evidence_failed);
            }
        } catch (Exception e) {
            return Utilities.exceptionMessage(e, null);
        }

        return App.getContext().getResources().getString(R.string.uploading_evidence_successfull);
    }

    public String centralConfigItemValue(String name){

        for(ConfigItemModel configItem: mConfigItemList){

            if (configItem.getDescription().equals(name)) {
                return configItem.getValue();
            }
        }

        return null;
    }

    private ConfigItemModel getLocalConfigItem(String value) throws SQLException {

        ConfigItemModel deviceItemModel = ConfigItemRepository.getConfigItem(value);
        return deviceItemModel;
    }
}
