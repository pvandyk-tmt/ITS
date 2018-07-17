package za.co.kapsch.console.Services;

import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.os.IBinder;
import android.os.PowerManager;
import android.support.v4.content.LocalBroadcastManager;

import za.co.kapsch.console.General.App;
import za.co.kapsch.console.General.Constants;
import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.console.Models.ConfigItemModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by pvdyk on 2016-10-11.
 */
public class LocationService extends Service {

    private static final int TWO_MINUTES = 1000 * 60 * 2;

    private Location currentBestLocation;
    private PowerManager.WakeLock mWakeLock;
    private LocationManager mLocationManager;

    @Override
    public void onCreate() {
        super.onCreate();

        PowerManager pm = (PowerManager) getSystemService(this.POWER_SERVICE);
        mWakeLock = pm.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "DoNotSleep");
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        try {
//            new ToggleGPS(getApplicationContext()).turnGPSOn();

            mLocationManager = (LocationManager) getApplicationContext().getSystemService(Context.LOCATION_SERVICE);
            mLocationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, ConfigItemModel.getInstance().getGpsInterval(), 1, mLocationListener);

        } catch (SecurityException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "LocationService::onHandleIntent()"), ErrorSeverity.High);
        }

        return Service.START_NOT_STICKY;
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    private LocationListener mLocationListener = new LocationListener() {
        @Override
        public void onLocationChanged(Location location) {

            if (location == null) {
                return;
            }

            boolean isBetter = isBetterLocation(location, currentBestLocation);

            if (!isBetter) {
                return;
            }

            currentBestLocation = location;

            Intent locationIntent = new Intent(za.co.kapsch.shared.Constants.LOCATION_ACTION);
            locationIntent.putExtra(za.co.kapsch.shared.Constants.LOCATION_RESULT, location);
            //LocalBroadcastManager.getInstance(App.getContext()).sendBroadcast(locationIntent);
            sendBroadcast(locationIntent);
        }

        @Override
        public void onProviderDisabled(String provider) {
        }

        @Override
        public void onProviderEnabled(String provider) {
        }

        @Override
        public void onStatusChanged(String provider, int status, Bundle extras) {
        }
    };

    @Override
    public void onDestroy() {
        try {
//            new ToggleGPS(getApplicationContext()).turnGPSOff();

            if (mWakeLock.isHeld()) {
                mWakeLock.release();
            }

            mLocationManager.removeUpdates(mLocationListener);
        } catch (SecurityException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "LocationService::onDestroy()"), ErrorSeverity.High);
        }
        super.onDestroy();
    }

    protected boolean isBetterLocation(Location location, Location currentBestLocation) {
        if (currentBestLocation == null) {
            // A new location is always better than no location
            return true;
        }

        // Check whether the new location fix is newer or older
        long timeDelta = location.getTime() - currentBestLocation.getTime();
        boolean isSignificantlyNewer = timeDelta > TWO_MINUTES;
        boolean isSignificantlyOlder = timeDelta < -TWO_MINUTES;
        boolean isNewer = timeDelta > 0;

        // If it's been more than two minutes since the current location, use the new location
        // because the user has likely moved
        if (isSignificantlyNewer) {
            return true;
            // If the new location is more than two minutes older, it must be worse
        } else if (isSignificantlyOlder) {
            return false;
        }

        // Check whether the new location fix is more or less accurate
        int accuracyDelta = (int) (location.getAccuracy() - currentBestLocation.getAccuracy());
        boolean isLessAccurate = accuracyDelta > 0;
        boolean isMoreAccurate = accuracyDelta < 0;
        boolean isSignificantlyLessAccurate = accuracyDelta > 200;

        // Check if the old and new location are from the same provider
        boolean isFromSameProvider = isSameProvider(location.getProvider(),
                currentBestLocation.getProvider());

        // Determine location quality using a combination of timeliness and accuracy
        if (isMoreAccurate) {
            return true;
        } else if (isNewer && !isLessAccurate) {
            return true;
        } else if (isNewer && !isSignificantlyLessAccurate && isFromSameProvider) {
            return true;
        }
        return false;
    }

    /** Checks whether two providers are the same */
    private boolean isSameProvider(String provider1, String provider2) {
        if (provider1 == null) {
            return provider2 == null;
        }
        return provider1.equals(provider2);
    }
}
