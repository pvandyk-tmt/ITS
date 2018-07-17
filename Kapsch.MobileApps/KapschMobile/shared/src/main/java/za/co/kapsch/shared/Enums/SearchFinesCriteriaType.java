package za.co.kapsch.shared.Enums;

import za.co.kapsch.shared.R;
import za.co.kapsch.shared.LibApp;

/**
 * Created by csenekal on 2017/08/23.
 */
public enum SearchFinesCriteriaType {

    ID(0),
    VLN(1),
    RefNumber(2),
    TransactionToken(3),
    Unknown(4);

    private final int code;

    SearchFinesCriteriaType(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }

//    private String mFriendlyName;
//
//    SearchFinesCriteriaType(String friendlyName){
//        mFriendlyName = friendlyName;
//    }
//
//    @Override
//    public String toString() {
//        return mFriendlyName;
//    }
//
//    public static SearchFinesCriteriaType fromString(String value){
//
//        if (value.equals(LibApp.getContext().getString(R.string.id))) return SearchFinesCriteriaType.ID;
//        else if (value.equals(LibApp.getContext().getString(R.string.vln))) return SearchFinesCriteriaType.VLN;
//        return SearchFinesCriteriaType.RefNumber;
//    }
//
//    public static SearchFinesCriteriaType fromInt(int x) throws Exception {
//
//        switch((int)x) {
//            case 0 : return ID;
//            case 1 : return VLN;
//            case 2 : return RefNumber;
//            default: throw new Exception("SearchFinesCriteriaType Invalid value");
//        }
//    }
//
//    public static int toInt(SearchFinesCriteriaType identificationType) throws Exception {
//
//        switch(identificationType) {
//            case ID: return 0;
//            case VLN : return 1;
//            case RefNumber: return 2;
//            default: throw new Exception("SearchFinesCriteriaType Invalid value");
//        }
//    }
}
