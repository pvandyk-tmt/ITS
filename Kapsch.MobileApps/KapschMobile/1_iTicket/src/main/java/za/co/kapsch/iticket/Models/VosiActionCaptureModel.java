package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;

/**
 * Created by CSenekal on 2017/09/07.
 */
@DatabaseTable(tableName = "VosiActionCapture")
public class VosiActionCaptureModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", generatedId = true)
    public long mID;

    @SerializedName("VosiActionID")
    @DatabaseField(columnName = "VosiActionID")
    public long mVosiActionID;

    @SerializedName("VLN")
    @DatabaseField(columnName = "VLN")
    public String mVLN;

    @SerializedName("LocationStreet")
    @DatabaseField(columnName = "LocationStreet")
    public String mLocationStreet;

    @SerializedName("LocationSuburb")
    @DatabaseField(columnName = "LocationSuburb")
    public String mLocationSuburb;

    @SerializedName("LocationTown")
    @DatabaseField(columnName = "LocationTown")
    public String mLocationTown;

    @SerializedName("LocationLatitude")
    @DatabaseField(columnName = "LocationLatitude")
    public Double mLocationLatitude;

    @SerializedName("Locationlongitude")
    @DatabaseField(columnName = "Locationlongitude")
    public Double mLocationlongitude;

    @SerializedName("Comments")
    @DatabaseField(columnName = "Comments")
    public String mComments;

    @SerializedName("CapturedDateTime")
    @DatabaseField(columnName = "CapturedDateTime")
    public Date mCapturedDateTime;

    @SerializedName("CapturedCredentialID")
    @DatabaseField(columnName = "CapturedCredentialID")
    public long mCapturedCredentialID;

    @DatabaseField(columnName = "Uploaded")
    private transient boolean mUploaded;

    public long getID() {
        return mID;
    }

    public void setID(long ID) {
        mID = ID;
    }

    public long getVosiActionID() {
        return mVosiActionID;
    }

    public void setVosiActionID(long vosiActionID) {
        mVosiActionID = vosiActionID;
    }

    public String getVLN() {
        return mVLN;
    }

    public void setVLN(String VLN) {
        mVLN = VLN;
    }

    public String getLocationStreet() {
        return mLocationStreet;
    }

    public void setLocationStreet(String locationStreet) {
        mLocationStreet = locationStreet;
    }

    public String getLocationSuburb() {
        return mLocationSuburb;
    }

    public void setLocationSuburb(String locationSuburb) {
        mLocationSuburb = locationSuburb;
    }

    public String getLocationTown() {
        return mLocationTown;
    }

    public void setLocationTown(String locationTown) {
        mLocationTown = locationTown;
    }

    public Double getLocationLatitude() {
        return mLocationLatitude;
    }

    public void setLocationLatitude(Double locationLatitude) {
        mLocationLatitude = locationLatitude;
    }

    public Double getLocationlongitude() {
        return mLocationlongitude;
    }

    public void setLocationlongitude(Double locationlongitude) {
        mLocationlongitude = locationlongitude;
    }

    public String getComments() {
        return mComments;
    }

    public void setComments(String comments) {
        mComments = comments;
    }

    public Date getCapturedDateTime() {
        return mCapturedDateTime;
    }

    public void setCapturedDateTime(Date capturedDateTime) {
        mCapturedDateTime = capturedDateTime;
    }

    public long getCapturedCredentialID() {
        return mCapturedCredentialID;
    }

    public void setCapturedCredentialID(long capturedCredentialID) {
        mCapturedCredentialID = capturedCredentialID;
    }

    public boolean isUploaded() {
        return mUploaded;
    }

    public void setUploaded(boolean uploaded) {
        mUploaded = uploaded;
    }
}
