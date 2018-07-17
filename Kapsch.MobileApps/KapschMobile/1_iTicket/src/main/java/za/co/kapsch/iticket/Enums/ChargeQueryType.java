package za.co.kapsch.iticket.Enums;

/**
 * Created by csenekal on 2016-08-15.
 */
public enum ChargeQueryType {
    DriversLicence,
    VehicleLicence,
    StopSign,
    TrafficSign,
    Seatbelt,
    Tyre,
    Favourites,
    Code,
    Cellular,
    Roadworthy,
    Description,
    All;

    public static ChargeQueryType fromInteger(int x) {
        switch(x) {
            case 0 : return DriversLicence;
            case 1 : return VehicleLicence;
            case 2 : return StopSign;
            case 3 : return TrafficSign;
            case 4 : return Seatbelt;
            case 5 : return Tyre;
            case 6 : return Favourites;
            case 7 : return Code;
            case 8 : return Cellular;
            case 9 : return Roadworthy;
            case 10 : return Description;
            case 11: return All;
        }
        return null;
    }

    public static int toInteger(ChargeQueryType chargeQueryType) {
        switch(chargeQueryType) {
            case DriversLicence : return 0;
            case VehicleLicence : return 1;
            case StopSign : return 2;
            case TrafficSign : return 3;
            case Seatbelt : return 4;
            case Tyre : return 5;
            case Favourites : return 6;
            case Code : return 7;
            case Cellular : return 8;
            case Roadworthy : return 9;
            case Description : return 10;
            case All : return 11;
        }
        return -1;
    }
}
