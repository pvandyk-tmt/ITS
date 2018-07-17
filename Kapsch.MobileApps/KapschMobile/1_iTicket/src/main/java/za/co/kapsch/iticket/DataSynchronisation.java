package za.co.kapsch.iticket;

import android.app.Activity;
import android.support.v4.app.Fragment;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Enums.EvidenceType;
import za.co.kapsch.iticket.Enums.VosiActionType;
import za.co.kapsch.iticket.Models.CountryModel;
import za.co.kapsch.iticket.Models.IdentificationTypeModel;
import za.co.kapsch.iticket.Models.InfringementLocationModel;
import za.co.kapsch.iticket.Models.VosiActionCaptureModel;
import za.co.kapsch.iticket.Models.VosiActionModel;
import za.co.kapsch.iticket.orm.VosiActionCaptureRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Interfaces.IMessageCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.iticket.Models.ChargeInfoModel;
import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.iticket.Models.CourtsInfoModel;
import za.co.kapsch.iticket.Models.EvidenceModel;
import za.co.kapsch.iticket.Models.HandWrittenModel;
import za.co.kapsch.iticket.Models.PublicHolidayModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.iticket.Models.TicketNumberModel;
import za.co.kapsch.iticket.orm.EvidenceRepository;
import za.co.kapsch.iticket.orm.HandWrittenRepository;
import za.co.kapsch.shared.Models.PaginationListModel;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.orm.UserActivityLogRepository;


import static za.co.kapsch.shared.WebAccess.DataService.FAILED;
import static za.co.kapsch.shared.WebAccess.DataService.SUCCESS;

/**
 * Created by CSenekal on 2017/02/21.
 */
public class DataSynchronisation  implements IAsyncProcessCallBack {

    private Activity mActivity;
    private IMessageCallBack mCaller;
    private ConfigItemSynchroniser mConfigItemSynchroniser;
    private List<String> mAttemptedTicketUpdates;
    private EvidenceModel mCurrentEvidence;
    private List<String> mAttemptedEvidenceUpdates;
    private boolean mPromptForUpdate;
    private List<String> mAttemptedTicketsComplete;
    private List<Long> mAttemptedVosiActionCaptureUpdates;


    public DataSynchronisation(IMessageCallBack caller, boolean promptForUpdate) throws Exception {

        if ( ((caller instanceof Activity) == false) && ((caller instanceof Fragment) == false)){
            throw new Exception("IMessageCallBack must be of type Activity or Fragment");
        }

        if (caller instanceof Activity){
            mActivity = (Activity)caller;
        }else if (caller instanceof Fragment){
            mActivity = ((Fragment)caller).getActivity();
        }

        mCaller = caller;
        mPromptForUpdate = promptForUpdate;

        mAttemptedTicketUpdates = new ArrayList<>();
        mAttemptedEvidenceUpdates = new ArrayList<>();
        mAttemptedTicketsComplete = new ArrayList<>();
        mAttemptedVosiActionCaptureUpdates = new ArrayList<>();
    }

    public void processUpdates() throws Exception {

        if (centralConfigItems() == true) return;

        if (updateChargeInfoItems() == true) return;

        if (updateCourtsInfo() == true) return;

        if (updatePublicHolidays() == true) return;

        if (updateInfringementLocation() == true) return;

        if (updateVosiAction() == true) return;

        if (updateIdentificationType() == true) return;

        if (updateCountry() == true) return;

        if (updateVosiActionCapture() == true) return;

        if (updateRoadSideTicketNumbers() == true) return;

        if (updateHandWrittenTickets() == true) return;

        if (updateEvidence() == true) return;

        if (updateUserActivityLog() == true) return;

        mCaller.message(Constants.FINISHED_MESSAGE, true);
    }

//    public boolean getEvidence(){
//        DataServiceRequest.getEvidenceRequest(this, mActivity, 14246, "Signature.jpg");
//        return true;
//    }

