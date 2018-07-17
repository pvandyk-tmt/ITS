package za.co.kapsch.iticket.Models;

import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;

/**
 * Created by csenekal on 2016-09-15.
 */
@DatabaseTable(tableName = "PublicHoliday")
public class PublicHolidayModel {

    @DatabaseField(columnName = "ID",  generatedId = true)
    public long ID;

    @DatabaseField(columnName = "Description")
    public String HolidayDescription;

    @DatabaseField(columnName = "HolidayDate")
    public Date HolidayDate;

    @DatabaseField(columnName = "Active")
    public String Active;

    public long getId() {
        return ID;
    }

    public void setId(long id) {
        ID = id;
    }

    public String getHolidayDescription() {
        return HolidayDescription;
    }

    public void setHolidayDescription(String holidayDescription) {
        HolidayDescription = holidayDescription;
    }

    public Date getHolidayDate() {
        return HolidayDate;
    }

    public void setHolidayDate(Date holidayDate) {
        HolidayDate = holidayDate;
    }

    public String getActive() {
        return Active;
    }

    public void setActive(String active) {
        Active = active;
    }
}
