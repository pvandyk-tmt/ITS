package za.co.kapsch.console.General;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.support.v4.content.FileProvider;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.console.Enums.ApplicationType;
import za.co.kapsch.console.orm.DistrictRepository;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.console.Interfaces.IMessageCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.console.Models.ConfigItemModel;
import za.co.kapsch.console.Models.MobileDeviceApplicationModel;
import za.co.kapsch.console.Models.MobileDeviceLocationModel;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.PaginationListModel;
import za.co.kapsch.shared.Models.SystemFunctionModel;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.console.R;
import za.co.kapsch.console.orm.ConfigItemRepository;
import za.co.kapsch.console.orm.MobileDeviceLocationRepository;
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

    public DataSynchronisation(IMessageCallBack caller) throws Exception {

        mCaller = caller;
        mActivity = (Activity) caller;
    }

    public void processUpdates() throws Exception {

        if (centralConfigItems() == true) return;

        //if (centralMobileDeviceApplications() == true) return;

        if (downloadApks() == true) return;

        //if (updateConfigItems() == true) return;

        if (updateDistricts() == true) return;

        if (updateSystemFunctions() == true) return;

        if (updateUsers() == true) return;

        if (installApks() == true) return;

        if (updateGpsLogs() == true) return;

        if (updateUserActivityLog() == true) return;

        mCaller.message(Constants.FINISHED_MESSAGE, true);
    }

    public boolean centralConfigItems(){

        if (mConfigItemSynchroniser == null){
            DataServiceRequest.configItemRequest(this, mActivity);
            return true;
        }

        return false;
    }

//    public boolean centralMobileDeviceApplications(){
//
//        if (mConfigItemSynchroniser == null){
//            DataServiceRequest.mobileDeviceApplicationRequest(this, mActivity);
//            return true;
//        }
//
//        return false;
//    }

    public boolean downloadApks() throws SQLException {

        if (mConfigItemSynchroniser == null) return false;

        for(MobileDeviceApplicationModel mobileDeviceApplication : mConfigItemSynchroniser.getMobileDeviceApplicationList()) {
            int localAppVersionCode = Utilities.getInstalledAppVersion(mobileDeviceApplication.getName());
            int downloadedApkVersion = Utilities.getDownloadedApkVersion(String.format("%s.apk", mobileDeviceApplication.getName()));

            if (mConfigItemSynchroniser.doUpdateApk(mobileDeviceApplication.getName())) {
                mCaller.message(String.format(Utilities.getString(R.string.downloading_device_software), mobileDeviceApplication.getName()), true);
                DataServiceRequest.apkRequest(this, mActivity, String.format("%s%s",mobileDeviceApplication.getName(),".apk"), downloadedApkVersion > localAppVersionCode ? downloadedApkVersion : localAppVersionCode);
                return true;
            }
        }

        return false;
    }

    public boolean installApks() {

        for(MobileDeviceApplicationModel mobileDeviceApplication : mConfigItemSynchroniser.getMobileDeviceApplicationList()) {
            if (Utilities.installApk(mobileDeviceApplication.getName())){
                apkUpdateActivity(String.format("%s.apk",mobileDeviceApplication.getName()));
            }
        }

        return false;
    }

    private void apkUpdateActivity(String apkFilename){

        Intent intent = new Intent(Intent.ACTION_VIEW);

        intent.setDataAndType(Uri.fromFile(Utilities.getTicketFile(apkFilename)), "application/vnd.android.package-archive");
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
        mActivity.startActivityForResult(intent, Constants.INSTALL_APP_REQUEST_CODE);
    }

    private void apkUpdateActivityEx(String apkFilename){

        Intent intent = new Intent(Intent.ACTION_VIEW);

        //also view provider tag in AndroidManifest and also add provider_paths.xml
        Uri uri = FileProvider.getUriForFile(
                mActivity,
                mActivity.getApplicationContext().getApplicationContext().getPackageName() + ".za.co.kapsch.console.provider",
                Utilities.getTicketFile(apkFilename));

        intent.setDataAndType(uri, "application/vnd.android.package-archive");

        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
        mActivity.startActivityForResult(intent, Constants.INSTALL_APP_REQUEST_CODE);
    }

    private boolean updateUsers() throws SQLException {

        if (mConfigItemSynchroniser.doUpdateOfficers()) {
            mCaller.message(Utilities.getString(R.string.downloading_users), true);
            DataServiceRequest.usersRequest(this, mActivity);
            return true;
        }

        return false;
    }

//    private boolean updateDistrict() throws SQLException {
//
//        if (mConfigItemSynchroniser.doUpdateDistrict()) {
//            mCaller.message(Utilities.getString(R.string.downloading_district), true);
//            DataServiceRequest.districtRequest(this, mActivity, DistrictRepository.getDistrictID());
//            return true;
//        }
//
//        return false;
//    }

    private boolean updateDistricts() throws SQLException {

        if (mConfigItemSynchroniser.doUpdateDistricts()) {
            mCaller.message(Utilities.getString(R.string.downloading_districts), true);
            DataServiceRequest.districtsRequest(this, mActivity);
            return true;
        }

        return false;
    }

    private boolean updateSystemFunctions() throws SQLException {

        if (mConfigItemSynchroniser.doUpdateSystemFunctions()) {
            mCaller.message(Utilities.getString(R.string.downloading_system_functions), true);
            DataServiceRequest.systemFunctionRequest(this, mActivity);
            return true;
        }

        return false;
    }

