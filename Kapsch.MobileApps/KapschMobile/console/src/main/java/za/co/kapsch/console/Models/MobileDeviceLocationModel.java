package za.co.kapsch.console.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;

/**
 * Created by CSenekal on 2017/07/13.
 */
@DatabaseTable(tableName = "GpsLog")
public class MobileDeviceLocationModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", generatedId = true)
    private long mID;

    @SerializedName("GpsLatitude")
    @DatabaseField(columnName = "GpsLatitude")
    private Double mGpsLatitude;

    @SerializedName("GpsLongitude")
    @DatabaseField(columnName = "GpsLongitude")
    private Double mGpsLongitude;

    @SerializedName("MobileDeviceID")
    @DatabaseField(columnName = "MobileDeviceID")
    private long mMobileDeviceID;

    @SerializedName("LocationTimestamp")
    @DatabaseField(columnName = "LocationTimestamp")
    private Date mLocationTimestamp;

    @DatabaseField(columnName = "IsSynced")
    public transient boolean mIsSynced;

    @DatabaseField(columnName = "UploadDateTime")
    public Date mUploadDateTime;

    public Double getGpsLongitude() {
        return mGpsLongitude;
    }

    public void setGpsLongitude(Double gpsLongitude) {
        mGpsLongitude = gpsLongitude;
    }

    public Double getGpsLatitude() {
        return mGpsLatitude;
    }

    public void setGpsLatitude(Double gpsLatitude) {
        this.mGpsLatitude = gpsLatitude;
    }

    public long getMobileDeviceID() {
        return mMobileDeviceID;
    }

    public void setMobileDeviceID(long mobileDeviceID) {
       mMobileDeviceID = mobileDeviceID;
    }

    public Date getLocationTimestamp() {
        return mLocationTimestamp;
    }

    public void setLocationTimestamp(Date locationTimestamp) {
        mLocationTimestamp = locationTimestamp;
    }

    public Date getUploadDateTime() {
        return mUploadDateTime;
    }

    public void setUploadDateTime(Date uploadDateTime) {
        mUploadDateTime = uploadDateTime;
    }

    public boolean ismIsSynced() {
        return mIsSynced;
    }

    public void setIsSynced(boolean isSynced) {
        mIsSynced = isSynced;
    }

    public long getID() {

        return mID;
    }

    public void setID(long ID) {
        mID = ID;
    }
}
