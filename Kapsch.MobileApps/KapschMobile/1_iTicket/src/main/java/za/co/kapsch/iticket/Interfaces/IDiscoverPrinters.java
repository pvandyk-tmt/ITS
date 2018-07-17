package za.co.kapsch.iticket.Interfaces;

import java.util.Map;

/**
 * Created by csenekal on 2016-09-02.
 */
public interface IDiscoverPrinters {
    void printerFoundCallBack(Map<String, String> printers);
    void discoveryFinishedCallBack();
    void discoveryErrorCallback();
}
