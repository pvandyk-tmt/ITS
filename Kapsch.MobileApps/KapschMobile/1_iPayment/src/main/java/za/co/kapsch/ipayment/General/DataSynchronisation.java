package za.co.kapsch.ipayment.General;

import android.app.Activity;
import android.support.v4.app.Fragment;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.ipayment.Models.ConfigItemModel;
import za.co.kapsch.ipayment.R;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Interfaces.IMessageCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
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
    }

    public void processUpdates() throws Exception {

        if (centralConfigItems() == true) return;

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

    private boolean updateUserActivityLog() throws SQLException {

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
                            mConfigItemSynchroniser.updateConfigItems((List<ConfigItemModel>)asyncResultModel.getObject());
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

            }

            mCaller.message(Constants.FAILED_MESSAGE, true);
        }catch (Exception e) {
            mCaller.message(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), true);
            mCaller.message(Constants.FAILED_MESSAGE, true);
            return;
        }
    }
}
