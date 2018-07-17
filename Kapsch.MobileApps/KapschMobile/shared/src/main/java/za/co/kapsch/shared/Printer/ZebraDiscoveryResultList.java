package za.co.kapsch.shared.Printer;

import android.app.Activity;

import com.zebra.sdk.printer.discovery.DiscoveredPrinter;
import com.zebra.sdk.printer.discovery.DiscoveryHandler;

import java.util.List;
import java.util.Map;

import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Enums.ErrorSeverity;

/**
 * Created by csenekal on 2016-08-22.
 */
public class ZebraDiscoveryResultList implements DiscoveryHandler {

    private Activity mActivity;
    private List<Map<String, String>> printerList;// = new ArrayList<Map<String, String>>();

    public ZebraDiscoveryResultList(Activity activity){
        mActivity = activity;
    }

    @Override
    public void foundPrinter(DiscoveredPrinter discoveredPrinter) {
        MessageManager.showMessage(discoveredPrinter.address, ErrorSeverity.None);
        //Toast.makeText(mActivity, discoveredPrinter.address, Toast.LENGTH_SHORT).show();

        Map<String, String> printer = discoveredPrinter.getDiscoveryDataMap();
        printerList.add(printer);

    }

    @Override
    public void discoveryFinished() {
        //Toast.makeText(mActivity, "discoveryFinished", Toast.LENGTH_SHORT).show();
        //mDiscoverPrinters.discoveryFinishedCallBack();
    }

    @Override
    public void discoveryError(String s) {

    }
}
