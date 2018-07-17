package za.co.kapsch.iticket.Enums;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.R;

/**
 * Created by csenekal on 2017/08/10.
 */
public enum IdentificationType {

    Unknown(App.getContext().getString(R.string.unknown)),
    ZambianID(App.getContext().getString(R.string.zambian_id)),
    ZambianPassport(App.getContext().getString(R.string.zambian_passport)),
    ForeignIdDocument(App.getContext().getString(R.string.foreign_id_document)),
    ForeignPassport(App.getContext().getString(R.string.foreign_Passport)),
    ZambianDriversCard(App.getContext().getString(R.string.zambian_drivers_card));

    private String mFriendlyName;

    IdentificationType(String friendlyName){
        mFriendlyName = friendlyName;
    }

    @Override
    public String toString() {
        return mFriendlyName;
    }

    public static IdentificationType fromString(String value){

        if (value.equals(App.getContext().getString(R.string.unknown))) return IdentificationType.Unknown;
        else if (value.equals(App.getContext().getString(R.string.zambian_id))) return IdentificationType.ZambianID;
        else if (value.equals(App.getContext().getString(R.string.zambian_passport))) return IdentificationType.ZambianPassport;
        else if (value.equals(App.getContext().getString(R.string.foreign_id_document))) return IdentificationType.ForeignIdDocument;
        else if (value.equals(App.getContext().getString(R.string.foreign_Passport))) return IdentificationType.ForeignPassport;
        else if (value.equals(App.getContext().getString(R.string.zambian_drivers_card))) return IdentificationType.ZambianDriversCard;

        return IdentificationType.Unknown;
    }

    public static IdentificationType fromLong(long x) {
        switch((int)x) {
            case 0 : return Unknown;
            case 1 : return ZambianID;
            case 2 : return ZambianPassport;
            case 3 : return ForeignIdDocument;
            case 4 : return ForeignPassport;
            case 5 : return ZambianDriversCard;
        }
        return null;
    }

    public static long toLong(IdentificationType identificationType) {
        switch(identificationType) {
            case Unknown: return 0;
            case ZambianID : return 1;
            case ZambianPassport : return 2;
            case ForeignIdDocument: return 3;
            case ForeignPassport: return 4;
            case ZambianDriversCard: return 5;
        }
        return -1;
    }
}
