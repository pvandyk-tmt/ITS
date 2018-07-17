package za.co.kapsch.iticket.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/06/02.
 */
public enum DistanceOverTimeServiceTypeEnum {

    @SerializedName("0")
    Socket(0),
    @SerializedName("1")
    Disk(1),
    @SerializedName("2")
    File(2);

    private final int code;

    DistanceOverTimeServiceTypeEnum(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }
}
