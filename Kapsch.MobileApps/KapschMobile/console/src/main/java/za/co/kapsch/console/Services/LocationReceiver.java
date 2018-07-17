package za.co.kapsch.console.Services;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.location.Location;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;

import za.co.kapsch.shared.Constants;
import za.co.kapsch.console.General.DataServiceRequest;
import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.console.Google.GoogleGeoCodeReponse;
import za.co.kapsch.console.orm.MobileDeviceRepository;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.console.Models.InsertResultModel;
import za.co.kapsch.console.Models.MobileDeviceLocationModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.console.orm.MobileDeviceLocationRepository;

import static za.co.kapsch.shared.WebAccess.DataService.FAILED;
import static za.co.kapsch.shared.WebAccess.DataService.SUCCESS;

/**
 * Created by csenekal on 2016-10-17.
 */
public class LocationReceiver implements IAsyncProcessCallBack {

    //private int gpsUploadInterval = 1; //will upload gps on every

    private BroadcastReceiver mLocationReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            switch(intent.getAction()) {
                case Constants.LOCATION_ACTION : Location location = intent.getParcelableExtra(Constants.LOCATION_RESULT);
                    if (location.getProvider() == Constants.LOCATION_ERROR){
                        displayLocationServiceErrorMessage(location.getExtras().getString("error"));
                    }
                    else{
                        if ((SessionModel.getInstance().getLatitude() != location.getLatitude()) || (SessionModel.getInstance().getLongitude() != location.getLongitude())) {
                            SessionModel.getInstance().setLatitude(location.getLatitude());
                            SessionModel.getInstance().setLongitude(location.getLongitude());
                            postMobileDeviceLocation(location.getLatitude(), location.getLongitude());
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    };

    public BroadcastReceiver getReceiver(){
        return mLocationReceiver;
    }

    private void displayLocationServiceErrorMessage(String error){
        MessageManager.showMessage(error, ErrorSeverity.Medium);
    }

    private MobileDeviceLocationModel getMobileDeviceLocation(double latitude, double longitude){

        MobileDeviceLocationModel mobileDeviceLocationModel = new MobileDeviceLocationModel();
        mobileDeviceLocationModel.setLocationTimestamp(Calendar.getInstance().getTime());
        mobileDeviceLocationModel.setGpsLatitude(latitude);
        mobileDeviceLocationModel.setGpsLongitude(longitude);
        mobileDeviceLocationModel.setMobileDeviceID(MobileDeviceRepository.getID());

        return mobileDeviceLocationModel;
    }

    private void postMobileDeviceLocation(double latitude, double longitude){
        try {
            MobileDeviceLocationModel mobileDeviceLocationModel = getMobileDeviceLocation(latitude, longitude);

            MobileDeviceLocationRepository.Create(mobileDeviceLocationModel);

            List<MobileDeviceLocationModel> gpsLogs = new ArrayList<>();
            gpsLogs.add(mobileDeviceLocationModel);

            DataServiceRequest.gpsLogUploadRequest(this, null, gpsLogs);
            //DataServiceRequest.googleAddressSearchRequest(this, null, Double.toString(latitude), Double.toString(longitude));
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
        MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {

        if (asyncResultModel == null){
            return;
        }

        if (asyncResultModel.getObject() == null){
            return;
        }

        try {
            switch (asyncResultModel.getProcessId()) {
                case za.co.kapsch.console.General.Constants.PROCESS_ID_UPLOAD_GPS_LOGS:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            MobileDeviceLocationRepository.deleteLogs();
                            break;
                        case FAILED:
                            MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.Medium);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_GOOGLE_ADDRESS_SEARCH_BY_GPS:
                    GoogleGeoCodeReponse response = (GoogleGeoCodeReponse) asyncResultModel.getObject();
                    if (response != null){
                        if (response.results.length > 0) {
                            SessionModel.getInstance().setCurrentGpsAddress(response.results[0].formatted_address);
                        }
                    }

            }
        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), ErrorSeverity.High);
            return;
        }
    }
}
