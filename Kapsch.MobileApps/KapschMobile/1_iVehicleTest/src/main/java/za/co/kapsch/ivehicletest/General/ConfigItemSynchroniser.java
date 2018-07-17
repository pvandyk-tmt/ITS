package za.co.kapsch.ivehicletest.General;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.ivehicletest.App;
import za.co.kapsch.ivehicletest.Models.ConfigItemModel;

import za.co.kapsch.ivehicletest.Models.EvidenceModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultsModel;
import za.co.kapsch.ivehicletest.R;
import za.co.kapsch.ivehicletest.orm.ConfigItemRepository;
import za.co.kapsch.ivehicletest.orm.EvidenceRepository;
import za.co.kapsch.ivehicletest.orm.VehicleInspectionResultRepository;
import za.co.kapsch.ivehicletest.orm.VehicleInspectionResultsRepository;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.orm.UserActivityLogRepository;

/**
 * Created by CSenekal on 2017/01/26.
 */
public class ConfigItemSynchroniser {

    private static final String VEHICLE_MAKE_VERSION = "VEHICLE_MAKE_VERSION";
    private static final String VEHICLE_MODEL_NUMBER_VERSION = "VEHICLE_MODEL_NUMBER_VERSION";
    private static final String VEHICLE_MAKE_MODEL_VERSION = "VEHICLE_MAKE_MODEL_VERSION";

    private List<ConfigItemModel> mConfigItemList;

    public void setConfigItemList(List<ConfigItemModel> configItemList){

        mConfigItemList = configItemList;
    }

    public boolean queryForUpdates() throws Exception {

        boolean updateRequired = false;

        updateRequired =
                (doUpdateUserActivityLog() || doUpdateVehicleInspectionResults());

        return updateRequired;
    }

    private boolean doUpdateConfigItem(String congfigValue) throws SQLException{

        String localConfigItemValue = getConfigItemValue(getLocalConfigItem(congfigValue));

        if (localConfigItemValue == null) return true;

        return !localConfigItemValue.equals(centralConfigItemValue(congfigValue));
    }

    public boolean doUpdateVehicleMake() throws SQLException{
        return doUpdateConfigItem(VEHICLE_MAKE_VERSION);
    }

    public boolean doUpdateVehicleMakeModel() throws SQLException{
        return doUpdateConfigItem(VEHICLE_MAKE_MODEL_VERSION);
    }

    public boolean doUpdateVehicleModelNumber() throws SQLException{
        return doUpdateConfigItem(VEHICLE_MODEL_NUMBER_VERSION);
    }

    public boolean doUpdateUserActivityLog() throws SQLException {
        List<UserActivityLogModel> uerActivityLogList = UserActivityLogRepository.getUnSyncedUserActivityLogs();
        return uerActivityLogList.size() > 0;
    }

    public boolean doUpdateVehicleInspectionResults() throws SQLException {
        List<VehicleInspectionResultsModel> vehicleInspectionResultList = VehicleInspectionResultsRepository.getUnSyncedVehicleInspectionResults();
        return vehicleInspectionResultList.size() > 0;
    }

    private String getConfigItemValue(ConfigItemModel configItem){
        return configItem == null ? null : configItem.getValue();
    }

    public boolean doUpdateEvidence() throws SQLException {
        List<EvidenceModel> evidenceList = EvidenceRepository.getUnSyncedEvidence();
        return evidenceList.size() > 0;
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

    public String updateVehicleInspectionRestultsToUploaded(VehicleInspectionResultsModel vehicleInspectionResults){//List<VehicleInspectionResultModel> vehicleInspectionResultList){

        try{
            vehicleInspectionResults.setUploaded(true);
            VehicleInspectionResultsRepository.update(vehicleInspectionResults);
            for (VehicleInspectionResultModel vehicleInspectionResult : vehicleInspectionResults.getVehicleInspectionResultList()) {
                vehicleInspectionResult.setUploaded(true);

                if (VehicleInspectionResultRepository.update(vehicleInspectionResult) == false) {
                    return App.getContext().getResources().getString(R.string.uploading_inspection_failed);
                }
            }

            return App.getContext().getResources().getString(R.string.uploading_inspection_successful);
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

    public String centralConfigItemValue(String name){

        for(ConfigItemModel configItem: mConfigItemList){

            if (configItem.getDescription().equals(name)) {
                return configItem.getValue();
            }
        }

        return null;
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

    private ConfigItemModel getLocalConfigItem(String value) throws SQLException {

        ConfigItemModel deviceItemModel = ConfigItemRepository.getConfigItem(value);
        return deviceItemModel;
    }
}
