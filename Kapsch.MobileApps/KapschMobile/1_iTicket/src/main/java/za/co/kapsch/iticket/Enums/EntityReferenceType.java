package za.co.kapsch.iticket.Enums;

/**
 * Created by CSenekal on 2017/08/04.
 */
public enum EntityReferenceType {

    TrafficViolations(1);

    private int mNumValue;

    EntityReferenceType(int numValue){
        mNumValue = numValue;
    }

    public int getNumValue(){
        return mNumValue;
    }

    public static EntityReferenceType fromInteger(int x) {
        switch(x) {
            case 1 : return TrafficViolations;
        }
        return null;
    }

    public static int toInteger(EntityReferenceType entityReferenceType) {
        switch(entityReferenceType) {
            case TrafficViolations: return 1;
        }
        return -1;
    }
}
