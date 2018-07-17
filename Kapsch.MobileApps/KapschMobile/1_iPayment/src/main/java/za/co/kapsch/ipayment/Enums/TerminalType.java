package za.co.kapsch.ipayment.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/09/08.
 */
public enum TerminalType {

    @SerializedName("0")
    None(0),

    @SerializedName("1")
    InternalServer(1),

    @SerializedName("2")
    InternalPC (2),

    @SerializedName("3")
    InternalMobileDevice(3),

    @SerializedName("4")
    ExternalServer(4),

    @SerializedName("5")
    ExternalPC(5),

    @SerializedName("6")
    ExternalMobileDevice(6);

    private final int code;

    TerminalType(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }
}
