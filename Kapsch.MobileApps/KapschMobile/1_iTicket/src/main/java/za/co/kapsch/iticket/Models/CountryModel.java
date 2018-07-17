package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

/**
 * Created by CSenekal on 2018/04/20.
 */
@DatabaseTable(tableName = "Country")
public class CountryModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    private int mID;

    @SerializedName("Description")
    @DatabaseField(columnName = "Description")
    private String mDescription;

    public int getID() {
        return mID;
    }

    public String getDescription() {
        return mDescription;
    }

    @Override
    public String toString(){
        return mDescription;
    }
}
