package za.co.kapsch.console.General;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.console.Models.ConfigItemModel;
import za.co.kapsch.console.Models.MobileDeviceApplicationModel;
import za.co.kapsch.console.Models.MobileDeviceLocationModel;
import za.co.kapsch.console.orm.DistrictRepository;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.MobileDeviceModel;
import za.co.kapsch.console.orm.MobileDeviceRepository;
import za.co.kapsch.shared.Models.SystemFunctionModel;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.console.R;
import za.co.kapsch.console.orm.ConfigItemRepository;
import za.co.kapsch.console.orm.MobileDeviceApplicationRepository;
import za.co.kapsch.console.orm.MobileDeviceLocationRepository;
import za.co.kapsch.console.orm.SystemFunctionRepository;
import za.co.kapsch.console.orm.UserRepository;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.orm.UserActivityLogRepository;

/**
 * Created by CSenekal on 2017/01/26.
 */
public class ConfigItemSynchroniser {

    private static final String DISTRICT_VERSION = "DISTRICT_VERSION";
    private static final String OFFICERS_VERSION = "OFFICERS_VERSION";
    private static final String SYSTEM_FUNCTIONS_VERSION = "SYSTEM_FUNCTIONS_VERSION";
    private List<ConfigItemModel> mConfigItemList;
    private List<MobileDeviceApplicationModel> mMobileDeviceApplicationList;

    public ConfigItemSynchroniser() {
//        if (configItemList == null) {
//            throw new Exception("configItemList == null");
//        }
//
//        if (mobileDeviceApplicationList == null) {
//            throw new Exception("mobileDeviceApplicationList == null");
//        }
//
//        mConfigItemList = configItemList;
//        mMobileDeviceApplicationList = mobileDeviceApplicationList;
    }

    public void setConfigItemList(List<ConfigItemModel> configItemList){

         mConfigItemList = configItemList;
    }

    public void setMobileDeviceApplicationList(List<MobileDeviceApplicationModel> mobileDeviceApplicationList) {

        mMobileDeviceApplicationList = mobileDeviceApplicationList;
    }

    public boolean queryForUpdates() throws Exception {

        boolean updateRequired;

        updateRequired =
            (doUpdateDistricts()||
             doUpdateOfficers()||
             doUpdateSystemFunctions() ||
             doUpdateApks() ||
             doInstallApks());
             //doUpdateDeviceConfigItems());

        return updateRequired;
    }

    public boolean doUpdateOfficers() throws SQLException{

        String localOfficersVersion = localOfficersVersion();

        if (localOfficersVersion == null) return true;

        return !localOfficersVersion.equals(centralConfigItemValue(OFFICERS_VERSION));
    }

//    public boolean doUpdateDistrict() throws SQLException{
//
//        String localDistrictVersion = localDistrictVersion();
//
//        if (localDistrictVersion == null) return true;
//
//        return !localDistrictVersion.equals(centralConfigItemValue(DISTRICT_VERSION));
//    }

    public boolean doUpdateDistricts() throws SQLException{

        String localDistrictsVersion = localDistrictsVersion();

        if (localDistrictsVersion == null) return true;

        return !localDistrictsVersion.equals(centralConfigItemValue(DISTRICT_VERSION));
    }

    public boolean doUpdateSystemFunctions() throws SQLException{

        String localSystemFunctionsVersion = localSystemFunctionsVersion();

        if (localSystemFunctionsVersion == null) return true;

        return !localSystemFunctionsVersion.equals(centralConfigItemValue(SYSTEM_FUNCTIONS_VERSION));
    }

    public boolean doUpdateGpsLogs() throws SQLException {
        List<MobileDeviceLocationModel> gpsLogList = MobileDeviceLocationRepository.getGpsLogs();
        return gpsLogList.size() > 0;
    }

    public boolean doUpdateUserActivityLog() throws SQLException {
        List<UserActivityLogModel> uerActivityLogList = UserActivityLogRepository.getUnSyncedUserActivityLogs();
        return uerActivityLogList.size() > 0;
    }

