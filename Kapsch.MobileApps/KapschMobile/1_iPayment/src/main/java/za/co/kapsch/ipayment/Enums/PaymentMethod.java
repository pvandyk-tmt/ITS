package za.co.kapsch.ipayment.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/09/08.
 */
public enum PaymentMethod {

    @SerializedName("0")
    None(0),

    @SerializedName("1")
    Cash(1),

    @SerializedName("2")
    DumaPay(2);

    private final int code;

    PaymentMethod(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }

    public static PaymentMethod fromInt(int x) throws Exception{
        switch((int)x) {
            case 0 : return None;
            case 1 : return Cash;
            case 2 : return DumaPay;
            default: throw new Exception("PaymentMethod: Invalid value");
        }
    }
}
