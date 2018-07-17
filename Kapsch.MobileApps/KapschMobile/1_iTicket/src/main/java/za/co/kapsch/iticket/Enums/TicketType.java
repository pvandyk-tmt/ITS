package za.co.kapsch.iticket.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by csenekal on 2016-09-19.
 */
public enum TicketType {
    @SerializedName("0")Unknown(0),
    @SerializedName("56")Section56(56),
    @SerializedName("341")Section341(341),
    @SerializedName("44")OpusSection56(44),
    @SerializedName("43")OpusSection341(43);

    private int mNumValue;

    TicketType(int numValue){
        mNumValue = numValue;
    }

    public int getNumValue(){
        return mNumValue;
    }

    public static TicketType fromInteger(int x) {
        switch(x) {
            case 0 : return Unknown;
            case 56 : return Section56;
            case 341 : return Section341;
            case 43 : return OpusSection341;
            case 44 : return OpusSection56;
        }
        return null;
    }

    public static int toInteger(TicketType ticketType) {
        switch(ticketType) {
            case Unknown: return 0;
            case Section56 : return 56;
            case Section341 : return 341;
            case OpusSection56 : return 44;
            case OpusSection341 : return 43;
        }
        return -1;
    }
}
