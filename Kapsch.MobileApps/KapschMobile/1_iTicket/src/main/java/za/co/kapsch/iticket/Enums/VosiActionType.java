package za.co.kapsch.iticket.Enums;

import com.google.gson.annotations.SerializedName;

import za.co.kapsch.iticket.Constants;

/**
 * Created by csenekal on 2017/08/29.
 */
public enum VosiActionType {

    UnpaidInfringement(1),//PaymentIntent(1),
    Cancelled(2);

    private final int code;

    VosiActionType(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }
}
