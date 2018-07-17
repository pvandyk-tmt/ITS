package za.co.kapsch.console.Enums;

/**
 * Created by CSenekal on 2017/07/04.
 */
public enum UserStatus {

    Inactive(0),
    Active(1),
    SuspendedLocked(2);

    private int mNumValue;

    UserStatus(int numValue){
        mNumValue = numValue;
    }

    public int getNumValue(){
        return mNumValue;
    }
}
