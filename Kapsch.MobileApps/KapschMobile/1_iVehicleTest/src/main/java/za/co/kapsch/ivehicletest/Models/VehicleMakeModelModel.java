package za.co.kapsch.ivehicletest.Models;

import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;

/**
 * Created by csenekal on 2017/11/28.
 */

@DatabaseTable(tableName = "VehicleMakeModel")
public class VehicleMakeModelModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    public int mID;

    @SerializedName("VehicleMakeID")
    @DatabaseField(columnName = "VehicleMakeID")
    public int mVehicleMakeID;

    @SerializedName("Description")
    @DatabaseField(columnName = "Description")
    public String mDescription;

    @SerializedName("ExternalCode")
    @DatabaseField(columnName = "ExternalCode")
    public String mExternalCode;

    @SerializedName("ModifiedDate")
    @DatabaseField(columnName = "ModifiedDate")
    public Date mModifiedDate;

    public int getID() {
        return mID;
    }

    public int getVehicleMakeID() {
        return mVehicleMakeID;
    }

    public String getDescription() {
        return mDescription;
    }

    public String getExternalCode() {
        return mExternalCode;
    }

    public Date getModifiedDate() {
        return mModifiedDate;
    }
}
