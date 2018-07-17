package za.co.kapsch.shared.Models;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2018/04/25.
 */

public class PaymentOption {

    @SerializedName("Header")
    private String mHeader;
    @SerializedName("Details")
    private String mDetails;
    @SerializedName("Address")
    private String mAddress;

    public String getHeader() {
        return mHeader;
    }

    public String getDetails() {
        return mDetails;
    }

    public String getAddress() {
        return mAddress;
    }
}
