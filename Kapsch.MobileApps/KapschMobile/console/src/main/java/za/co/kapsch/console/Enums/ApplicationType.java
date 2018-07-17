package za.co.kapsch.console.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/07/20.
 */
public enum ApplicationType {
    @SerializedName("0")
    None,
    @SerializedName("1")
    Shared,
    @SerializedName("2")
    Standalone,
    @SerializedName("3")
    InConsole
}
