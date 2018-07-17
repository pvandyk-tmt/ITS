package za.co.kapsch.ivehicletest.General;

import android.app.Activity;
import android.support.v4.app.Fragment;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.ivehicletest.App;
import za.co.kapsch.ivehicletest.Enums.InspectionEvidenceType;
import za.co.kapsch.ivehicletest.General.ConfigItemSynchroniser;
import za.co.kapsch.ivehicletest.General.Constants;
import za.co.kapsch.ivehicletest.General.DataServiceRequest;
import za.co.kapsch.ivehicletest.Models.ConfigItemModel;
import za.co.kapsch.ivehicletest.Models.EvidenceModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultsModel;
import za.co.kapsch.ivehicletest.R;
import za.co.kapsch.ivehicletest.orm.EvidenceRepository;
import za.co.kapsch.ivehicletest.orm.VehicleInspectionResultRepository;
import za.co.kapsch.ivehicletest.orm.VehicleInspectionResultsRepository;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Interfaces.IMessageCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.orm.UserActivityLogRepository;

import static za.co.kapsch.shared.WebAccess.DataService.FAILED;
import static za.co.kapsch.shared.WebAccess.DataService.SUCCESS;

/**
 * Created by CSenekal on 2017/02/21.
 */
public class DataSynchronisation implements IAsyncProcessCallBack {

