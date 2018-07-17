package za.co.kapsch.shared.Models;

/**
 * Created by CSenekal on 2017/06/29.
 */

import com.google.gson.annotations.SerializedName;

public class FilterModel {

    @SerializedName("PropertyName")
    private String mPropertyName;
    @SerializedName("Operation")
    private int mOperation;
    @SerializedName("Value")
    private Object mValue;

    public void setPropertyName(String propertyName) {
        mPropertyName = propertyName;
    }

    public void setOperation(int operation) {
        mOperation = operation;
    }

    public void setValue(Object value) {
        mValue = value;
    }
}
