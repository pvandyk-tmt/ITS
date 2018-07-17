package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

/**
 * Created by CSenekal on 2018/03/09.
 */

@DatabaseTable(tableName = "IdentificationType")
public class IdentificationTypeModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    private long mID;

    @SerializedName("Description")
    @DatabaseField(columnName = "Description")
    private String mDescription;

    public long getID() {
        return mID;
    }

    public void setID(long ID) {
        mID = ID;
    }

    public String getDescription() {
        return mDescription;
    }

    public void setDescription(String description) {
        mDescription = description;
    }

    @Override
    public String toString(){
        return mDescription;
    }
}
