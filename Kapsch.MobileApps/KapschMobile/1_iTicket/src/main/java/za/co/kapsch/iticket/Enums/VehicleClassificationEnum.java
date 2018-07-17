package za.co.kapsch.iticket.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/05/23.
 */
public enum VehicleClassificationEnum {

    @SerializedName("0")
    Light(0),
    @SerializedName("1")
    Heavy(1),
    @SerializedName("2")
    PublicTransport(2);

    private final int code;

    VehicleClassificationEnum(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }
}
