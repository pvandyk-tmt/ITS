package za.co.kapsch.iticket.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/05/23.
 */
public enum DirectionEnum {

    @SerializedName("0")
    Towards(0),
    @SerializedName("1")
    Away(1);

    private final int code;

    DirectionEnum(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }
}