//    private boolean updateMobileDeviceApplications(){
//
//        if (mConfigItemSynchroniser.doUpdateSystemFunctions()) {
//            mCaller.message(Utilities.getString(R.string.downloading_system_functions), true);
//            DataServiceRequest.mobileDeviceApplicationRequest(this, mActivity);
//            return true;
//        }
//
//        return false;
//    }

    private void queryForUpdates() throws Exception{

        if (mConfigItemSynchroniser != null) {
            if (mConfigItemSynchroniser.queryForUpdates()) {
                mCaller.message(Constants.SYNCHRONISATION_REQUIRED, false);
            } else {
                processUpdates();
            }
        }
    }


//    private boolean updateConfigItems() throws SQLException {
//
//        if (mConfigItemSynchroniser.doUpdateDeviceConfigItems()) {
//            mCaller.message(Utilities.getString(R.string.downloading_device_config), true);
//            DataServiceRequest.configItemRequest(this, mActivity);
//            return true;
//        }
//
//        return false;
//    }

//    private boolean getDlcSerializerRsaLic() throws SQLException {
//
//        if (mBackendSynchronisation.doGetDlcSerializerRsaLic()) {
//            mCaller.message(App.getContext().getResources().getString(R.string.data_service_requesting_dlc_serializer_rsa_licence), true);
//            DataServiceRequest.dlcSerializerRsaLicRequest(this, mActivity);
//            return true;
//        }
//
//        return false;
//    }

    private boolean updateGpsLogs() throws SQLException {

        if (mConfigItemSynchroniser.doUpdateGpsLogs()) {
            mCaller.message(Utilities.getString(R.string.uploading_gps_logs), true);
            List<MobileDeviceLocationModel> mobileDeviceLocationList = MobileDeviceLocationRepository.getGpsLogs();
            if (mobileDeviceLocationList.size() > 0) {
                DataServiceRequest.gpsLogUploadRequest(this, mActivity, mobileDeviceLocationList);
                return true;
            }
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

    private ConfigItemModel getConfigItem(String configItemName) throws SQLException{

        List<ConfigItemModel> configItemList = mConfigItemSynchroniser.getConfigItemList();

        for (ConfigItemModel configItem: configItemList) {
            if (configItem.getName().equals(configItemName)){

                int downloadedVersion = Utilities.getDownloadedApkVersion(String.format("%s.apk", configItem.getName()));
                int appVersion = Utilities.getInstalledAppVersion(configItemName);

                configItem.setValue(Integer.toString(appVersion > downloadedVersion ? appVersion : downloadedVersion));
                configItem.setID(ConfigItemRepository.getMaxID()+1);
                return configItem;
            }
        }

        return null;
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
                            //do not insert config items only update
                            //mConfigItemSynchroniser.insertDeviceConfiguration(mConfigItemSynchroniser.getConfigItemList());
                            DataServiceRequest.mobileDeviceApplicationListRequest(this, mActivity);
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_MOBILE_DEVICE_APPLICATION:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            getConfigItemSynchroniser().setMobileDeviceApplicationList((List<MobileDeviceApplicationModel>)((PaginationListModel)asyncResultModel.getObject()).getModels());//((List<MobileDeviceApplicationModel>)asyncResultModel.getObject());
                            mConfigItemSynchroniser.insertMobileDeviceApplication(mConfigItemSynchroniser.getMobileDeviceApplicationList());
                            queryForUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

//                case Constants.PROCESS_ID_DOWNLOAD_OFFICERS:
//                    switch (asyncResultModel.getProcessResult()) {
//                        case SUCCESS:
//                            List<UserModel> userList = (List<UserModel>)asyncResultModel.getObject();
//                            if (userList.size() == 0){
//                                mCaller.message(App.getContext().getResources().getString(R.string.user_list_is_empty), true);
//                                return;
//                            }
//                            mCaller.message(mConfigItemSynchroniser.insertOfficers(userList), true);
//                            processUpdates();
//                            return;
//                        case FAILED:
//                            mCaller.message(asyncResultModel.getMessage(), true);
//                            break;
//                    }
//                    break;

                case Constants.PROCESS_ID_DOWNLOAD_USERS:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            List<UserModel> userList = (List<UserModel>)asyncResultModel.getObject();
                            if (userList.size() == 0){
                                mCaller.message(App.getContext().getResources().getString(R.string.user_list_is_empty), true);
                                return;
                            }
                            mCaller.message(mConfigItemSynchroniser.insertOfficers(userList), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DOWNLOAD_APK:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DOWNLOAD_SYSTEM_FUNCTIONS:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            List<SystemFunctionModel> systemFunctions = ((PaginationListModel)asyncResultModel.getObject()).getModels();
                            if (systemFunctions.size() == 0){
                                mCaller.message(App.getContext().getResources().getString(R.string.system_functions_list_is_empty), true);
                                return;
                            }
                            mCaller.message(mConfigItemSynchroniser.insertSystemFunctions(systemFunctions), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_UPLOAD_GPS_LOGS:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            List<MobileDeviceLocationModel> mobileDeviceLocationList = (List<MobileDeviceLocationModel>)asyncResultModel.getObject();

                            for (MobileDeviceLocationModel mobileDeviceLocation: mobileDeviceLocationList) {
                                MobileDeviceLocationRepository.delete(mobileDeviceLocation);
                            }

                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
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

                case Constants.PROCESS_ID_DOWNLOAD_DISTRICT:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(mConfigItemSynchroniser.updateDistrict((DistrictModel) asyncResultModel.getObject()), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DOWNLOAD_DISTRICTS:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mCaller.message(mConfigItemSynchroniser.insertDistricts(((PaginationListModel)asyncResultModel.getObject()).getModels()), true);
                            processUpdates();
                            return;
                        case FAILED:
                            mCaller.message(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;

            }

            mCaller.message(Constants.FAILED_MESSAGE, true);
        }catch (Exception e) {
            mCaller.message(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), true);
            mCaller.message(Constants.FAILED_MESSAGE, true);
            return;
        }
    }
}
