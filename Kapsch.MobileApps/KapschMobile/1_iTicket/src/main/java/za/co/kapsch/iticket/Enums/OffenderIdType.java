package za.co.kapsch.iticket.Enums;

import za.co.kapsch.iticket.Constants;

/**
 * Created by CSenekal on 2017/04/20.
 */
public enum OffenderIdType {

    //Select(Constants.SELECT),
    NoID(Constants.ID_TYPE_NO_ID),
    RSA(Constants.ID_TYPE_RSA),
    Passport(Constants.ID_TYPE_PASSPORT);

    private String friendlyName;

    OffenderIdType(String friendlyName){
        this.friendlyName = friendlyName;
    }

    @Override
    public String toString(){
        return friendlyName;
    }

    public static String toString(OffenderIdType offenderIdType) {

        switch(offenderIdType) {
            case RSA : return Constants.ID_TYPE_RSA;
            case NoID : return Constants.ID_TYPE_NO_ID;
            case Passport : return Constants.ID_TYPE_PASSPORT;
            //case Select: return Constants.SELECT;
        }
        return null;
    }

    public static OffenderIdType fromString(String value){

       if (value.equals(Constants.ID_TYPE_RSA)) return OffenderIdType.RSA;
       else if (value.equals(Constants.ID_TYPE_NO_ID)) return OffenderIdType.NoID;
       else if (value.equals(Constants.ID_TYPE_PASSPORT)) return OffenderIdType.Passport;
       //else if (value.equals(Constants.SELECT)) return OffenderIdType.Select;

       return OffenderIdType.NoID;
    }
}