    public boolean doUpdateApks() {

        if (mMobileDeviceApplicationList != null) {

            for (MobileDeviceApplicationModel mobileDeviceAplication : mMobileDeviceApplicationList) {

                if (mobileDeviceAplication.getName().contains(Constants.KAPSCH_ANDROID_PACKAGE_NAME)) {

                    if (doUpdateApk(mobileDeviceAplication.getName()) == true) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public boolean doUpdateApk(String applicationName) {

        int downloadedApkVersion = Utilities.getDownloadedApkVersion(String.format("%s.apk", applicationName));

        int localAppVersionCode = Utilities.getInstalledAppVersion(applicationName);

        if (downloadedApkVersion == -1){
            return localAppVersionCode < Integer.parseInt(centralMobileDeviceApplicationValue(applicationName));
        }

        if (downloadedApkVersion < Integer.parseInt(centralMobileDeviceApplicationValue(applicationName))){
            return true;
        }

        return false;
    }

    public boolean doInstallApks(){

        for(ConfigItemModel configItem : mConfigItemList) {

            if (configItem.getName().contains(Constants.KAPSCH_ANDROID_PACKAGE_NAME)) {

                int downloadedApkVersion = Utilities.getDownloadedApkVersion(String.format("%s.apk", configItem.getName()));

                int localAppVersionCode = Utilities.getInstalledAppVersion(configItem.getName());

                return localAppVersionCode < downloadedApkVersion;
            }
        }

        return  false;
    }

    public List<ConfigItemModel> getConfigItemList(){
        return mConfigItemList;
    }

    public List<MobileDeviceApplicationModel> getMobileDeviceApplicationList(){
        return mMobileDeviceApplicationList;
    }

//    public boolean doUpdateDeviceConfigItems(){
//        return true;
//        String deviceConfigurationVersion = deviceConfigurationVersion();
//
//        if (deviceConfigurationVersion == null) return true;
//
//        return !deviceConfigurationVersion.equals(backendDeviceConfigurationVersion());
//        return false;
//    }

    public String insertOfficers(List<UserModel> userList) throws Exception {

        if (userList == null) {
            return Utilities.getString(R.string.users_update_failed);
        }

        UserRepository.cleanInsert(userList);

        if (ConfigItemRepository.update(getLocalConfigItem(OFFICERS_VERSION, centralConfigItemValue(OFFICERS_VERSION))) == false) {
            return Utilities.getString(R.string.users_update_failed);
        }

        return Utilities.getString(R.string.users_updated);
    }

    public String insertDistricts(List<DistrictModel> districtList) throws Exception {

        if (districtList == null) {
            return Utilities.getString(R.string.downloading_districts_failed);
        }

        DistrictRepository.cleanInsert(districtList);

        if (ConfigItemRepository.update(getLocalConfigItem(DISTRICT_VERSION, centralConfigItemValue(DISTRICT_VERSION))) == false) {
            return Utilities.getString(R.string.downloading_districts_failed);
        }

        return Utilities.getString(R.string.downloading_districts_successful);
    }

    public String insertSystemFunctions(List<SystemFunctionModel> systemFunctions) throws Exception {

        if (systemFunctions == null) {
            return Utilities.getString(R.string.system_functions_update_failed);
        }

        SystemFunctionRepository.cleanInsert(systemFunctions);

        if (ConfigItemRepository.update(getLocalConfigItem(SYSTEM_FUNCTIONS_VERSION, centralConfigItemValue(SYSTEM_FUNCTIONS_VERSION))) == false) {
            return Utilities.getString(R.string.system_functions_update_failed);
        }

        return Utilities.getString(R.string.system_functions_updated);
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

    public String updateDistrict(DistrictModel district){

        try{
            DistrictRepository.update(district);

            if (ConfigItemRepository.update(getLocalConfigItem(DISTRICT_VERSION, centralConfigItemValue(DISTRICT_VERSION))) == false) {
                return Utilities.getString(R.string.downloading_district_failed);
            }

            return App.getContext().getResources().getString(R.string.downloading_district_successful);

        } catch (SQLException e) {
            return String.format("%s : %s", App.getContext().getResources().getString(R.string.downloading_district_failed), Utilities.exceptionMessage(e, null));
        }
    }

    public String insertMobileDevice(MobileDeviceModel mobileDevice) throws Exception {

        if (mobileDevice == null) {
            return Utilities.getString(R.string.system_functions_update_failed);
        }

        MobileDeviceRepository.create(mobileDevice);

        return Utilities.getString(R.string.system_functions_updated);
    }

    public String insertDeviceConfiguration(ConfigItemModel configItem) throws Exception {

        if (configItem == null) {
            return Utilities.getString(R.string.device_config_update_failed);
        }

        ConfigItemModel localConfigItem = ConfigItemRepository.getConfigItem(configItem.getName());

        if (localConfigItem != null){
            //localConfigItem.setValue(configItem.getValue());
            //ConfigItemRepository.update(localConfigItem);
        }else {
           configItem.setID(ConfigItemRepository.getMaxID()+1);
           ConfigItemRepository.insert(configItem);
        }

        ConfigItemModel.getInstance().refreshConfigurationValues();

        return Utilities.getString(R.string.device_config_updated);
    }

//    public String insertDeviceConfiguration(List<ConfigItemModel> configItemModelList) throws Exception {
//
//        if (configItemModelList == null) {
//            return Utilities.getString(R.string.device_config_update_failed);
//        }
//
//        for (ConfigItemModel configItem :configItemModelList) {
//            insertDeviceConfiguration(configItem);
//        }
//
//        ConfigItemModel.getInstance().refreshConfigurationValues();
//
//        return Utilities.getString(R.string.device_config_updated);
//    }

    public String insertMobileDeviceApplication(List<MobileDeviceApplicationModel> mobileDeviceApplicationList) throws Exception {

        if (mobileDeviceApplicationList == null) {
            return Utilities.getString(R.string.device_config_update_failed);
        }

        for (MobileDeviceApplicationModel mobileDeviceApplication :mobileDeviceApplicationList) {
            insertMobileDeviceApplication(mobileDeviceApplication);
        }

        ConfigItemModel.getInstance().refreshConfigurationValues();

        return Utilities.getString(R.string.device_config_updated);
    }

    public String insertMobileDeviceApplication(MobileDeviceApplicationModel mobileDeviceApplication) throws Exception {

        if (mobileDeviceApplication == null) {
            return Utilities.getString(R.string.device_config_update_failed);
        }

        MobileDeviceApplicationModel localMobileDeviceApplicationModel = MobileDeviceApplicationRepository.getMobileDeviceApplication(mobileDeviceApplication.getName());

        if (localMobileDeviceApplicationModel != null){
            //localConfigItem.setValue(configItem.getValue());
            //ConfigItemRepository.update(localConfigItem);
        }else {
            mobileDeviceApplication.setID(MobileDeviceApplicationRepository.getMaxID()+1);
            MobileDeviceApplicationRepository.insert(mobileDeviceApplication);
        }

        return Utilities.getString(R.string.device_config_updated);
    }

    public String centralConfigItemValue(String name){

        for(ConfigItemModel configItem: mConfigItemList){

            if (configItem.getName().equals(name)) {
                return configItem.getValue();
            }
        }

        return null;
    }

    public String centralMobileDeviceApplicationValue(String name){

        for(MobileDeviceApplicationModel mobileDeviceApplication: mMobileDeviceApplicationList){

            if (mobileDeviceApplication.getName().equals(name)) {
                return mobileDeviceApplication.getSoftwareVersion();
            }
        }

        return null;
    }

//    private List<ConfigItemModel> getConfigItemList() {
//
//        try{
//            return ConfigItemRepository.getAll();
//        }catch(SQLException e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "getDeviceItemList()"), ErrorSeverity.High);
//            return null;
//        }
//    }

    private String localOfficersVersion() throws SQLException {

        ConfigItemModel deviceItemModel = ConfigItemRepository.getConfigItem(OFFICERS_VERSION);
        if (deviceItemModel != null) {
            return deviceItemModel.getValue();
        }
        return null;
    }

//    private String localDistrictVersion() throws SQLException {
//
//        ConfigItemModel deviceItemModel = ConfigItemRepository.getConfigItem(DISTRICT_VERSION);
//        if (deviceItemModel != null) {
//            return deviceItemModel.getValue();
//        }
//        return null;
//    }

    private String localDistrictsVersion() throws SQLException {

        ConfigItemModel deviceItemModel = ConfigItemRepository.getConfigItem(DISTRICT_VERSION);
        if (deviceItemModel != null) {
            return deviceItemModel.getValue();
        }
        return null;
    }

    private String localSystemFunctionsVersion() throws SQLException {

        ConfigItemModel deviceItemModel = ConfigItemRepository.getConfigItem(SYSTEM_FUNCTIONS_VERSION);
        if (deviceItemModel != null) {
            return deviceItemModel.getValue();
        }
        return null;
    }

    private String localMobileDeviceApplicationVersion() throws SQLException {

        MobileDeviceApplicationModel mobileDeviceApplication = MobileDeviceApplicationRepository.getMobileDeviceApplication(SYSTEM_FUNCTIONS_VERSION);
        if (mobileDeviceApplication != null) {
            return mobileDeviceApplication.getSoftwareVersion();
        }
        return null;
    }

//    private String dlcSerializerRsaLic(){
//        DeviceItemModel deviceItem = findDeviceItem(Constants.DLC_SERIALIZER_RSA_LIC);
//
//        //save dlcSerializerRsaLic for later use;
//        if (deviceItem != null) {
//            SessionModel.getInstance().setDlcSerializerRsaLic(deviceItem.getValue());
//            return deviceItem.getValue();
//        }
//
//        return null;
//    }

    private ConfigItemModel getLocalConfigItem(String name, String value) throws SQLException{

        ConfigItemModel deviceItem = ConfigItemRepository.getConfigItem(name);
        deviceItem.setValue(value);
        return deviceItem;
    }
}
