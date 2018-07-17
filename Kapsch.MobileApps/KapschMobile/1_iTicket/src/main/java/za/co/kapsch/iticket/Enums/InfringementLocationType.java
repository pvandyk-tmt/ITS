package za.co.kapsch.iticket.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/10/12.
 */

public enum InfringementLocationType {

    @SerializedName("0")
    Other(1);

    private final int code;

    InfringementLocationType(int code) {
        this.code = code;
    }
}
