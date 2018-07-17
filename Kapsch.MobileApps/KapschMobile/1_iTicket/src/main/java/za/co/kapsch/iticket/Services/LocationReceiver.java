package za.co.kapsch.iticket.Services;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.location.Location;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;

import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Constants;
import za.co.kapsch.iticket.DataServiceRequest;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.iticket.Google.GoogleGeoCodeReponse;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.Constants;

import static za.co.kapsch.shared.Constants.PROCESS_ID_GOOGLE_ADDRESS_SEARCH_BY_GPS;
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
                            saveLocation(location.getLatitude(), location.getLongitude());
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

    private void saveLocation(double latitude, double longitude){
        try {
            DataServiceRequest.googleAddressSearchRequest(this, null, Double.toString(latitude), Double.toString(longitude));
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "LocationReceiver::saveLocation()"), ErrorSeverity.High);
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
                case PROCESS_ID_GOOGLE_ADDRESS_SEARCH_BY_GPS:
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
