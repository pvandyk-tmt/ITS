package za.co.kapsch.ipayment.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/09/11.
 */
public enum CustomerCreditType {

    @SerializedName("0")
    DumaPay(0),

    @SerializedName("1")
    Cash(1);

    private final int code;

    CustomerCreditType(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }

}
