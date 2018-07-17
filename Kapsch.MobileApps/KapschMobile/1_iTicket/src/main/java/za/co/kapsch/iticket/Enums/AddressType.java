package za.co.kapsch.iticket.Enums;

/**
 * Created by csenekal on 2016-08-11.
 */
public enum AddressType {
    Physical(1),
    Postal(2),
    Business(3),
    Offence(4);

    private int mNumValue;

    AddressType(int numValue){
        mNumValue = numValue;
    }

    public static AddressType fromInteger(int x) {
        switch(x) {
            case 1 : return Physical;
            case 2 : return Postal;
            case 3 : return Business;
            case 4 : return Offence;
        }
        return null;
    }

    public static int toInteger(AddressType addressType) {
        switch(addressType) {
            case Physical : return 1;
            case Postal : return 2;
            case Business: return 3;
            case Offence: return 4;
        }
        return -1;
    }
}
