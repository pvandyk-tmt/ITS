package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import za.co.kapsch.iticket.Enums.InfringementLocationType;
/**
 * Created by CSenekal on 2017/10/12.
 */

@DatabaseTable(tableName = "InfringementLocation")
public class InfringementLocationModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    public long mID;

    @SerializedName("Code")
    @DatabaseField(columnName = "Code")
    public String mCode;

    @SerializedName("Description")
    @DatabaseField(columnName = "Description")
    public String mDescription;

    @SerializedName("CourtID")
    @DatabaseField(columnName = "CourtID")
    public long mCourtID;

    @SerializedName("GpsLatitude")
    @DatabaseField(columnName = "GpsLatitude")
    public Double mGpsLatitude;

    @SerializedName("GpsLongitude")
    @DatabaseField(columnName = "GpsLongitude")
    public Double mGpsLongitude;

    @SerializedName("InfringementLocationType")
    @DatabaseField(columnName = "InfringementLocationType")
    public InfringementLocationType mInfringementLocationType;

    @SerializedName("CourtName")
    @DatabaseField(columnName = "CourtName")
    public String mCourtName;

    public long getID() {
        return mID;
    }

    public void setID(long ID) {
        mID = mID;
    }

    public String getCode() {
        return mCode;
    }

    public void setCode(String code) {
        mCode = code;
    }

    public String getDescription() {
        return mDescription;
    }

    public void setDescription(String description) {
        mDescription = description;
    }

    public long getCourtID() {
        return mCourtID;
    }

    public void setCourtID(long courtID) {
        mCourtID = courtID;
    }

    public Double getGpsLatitude() {
        return mGpsLatitude;
    }

    public void setGpsLatitude(Double gpsLatitude) {
        mGpsLatitude = gpsLatitude;
    }

    public Double getGpsLongitude() {
        return mGpsLongitude;
    }

    public void setGpsLongitude(Double gpsLongitude) {
        mGpsLongitude = gpsLongitude;
    }

    public InfringementLocationType getInfringementLocationType() {
        return mInfringementLocationType;
    }

    public void setInfringementLocationType(InfringementLocationType infringementLocationType) {
        mInfringementLocationType = infringementLocationType;
    }

    public String getCourtName() {
        return mCourtName;
    }

    public void setCourtName(String courtName) {
        mCourtName = courtName;
    }
}