    public boolean centralConfigItems(){

        if (mConfigItemSynchroniser == null){
            //mCaller.message(Utilities.getString(R.string.downloading_configuration_info), true);
            DataServiceRequest.configItemRequest(this, mActivity);
            return true;
        }

        return false;
    }

    private boolean updateChargeInfoItems() throws SQLException {

        if (mConfigItemSynchroniser.doUpdateChargeInfo()) {
            mCaller.message(Utilities.getString(R.string.downloading_charge_info), true);
            DataServiceRequest.chargeInfoRequest(this, mActivity);
            return true;
        }

        return false;
    }

    private boolean updateCourtsInfo() throws SQLException {

        if (mConfigItemSynchroniser.doUpdateCourtDetail()) {
            mCaller.message(App.getContext().getString(R.string.downloading_court_detail), true);
            DataServiceRequest.courtsInfoRequest(this, mActivity);
            return true;
        }

        return false;
    }

    private boolean updatePublicHolidays() throws SQLException {

        if (mConfigItemSynchroniser.doUpdatePublicHolidays()) {
            mCaller.message(Utilities.getString(R.string.downloading_public_holidays), true);
            DataServiceRequest.publicHolidayRequest(this, mActivity);
            return true;
        }

        return false;
    }

    private boolean updateInfringementLocation() throws SQLException {

//        if (mConfigItemSynchroniser.doUpdateInfringementLocation()) {
//            mCaller.message(Utilities.getString(R.string.downloading_infringement_locations), true);
//            DataServiceRequest.infringementLocationRequest(this, mActivity);
//            return true;
//        }

        return false;
    }

    private boolean updateVosiAction() throws SQLException {

        if (mConfigItemSynchroniser.doUpdateVosiAction()) {
            mCaller.message(Utilities.getString(R.string.downloading_vosi_action), true);
            DataServiceRequest.vosiActionRequest(this, mActivity);
            return true;
        }

        return false;
    }

    private boolean updateIdentificationType() throws SQLException {

        if (mConfigItemSynchroniser.doUpdateIdentificationType()) {
            mCaller.message(Utilities.getString(R.string.downloading_identification_type), true);
            DataServiceRequest.identificationTypeRequest(this, mActivity);
            return true;
        }

        return false;
    }

    private boolean updateCountry() throws SQLException {

        if (mConfigItemSynchroniser.doUpdateCountry()) {
            mCaller.message(Utilities.getString(R.string.downloading_countries), true);
            DataServiceRequest.countryRequest(this, mActivity);
            return true;
        }

        return false;
    }

    private boolean updateRoadSideTicketNumbers() throws SQLException {

        if (mConfigItemSynchroniser.doUpdateRoadSideTicketNumbers()) {
            mCaller.message(Utilities.getString(R.string.downloading_ticket_numbers), true);
            DataServiceRequest.ticketNumbersRequest(this, mActivity, DocumentType.RoadSideDriver);
            return true;
        }

        return false;
    }

    private boolean updateHandWrittenTickets() throws SQLException{

        if (mConfigItemSynchroniser.doUpdateTickets()) {

            try {
                List<HandWrittenModel> handWrittenModellList = HandWrittenRepository.getUnSyncedTickets();

                for (HandWrittenModel handWritten : handWrittenModellList) {
                    //offenceDateSanityCheck(handWritten.getOffenceDate(), handWritten.getIssueDate());
                    if (ticketUpdateAttempted(handWritten.getTicketNumber()) == false) {
                        mAttemptedTicketUpdates.add(handWritten.getTicketNumber());
                        mCaller.message(App.getContext().getResources().getString(R.string.uploading_ticket), true);
                        DataServiceRequest.handWrittenUploadRequest(this, mActivity, handWritten);
                        return true;
                    }
                }
            } catch (SQLException e) {
                mCaller.message(Utilities.exceptionMessage(e, null), true);
                return true;
            } catch (Exception e) {
                mCaller.message(Utilities.exceptionMessage(e, null), true);
                return true;
            }
            return false;
        }

        return false;
    }

