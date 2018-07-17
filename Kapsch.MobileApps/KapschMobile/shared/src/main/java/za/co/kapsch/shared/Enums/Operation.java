package za.co.kapsch.shared.Enums;

/**
 * Created by CSenekal on 2017/06/29.
 */
public enum Operation {
    Equals(0),
    GreaterThan(1),
    LessThan(2),
    GreaterThanOrEqual(3),
    LessThanOrEqual(4),
    Contains(5),
    StartsWith(6),
    EndsWith(7);

    private int mNumValue;

    Operation(int numValue){
        mNumValue = numValue;
    }

    public int getNumValue(){
        return mNumValue;
    }
}
