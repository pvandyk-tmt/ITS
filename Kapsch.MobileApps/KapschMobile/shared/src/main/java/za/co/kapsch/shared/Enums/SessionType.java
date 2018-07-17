package za.co.kapsch.shared.Enums;

/**
 * Created by csenekal on 2016-09-08.
 */
public enum SessionType {
    SystemUser(0),
    ExternalUser1(1);

    private int mNumValue;

    SessionType(int numValue){
        mNumValue = numValue;
    }

    public int getmNumValue(){
        return mNumValue;
    }
}
