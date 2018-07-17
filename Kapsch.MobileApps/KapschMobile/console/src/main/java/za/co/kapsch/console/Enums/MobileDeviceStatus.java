package za.co.kapsch.console.Enums;

/**
 * Created by CSenekal on 2017/06/29.
 */
public enum MobileDeviceStatus {
    Inactive(0),
    Active(1),
    SuspendedLocked(2);

    private int mNumValue;

    MobileDeviceStatus(int numValue){
        mNumValue = numValue;
    }

    public int getNumValue(){
        return mNumValue;
    }
}
