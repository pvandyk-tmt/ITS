package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;

import za.co.kapsch.iticket.Interfaces.IDelimitation;

/**
 * Created by csenekal on 2016-07-25.
 */
@DatabaseTable(tableName = "Court")
public class CourtModel implements IDelimitation, Parcelable {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    private int mId;

    @SerializedName("Name")
    @DatabaseField(columnName = "Name")
    private String mName;

    @SerializedName("Room")
    @DatabaseField(columnName = "Room")
    private String mRoom;

    private Date mDate;

    @Override
    public int getId(){
        return mId;
    }

    public String getName() {
        return mName;
    }

    public void setName(String name) {
        mName = name;
    }

    public String getRoom() {
        return mRoom;
    }

    public void setRoom(String room) {
        mRoom = room;
    }

    public Date getDate() {
        return mDate;
    }

    public void setDate(Date date) {
        mDate = date;
    }

    @Override
    public String toString() {
        return mName;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeInt(mId);
        out.writeString(mName);
        out.writeString(mRoom);
        out.writeLong(mDate == null ? -1 : mDate.getTime());
    }

    public static final Parcelable.Creator<CourtModel> CREATOR = new Parcelable.Creator<CourtModel>() {
        public CourtModel createFromParcel(Parcel in) {
            return new CourtModel(in);
        }

        public CourtModel[] newArray(int size) {
            return new CourtModel[size];
        }
    };

    public CourtModel(){}

    private CourtModel(Parcel in){
        mId = in.readInt();
        mName = in.readString();
        mRoom = in.readString();
        long tmpDate = in.readLong();
        mDate =  tmpDate == -1 ? null : new Date(tmpDate);
    }
}

