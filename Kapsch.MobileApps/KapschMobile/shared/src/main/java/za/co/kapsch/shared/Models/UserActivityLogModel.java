package za.co.kapsch.shared.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;

/**
 * Created by csenekal on 2017/11/21.
 */

@DatabaseTable(tableName = "UserActivityLog")
public class UserActivityLogModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", generatedId = true)
    public long mID;

    @SerializedName("DeviceID")
    @DatabaseField(columnName = "DeviceID")
    public String mDeviceID;

    @SerializedName("CredentialID")
    @DatabaseField(columnName = "CredentialID")
    public Long mCredentialID;

    @SerializedName("CreatedTimestamp")
    @DatabaseField(columnName = "CreatedTimestamp")
    public Date mCreatedTimestamp;

    @SerializedName("Category")
    @DatabaseField(columnName = "Category")
    public String mCategory;

    @SerializedName("ActionDescription")
    @DatabaseField(columnName = "ActionDescription")
    public String mActionDescription;

    @DatabaseField(columnName = "Uploaded")
    public transient boolean mUploaded;

    public Long getCredentialID() {
        return mCredentialID;
    }

    public void setCredentialID(Long credentialID) {
        mCredentialID = credentialID;
    }

    public Date getCreatedTimestamp() {
        return mCreatedTimestamp;
    }

    public void setCreatedTimestamp(Date createdTimestamp) {
        mCreatedTimestamp = createdTimestamp;
    }

    public String getCategory() {
        return mCategory;
    }

    public void setCategory(String category) {
        mCategory = category;
    }

    public String getActionDescription() {
        return mActionDescription;
    }

    public void setActionDescription(String actionDescription) {
        mActionDescription = actionDescription;
    }

    public void setUploaded(boolean uploaded){
        mUploaded = uploaded;
    }

    public String getDeviceID() {
        return mDeviceID;
    }

    public void setDeviceID(String deviceID) {
        this.mDeviceID = deviceID;
    }
}
