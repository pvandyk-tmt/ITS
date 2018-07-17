package za.co.kapsch.shared.Printer;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by csenekal on 2016-08-26.
 */
public class PrintSection {

    private List<PrintItem> mPrintItems = new ArrayList<>();

    public void addPrintItem(PrintItem printItem) {
        mPrintItems.add(printItem);
    }

    public List<PrintItem> getPrintItems() {
        return mPrintItems;
    }
}
