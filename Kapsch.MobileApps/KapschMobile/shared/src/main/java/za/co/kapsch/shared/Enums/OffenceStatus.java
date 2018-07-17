package za.co.kapsch.shared.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/09/05.
 */
public enum OffenceStatus {

    @SerializedName("0")
    Open(0),

    @SerializedName("1")
    Cancelled(1),

    @SerializedName("2")
    Paid(2);

    private final int code;

    OffenceStatus(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }
}


