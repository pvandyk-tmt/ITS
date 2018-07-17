package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

/**
 * Created by CSenekal on 2017/09/07.
 */
@DatabaseTable(tableName = "VosiAction")
public class VosiActionModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    public long mID;

    @SerializedName("Description")
    @DatabaseField(columnName = "Description")
    public String mDescription;

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
