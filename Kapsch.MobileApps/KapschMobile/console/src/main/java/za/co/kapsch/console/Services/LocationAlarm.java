package za.co.kapsch.console.Services;

import android.app.AlarmManager;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.PowerManager;
import android.support.v4.content.LocalBroadcastManager;

import za.co.kapsch.console.General.App;
import za.co.kapsch.console.General.Constants;
import za.co.kapsch.console.Models.ConfigItemModel;

/**
 * Created by csenekal on 2016-10-11.
 */
public class LocationAlarm extends BroadcastReceiver implements AutoCloseable {

    PowerManager.WakeLock mWakeLock;

    public LocationAlarm(){
        PowerManager powerManager = (PowerManager) App.getContext().getSystemService(Context.POWER_SERVICE);
        mWakeLock = powerManager.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "DoNotSleep");
        mWakeLock.acquire();
    }

    @Override
    public void onReceive(Context context, Intent intent)
    {

    }

    @Override
    public void close() {
        if (mWakeLock.isHeld()) {
            mWakeLock.release();
        }
    }
}