    private Activity mActivity;
    private IMessageCallBack mCaller;
    private ConfigItemSynchroniser mConfigItemSynchroniser;
    private List<Long> mAttemptedVehicleInspectionResultsUpdates;
    private boolean mPromptForUpdate;
    private EvidenceModel mCurrentEvidence;
    private List<Long> mAttemptedEvidenceUpdates;

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
        mAttemptedEvidenceUpdates = new ArrayList<>();
        mAttemptedVehicleInspectionResultsUpdates = new ArrayList<>();
    }

    public void processUpdates(boolean vehicleInspectionResults) throws Exception {

        if (vehicleInspectionResults == false){
            if (centralConfigItems() == true) return;
            if (updateUserActivityLog() == true) return;
        }

        if (updateVehicleInspectionResults() == true) return;

        if (updateEvidence() == true) return;

        mCaller.message(Constants.FINISHED_MESSAGE, true);
    }

    public boolean centralConfigItems(){

        if (mConfigItemSynchroniser == null){
            //mCaller.message(Utilities.getString(R.string.downloading_configuration_info), true);
            DataServiceRequest.configItemRequest(this, mActivity);
            return true;
        }

        return false;
    }

    private boolean updateVehicleLookups() throws SQLException {

        if (getConfigItemSynchroniser().doUpdateVehicleMake() ||
            getConfigItemSynchroniser().doUpdateVehicleMake() ||
            getConfigItemSynchroniser().doUpdateVehicleModelNumber()) {

            mCaller.message(App.getContext().getString(R.string.downloading_vehicle_lookup), true);
            DataServiceRequest.vehicleLookupsRequest(
                    this,
                    mActivity,
                    getConfigItemSynchroniser().doUpdateVehicleMake(),
                    getConfigItemSynchroniser().doUpdateVehicleMake(),
                    getConfigItemSynchroniser().doUpdateVehicleModelNumber());

            return true;
        }

        return false;
    }

    private boolean updateVehicleInspectionResults() throws SQLException{

        if (getConfigItemSynchroniser().doUpdateVehicleInspectionResults()) {

            try {
                List<VehicleInspectionResultsModel> vehicleInspectionResultsList = VehicleInspectionResultsRepository.getUnSyncedVehicleInspectionResults();

                for (VehicleInspectionResultsModel vehicleInspectionResults : vehicleInspectionResultsList) {
                    List<VehicleInspectionResultModel> vehicleInspectionResultList = VehicleInspectionResultRepository.getUnSyncedVehicleInspectionResults(vehicleInspectionResults.getID());
                    vehicleInspectionResults.setVehicleInspectionResultList(vehicleInspectionResultList);

                    if (vehicleInspectionResultsUpdateAttempted(vehicleInspectionResults.getID()) == false) {
                        mAttemptedVehicleInspectionResultsUpdates.add(vehicleInspectionResults.getID());
                        mCaller.message(App.getContext().getResources().getString(R.string.uploading_inspection_results), true);
                        DataServiceRequest.vehicleInspectionResultUploadRequest(this, mActivity, vehicleInspectionResults);
                        return true;
                    }
                }
           } catch(SQLException e){
                mCaller.message(Utilities.exceptionMessage(e, null), true);
                return true;
            } catch(Exception e){
                mCaller.message(Utilities.exceptionMessage(e, null), true);
                return true;
            }
        }

        return false;
    }

    private boolean vehicleInspectionResultsUpdateAttempted(long ID){

        for (long value: mAttemptedVehicleInspectionResultsUpdates) {
            if(value == ID){
                return true;
            }
        }

        return false;
    }

    private boolean updateUserActivityLog() throws SQLException{

        if (getConfigItemSynchroniser().doUpdateUserActivityLog()) {

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

    private boolean updateEvidence() throws SQLException{

        if (mConfigItemSynchroniser.doUpdateEvidence()) {
            mCaller.message(App.getContext().getResources().getString(R.string.data_service_updating_evidence_message), true);
            try {
                List<EvidenceModel> evidenceList = EvidenceRepository.getUnSyncedEvidence();

                for (EvidenceModel evidence : evidenceList) {
                    mCurrentEvidence = evidence;

                    String mimeType = null;
                    switch(evidence.getInspectionEvidenceType()){
                        case VehiclePhoto:
                            mimeType = Constants.MIME_TYPE_JPEG;
                            break;
                    }

                    if (evidenceUpdateAttempted(evidence.getID()) == false){
                        mAttemptedEvidenceUpdates.add((long)evidence.getID());
                        DataServiceRequest.evidenceUploadRequest(
                                this,
                                mActivity,
                                evidence.getBookingID(),
                                evidence.getSiteID(),
                                evidence.getInspectionEvidenceType(),
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

    private boolean evidenceUpdateAttempted(long bookingID){

        for (long value: mAttemptedEvidenceUpdates) {
            if(value == bookingID){
                return true;
            }
        }

        return false;
    }


//    private boolean vosiActionCaptureUpdateAttempted(long id){
//
//        for (long value: mAttemptedVosiActionCaptureUpdates) {
//            if(value == id){
//                return true;
//            }
//        }
//
//        return false;
//    }

    private void queryForUpdates() throws Exception{

        if (getConfigItemSynchroniser() != null) {
            if (getConfigItemSynchroniser().queryForUpdates()) {
                mCaller.message(Constants.SYNCHRONISATION_REQUIRED, false);
            } else {
                processUpdates(false);
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
                            getConfigItemSynchroniser().updateConfigItems((List<ConfigItemModel>)asyncResultModel.getObject());

                            if (mPromptForUpdate == true){
                                queryForUpdates();
                            }else {
                                processUpdates(false);
                            }

                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_UPLOAD_VEHICLE_INSPECTION_RESULTS:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(getConfigItemSynchroniser().updateVehicleInspectionRestultsToUploaded((VehicleInspectionResultsModel) asyncResultModel.getObject()), true);
                            processUpdates(false);
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_UPLOAD_USER_ACTIVITY_LOG:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(getConfigItemSynchroniser().updateUserActivityLogToUploaded((List<UserActivityLogModel>) asyncResultModel.getObject()), true);
                            processUpdates(false);
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_UPLOAD_EVIDENCE:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(mConfigItemSynchroniser.deleteEvidenceFile(mCurrentEvidence), true);
                            processUpdates(false);
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            //processUpdates();
                            break;
                    }
                    break;
            }
        }catch (Exception e) {
            mCaller.message(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), true);
            mCaller.message(Constants.FAILED_MESSAGE, true);
            return;
        }
    }
}
