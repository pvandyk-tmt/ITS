package za.co.kapsch.ipayment.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/09/08.
 */
public enum PaymentTransactionStatus {

    @SerializedName("0")
    Added(0),

    @SerializedName("1")
    Settled(1),

    @SerializedName("2")
    Processed(2),

    @SerializedName("3")
    Cancelled(3);

    private final int mCode;

    PaymentTransactionStatus(int code) {
        mCode = code;
    }

    public int getCode() {
        return mCode;
    }

    public static PaymentTransactionStatus fromCode(int code) throws Exception{

        switch (code){
            case 0: return Added;
            case 1: return Settled;
            case 2: return Processed;
            case 3: return Cancelled;
            default: throw new Exception("Invalid value");
        }
    }
}
