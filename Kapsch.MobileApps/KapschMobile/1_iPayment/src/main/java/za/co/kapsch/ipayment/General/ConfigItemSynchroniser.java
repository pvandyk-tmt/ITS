package za.co.kapsch.ipayment.General;

import java.sql.SQLException;
import java.util.Calendar;
import java.util.List;

import za.co.kapsch.ipayment.Models.ConfigItemModel;
import za.co.kapsch.ipayment.R;
import za.co.kapsch.ipayment.orm.ConfigItemRepository;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.orm.UserActivityLogRepository;

/**
 * Created by CSenekal on 2017/01/26.
 */
public class ConfigItemSynchroniser {

    private List<ConfigItemModel> mConfigItemList;

    public void setConfigItemList(List<ConfigItemModel> configItemList){

        mConfigItemList = configItemList;
    }

    private boolean doUpdateConfigItem(String congfigValue) throws SQLException{

        String localConfigItemValue = getConfigItemValue(getLocalConfigItem(congfigValue));

        if (localConfigItemValue == null) return true;

        return !localConfigItemValue.equals(centralConfigItemValue(congfigValue));
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

    private ConfigItemModel getLocalConfigItem(String value) throws SQLException {

        ConfigItemModel deviceItemModel = ConfigItemRepository.getConfigItem(value);
        return deviceItemModel;
    }

    public boolean doUpdateUserActivityLog() throws SQLException {
        List<UserActivityLogModel> uerActivityLogList = UserActivityLogRepository.getUnSyncedUserActivityLogs();
        return uerActivityLogList.size() > 0;
    }
}
