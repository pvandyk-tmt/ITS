package za.co.kapsch.console.Models;

import com.google.gson.annotations.SerializedName;

/**
 * Created by csenekal on 2016-10-14.
 */
public class InsertResultModel {

    @SerializedName("d")
    private long mId;

    @SerializedName("Message")
    private String mMessage;

    public String getMessage() {
        return mMessage;
    }

    public void setMessage(String message) {
        mMessage = message;
    }

    public long getId() {
        return mId;
    }

    public void setId(long id) {
        mId = id;
    }
}
