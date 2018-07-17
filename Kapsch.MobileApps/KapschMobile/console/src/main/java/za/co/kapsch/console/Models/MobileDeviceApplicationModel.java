package za.co.kapsch.console.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;

import za.co.kapsch.console.Enums.ApplicationStatus;
import za.co.kapsch.console.Enums.ApplicationType;

/**
 * Created by CSenekal on 2017/07/20.
 */

@DatabaseTable(tableName = "MobileDeviceApplication")
public class MobileDeviceApplicationModel {

    @DatabaseField(columnName = "ID", id = true)
    @SerializedName("ID")
    public long mID;

    @DatabaseField(columnName = "Name")
    @SerializedName("Name")
    public String mName;

    @DatabaseField(columnName = "SoftwareVersion")
    @SerializedName("SoftwareVersion")
    public String mSoftwareVersion;

    @DatabaseField(columnName = "ApplicationType")
    @SerializedName("ApplicationType")
    public ApplicationType mApplicationType;

    @DatabaseField(columnName = "Status")
    @SerializedName("Status")
    public ApplicationStatus mStatus;

    @DatabaseField(columnName = "ModifiedTimestamp")
    @SerializedName("ModifiedTimestamp")
    public Date mModifiedTimestamp;

    public long getID() {
        return mID;
    }

    public void setID(long ID) {
        mID = ID;
    }

    public String getName() {
        return mName;
    }

    public String getSoftwareVersion() {
        return mSoftwareVersion;
    }

    public ApplicationType getApplicationType() {
        return mApplicationType;
    }

    public ApplicationStatus getStatus() {
        return mStatus;
    }

    public Date getModifiedTimestamp() {
        return mModifiedTimestamp;
    }
}
