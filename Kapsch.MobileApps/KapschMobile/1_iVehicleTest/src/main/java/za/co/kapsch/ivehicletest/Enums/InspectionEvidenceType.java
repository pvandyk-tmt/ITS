package za.co.kapsch.ivehicletest.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by csenekal on 2016-10-20.
 */
public enum InspectionEvidenceType {

    @SerializedName("1")
    VehiclePhoto(1);

    private final int code;

    InspectionEvidenceType(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }
}