    public void offenceDateSanityCheck(Date offenceDate, Date issueDate){

        try {
            if (EndPointConfigModel.getInstance().getITSGateway().equals("192.168.0.33:60002")) {

                if (issueDate == null) {
                    MessageManager.showMessage("ISSUE DATE IS NULL", ErrorSeverity.None);
                    Utilities.displayOkMessage("ISSUE DATE IS NULL", mActivity);
                }

                if (offenceDate == null) {
                    MessageManager.showMessage("OFFENCE DATE IS NULL", ErrorSeverity.None);
                    Utilities.displayOkMessage("OFFENCE DATE IS NULL", mActivity);
                }

                if (HandWrittenRepository.offenceDateCount(offenceDate) > 1) {
                    MessageManager.showMessage("OFFENCE DATE ALREADY EXIST", ErrorSeverity.None);
                    Utilities.displayOkMessage("OFFENCE DATE ALREADY EXIST", mActivity);
                }
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "offenceDateSanityCheck"), ErrorSeverity.High);
        }
    }

    private boolean ticketUpdateAttempted(String ticketNumber){

        for (String value: mAttemptedTicketUpdates) {
            if(value.equals(ticketNumber)){
                return true;
            }
        }

        return false;
    }

    private boolean updateVosiActionCapture() throws SQLException{

//        VosiActionCaptureModel vosiActionCaptureTest = new VosiActionCaptureModel();
//
//        vosiActionCaptureTest.setCapturedDateTime(Calendar.getInstance().getTime());
//        vosiActionCaptureTest.setCapturedCredentialID(SessionModel.getInstance().getUser().getCredentialID());
//        vosiActionCaptureTest.setComments("Test");
//        vosiActionCaptureTest.setLocationLatitude(-33.90307666666667);
//        vosiActionCaptureTest.setLocationlongitude(18.6263);
//        vosiActionCaptureTest.setLocationStreet("Street");
//        vosiActionCaptureTest.setLocationSuburb("Suburb");
//        vosiActionCaptureTest.setLocationTown("Town");
//        vosiActionCaptureTest.setVLN("CY123456");
//        vosiActionCaptureTest.setVosiActionID(VosiActionType.Cancelled.getCode());
//
//        DataServiceRequest.vosiActionCaptureUploadRequest(this, mActivity, vosiActionCaptureTest);

        if (mConfigItemSynchroniser.doUpdateVosiActionCapture()) {

            try {
                List<VosiActionCaptureModel> vosiActionCaptureList = VosiActionCaptureRepository.getUnSyncedVosiActionCapture();

                for (VosiActionCaptureModel vosiActionCapture : vosiActionCaptureList) {
                    if (vosiActionCaptureUpdateAttempted(vosiActionCapture.getID()) == false) {
                        mAttemptedVosiActionCaptureUpdates.add(vosiActionCapture.getID());
                        mCaller.message(App.getContext().getResources().getString(R.string.uploading_vosi_action_capture), true);
                        DataServiceRequest.vosiActionCaptureUploadRequest(this, mActivity, vosiActionCapture);
                        return true;
                    }
                }
            } catch (SQLException e) {
                mCaller.message(Utilities.exceptionMessage(e, null), true);
                return true;
            } catch (Exception e) {
                mCaller.message(Utilities.exceptionMessage(e, null), true);
                return true;
            }
            return false;
        }

        return false;
    }

    private boolean updateUserActivityLog() throws SQLException{

        if (mConfigItemSynchroniser.doUpdateUserActivityLog()) {

            try {
                List<UserActivityLogModel> userActivityLogList = UserActivityLogRepository.getUnSyncedUserActivityLogs();
                mCaller.message(App.getContext().getResources().getString(R.string.uploading_user_activity_log), true);
                DataServiceRequest.userActivityLogUploadRequest(this, mActivity, userActivityLogList);
                return true;
            } catch (SQLException e) {
                mCaller.message(Utilities.exceptionMessage(e, null), true);
                return true;
            } catch (Exception e) {
                mCaller.message(Utilities.exceptionMessage(e, null), true);
                return true;
            }
        }

        return false;
    }

    private boolean vosiActionCaptureUpdateAttempted(long id){

        for (long value: mAttemptedVosiActionCaptureUpdates) {
            if(value == id){
                return true;
            }
        }

        return false;
    }

    private boolean updateEvidence() throws SQLException{

        if (mConfigItemSynchroniser.doUpdateEvidence()) {
            mCaller.message(App.getContext().getResources().getString(R.string.data_service_updating_evidence_message), true);
            try {
                List<EvidenceModel> evidenceList = EvidenceRepository.getUnSyncedEvidenceEx();

                for (EvidenceModel evidence : evidenceList) {
                    mCurrentEvidence = evidence;

                    String mimeType = null;
                    switch(evidence.getEvidenceType()){
                        case PersonSignature:
                            mimeType = Constants.MIME_TYPE_PNG;
                            break;
                        case OfficerSignature:
                            mimeType = Constants.MIME_TYPE_PNG;
                            break;
                        case VoiceRecording:
                            mimeType = Constants.MIME_TYPE_MP4;
                            break;
                        case Other:
                            mimeType = Constants.MIME_TYPE_JPEG;
                            break;
                        case OffenderPhoto:
                            mimeType = Constants.MIME_TYPE_JPEG;
                            break;
                    }

                    if (evidenceUpdateAttempted(String.format("%s%d", evidence.getTicketNumber(), evidence.getID())) == false){
                        mAttemptedEvidenceUpdates.add(String.format("%s%d", evidence.getTicketNumber(), evidence.getID()));
                        DataServiceRequest.evidenceUploadRequest(
                                this,
                                mActivity,
                                evidence.getTicketNumber(),
                                SessionModel.getInstance().getDistrict().getID(),
                                evidence.getEvidenceType(),
                                mimeType,
                                evidence.getEvidence());

                        return true;
                    }else{
                        return false;
                    }
                }
            } catch (SQLException e) {
                mCaller.message(Utilities.exceptionMessage(e, null), true);
                return true;
            }
        }

        return false;
    }

    private boolean evidenceUpdateAttempted(String ticketNumberId){

        for (String value: mAttemptedEvidenceUpdates) {
            if(value.equals(ticketNumberId)){
                return true;
            }
        }

        return false;
    }

    private void queryForUpdates() throws Exception{

        if (mConfigItemSynchroniser != null) {
            if (mConfigItemSynchroniser.queryForUpdates()) {
                mCaller.message(Constants.SYNCHRONISATION_REQUIRED, false);
            } else {
                processUpdates();
            }
        }
    }

    private ConfigItemSynchroniser getConfigItemSynchroniser(){

        mConfigItemSynchroniser = mConfigItemSynchroniser == null ? new ConfigItemSynchroniser() : mConfigItemSynchroniser;
        return mConfigItemSynchroniser;
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {

        try {

            if (asyncResultModel == null){
                return;
            }

            switch (asyncResultModel.getProcessId()) {

                case Constants.PROCESS_ID_GET_DEVICE_CONFIG_ITEM:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            getConfigItemSynchroniser().setConfigItemList((List<ConfigItemModel>)asyncResultModel.getObject());
                            //mCaller.message(mConfigItemSynchroniser.updateConfigItems((List<ConfigItemModel>)asyncResultModel.getObject()), true);
                            mConfigItemSynchroniser.updateConfigItems((List<ConfigItemModel>)asyncResultModel.getObject());

                            if (mPromptForUpdate == true){
                                queryForUpdates();
                            }else {
                                processUpdates();
                            }

                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DOWNLOAD_CHARGE_INFO:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            List<ChargeInfoModel> chargeInfoList = (List<ChargeInfoModel>)asyncResultModel.getObject();
                            if (chargeInfoList.size() == 0){
                                mCaller.message(App.getContext().getResources().getString(R.string.charge_info_list_is_empty), true);
                                return;
                            }
                            mCaller.message(mConfigItemSynchroniser.insertChargeInfo(chargeInfoList), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DOWNLOAD_COURTS_INFO:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            //mCaller.message(mConfigItemSynchroniser.insertCourtDetail((List<CourtDetailModel>)((PaginationListModel)asyncResultModel.getObject()).getModels()), true);
                            CourtsInfoModel courtsInfo = (CourtsInfoModel)asyncResultModel.getObject();
                            if (courtsInfo.getCourts().size() == 0){
                                mCaller.message(App.getContext().getResources().getString(R.string.court_list_is_empty), true);
                                return;
                            }
                            if (courtsInfo.getCourtRooms().size() == 0){
                                mCaller.message(App.getContext().getResources().getString(R.string.court_room_list_is_empty), true);
                                return;
                            }
                            if (courtsInfo.getCourtDates().size() == 0){
                                mCaller.message(App.getContext().getResources().getString(R.string.court_date_list_is_empty), true);
                                return;
                            }
                            mCaller.message(mConfigItemSynchroniser.insertCourtsInfo(courtsInfo), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DOWNLOAD_PUBLIC_HOLIDAYS:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(mConfigItemSynchroniser.insertPublicHolidays((List<PublicHolidayModel>)((PaginationListModel)asyncResultModel.getObject()).getModels()), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DOWNLOAD_INFRINGEMENT_LOCATIONS:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(mConfigItemSynchroniser.insertInfringementLocations((List< InfringementLocationModel>)((PaginationListModel)asyncResultModel.getObject()).getModels()), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DOWNLOAD_VOSI_ACTION:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(mConfigItemSynchroniser.insertVosiActions((List<VosiActionModel>)(asyncResultModel.getObject())), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DOWNLOAD_IDENTIFICATION_TYPE:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(mConfigItemSynchroniser.insertIdentificationTypes((List<IdentificationTypeModel>)(asyncResultModel.getObject())), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DOWNLOAD_COUNTRY:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(mConfigItemSynchroniser.insertCountries((List<CountryModel>)((PaginationListModel)asyncResultModel.getObject()).getModels()), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DOWNLOAD_TICKET_NUMBERS:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(mConfigItemSynchroniser.insertTicketNumbers((List<TicketNumberModel>)asyncResultModel.getObject()), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_UPLOAD_HANDWRITTEN:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(mConfigItemSynchroniser.updateHandWrittenToUploaded((HandWrittenModel) asyncResultModel.getObject()), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            processUpdates();
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_UPLOAD_VOSI_ACTION_CAPTURE:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(mConfigItemSynchroniser.updateVosiActionCaptureToUploaded((VosiActionCaptureModel) asyncResultModel.getObject()), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            processUpdates();
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_UPLOAD_USER_ACTIVITY_LOG:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(mConfigItemSynchroniser.updateUserActivityLogToUploaded((List<UserActivityLogModel>) asyncResultModel.getObject()), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_UPLOAD_EVIDENCE:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            if ((mCurrentEvidence.getEvidenceType() != EvidenceType.OffenderPhoto) &&
                                (mCurrentEvidence.getEvidenceType() != EvidenceType.PersonSignature) &&
                                (mCurrentEvidence.getEvidenceType() != EvidenceType.OfficerSignature)) {
                                 mCaller.message(mConfigItemSynchroniser.deleteEvidenceFile(mCurrentEvidence), true);
                            }else{
                                mCaller.message(mConfigItemSynchroniser.markEvidenceFileAsUpdated(mCurrentEvidence), true);
                            }
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            processUpdates();
                            break;
                    }
                    break;

//                case Constants.PROCESS_ID_GET_EVIDENCE:
//                    switch (asyncResultModel.getProcessResult()) {
//                        case SUCCESS:
//                            processUpdates();
//                            return;
//                        case FAILED:
//                            mCaller.message(asyncResultModel.getMessage(), true);
//                            break;
//                    }
//                    break;
            }

            mCaller.message(Constants.FAILED_MESSAGE, true);
        }catch (Exception e) {
            mCaller.message(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), true);
            mCaller.message(Constants.FAILED_MESSAGE, true);
            return;
        }
    }
}
