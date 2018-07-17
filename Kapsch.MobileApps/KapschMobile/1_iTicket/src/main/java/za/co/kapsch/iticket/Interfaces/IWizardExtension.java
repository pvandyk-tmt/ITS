package za.co.kapsch.iticket.Interfaces;

import android.view.View;

import java.util.List;

/**
 * Created by csenekal on 2016-10-25.
 */
public interface IWizardExtension {
    List<View> getViewList();
    void callBackMethod(int requestCode, Object object);
}
