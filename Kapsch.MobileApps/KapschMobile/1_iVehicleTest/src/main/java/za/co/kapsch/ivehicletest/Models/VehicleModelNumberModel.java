package za.co.kapsch.ivehicletest.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;

/**
 * Created by csenekal on 2017/11/28.
 */

@DatabaseTable(tableName = "VehicleModelNumber")
public class VehicleModelNumberModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    public int mID;

    @SerializedName("VehicleModelID")
    @DatabaseField(columnName = "VehicleModelID")
    public int mVehicleModelID;

    @SerializedName("Description")
    @DatabaseField(columnName = "Description")
    public String mDescription;

    @SerializedName("ExternalCode")
    @DatabaseField(columnName = "ExternalCode")
    public String mExternalCode;

    @SerializedName("ExternalModelCode")
    @DatabaseField(columnName = "ExternalModelCode")
    public String mExternalModelCode;

    @SerializedName("ModifiedDate")
    @DatabaseField(columnName = "ModifiedDate")
    public Date mModifiedDate;

    public int getID() {
        return mID;
    }

    public int getVehicleModelID() {
        return mVehicleModelID;
    }

    public String getDescription() {
        return mDescription;
    }

    public String getExternalCode() {
        return mExternalCode;
    }

    public String getExternalModelCode() {
        return mExternalModelCode;
    }

    public Date getModifiedDate() {
        return mModifiedDate;
    }
}
