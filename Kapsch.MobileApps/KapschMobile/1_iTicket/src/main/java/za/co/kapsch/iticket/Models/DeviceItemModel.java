package za.co.kapsch.iticket.Models;

import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

/**
 * Created by csenekal on 2016-09-12.
 */
@DatabaseTable(tableName = "Device")
public class DeviceItemModel {

    @DatabaseField(columnName = "ID", generatedId = true)
    private int ID;

    @DatabaseField(columnName = "Desc")
    private String Desc;

    @DatabaseField(columnName = "Value")
    private String Value;

    public int getID() {
        return ID;
    }

    public void setID(int id) {
        this.ID = id;
    }

    public String getDesc() {
        return Desc;
    }

    public void setDesc(String desc) {
        Desc = desc;
    }

    public String getValue() {
        return Value;
    }

    public void setValue(String value) {
        Value = value;
    }
}
