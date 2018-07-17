package za.co.kapsch.shared.Services;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.PowerManager;

import za.co.kapsch.shared.LibApp;

/**
 * Created by csenekal on 2016-10-11.
 */
public class LocationAlarm extends BroadcastReceiver implements AutoCloseable {

    PowerManager.WakeLock mWakeLock;

    public LocationAlarm(){
        PowerManager powerManager = (PowerManager) LibApp.getContext().getSystemService(Context.POWER_SERVICE);
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
