package za.co.kapsch.ivehicletest.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;

/**
 * Created by csenekal on 2017/11/28.
 */

@DatabaseTable(tableName = "VehicleMake")
public class VehicleMakeModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    public int mID;

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
