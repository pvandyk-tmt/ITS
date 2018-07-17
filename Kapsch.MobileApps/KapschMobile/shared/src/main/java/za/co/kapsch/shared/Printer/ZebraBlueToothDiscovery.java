package za.co.kapsch.shared.Printer;

/**
 * Created by csenekal on 2016-08-22.
 */

import android.os.Looper;

import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.printer.discovery.BluetoothDiscoverer;
import com.zebra.sdk.printer.discovery.DiscoveryHandler;

import za.co.kapsch.shared.LibApp;
import za.co.kapsch.shared.Utilities;

public class ZebraBlueToothDiscovery{

    private DiscoveryHandler mDiscoveryHandler;

    public ZebraBlueToothDiscovery(DiscoveryHandler discoveryHandler) {

        mDiscoveryHandler =  discoveryHandler;
        discover();
    }

    private void discover() {

        new Thread(new Runnable() {
            public void run() {
                Looper.prepare();
                try {
                    BluetoothDiscoverer.findPrinters(LibApp.getContext(), mDiscoveryHandler);
                } catch (ConnectionException e) {
                    mDiscoveryHandler.discoveryError(Utilities.exceptionMessage(e, "ZebraBlueToothDiscovery::discover()"));
                } catch (Exception e) {
                    mDiscoveryHandler.discoveryError(Utilities.exceptionMessage(e, "ZebraBlueToothDiscovery::discover()"));
                } finally {
                    Looper.myLooper().quit();
                }
            }
        }).start();
    }
}
