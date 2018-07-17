package za.co.kapsch.shared.Enums;

/**
 * Created by CSenekal on 2017/06/29.
 */
public enum FilterJoin {

    And(0),
    Or(1);

    private int mNumValue;

    FilterJoin(int numValue){
        mNumValue = numValue;
    }

    public int getNumValue(){
        return mNumValue;
    }
}
