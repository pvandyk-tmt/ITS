package za.co.kapsch.console.Enums;

/**
 * Created by csenekal on 2016-08-11.
 */
public enum AddressType {
    Postal,
    Offence,
    Business,
    Home;

    public static AddressType fromInteger(int x) {
        switch(x) {
            case 0 : return Postal;
            case 1 : return Offence;
            case 2 : return Business;
            case 3 : return Home;
        }
        return null;
    }

    public static int toInteger(AddressType addressType) {
        switch(addressType) {
            case Postal : return 0;
            case Offence : return 1;
            case Business: return 2;
            case Home: return 3;
        }
        return -1;
    }
}
