package za.co.kapsch.ivehicletest;

import android.support.v4.app.Fragment;

import za.co.kapsch.ivehicletest.Interfaces.IFinalWizardPage;

/**
 * Created by CSenekal on 2017/12/11.
 */

public class WizardPage extends Fragment{

    protected long mNextPageID;

    public long getNextPageID(){
        return mNextPageID;
    }

    public boolean validateNextPage()  { return true; }

    public void beforeCancel(){}
}
