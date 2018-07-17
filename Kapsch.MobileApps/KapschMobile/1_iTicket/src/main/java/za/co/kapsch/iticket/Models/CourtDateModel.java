package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;

import za.co.kapsch.iticket.Interfaces.IDelimitation;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/01/24.
 */
@DatabaseTable(tableName = "CourtDate")
public class CourtDateModel implements IDelimitation, Parcelable {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    private int mId;

    @SerializedName("Date")
    @DatabaseField(columnName = "Date")
    private Date mDate;

    @SerializedName("CourtID")
    @DatabaseField(columnName = "CourtID")
    private int mCourtId;

    @SerializedName("CourtRoomID")
    @DatabaseField(columnName = "CourtRoomID")
    private int mCourtRoomId;

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public int getId() {
        return mId;
    }

    public void setId(int id) {
        mId = id;
    }

    public Date getDate() {
        return mDate;
    }

    public void setDate(Date date) {
        mDate = date;
    }

    public int getCourtId() {
        return mCourtId;
    }

    public void setCourtId(int courtId) {
        mCourtId = courtId;
    }

    public int getCourtRoomId() {
        return mCourtRoomId;
    }

    public void setCourtRoomId(int courtRoomId) {
        mCourtRoomId = courtRoomId;
    }

    @Override
    public String toString() {
        return Utilities.dateTimeToString(mDate);
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeInt(mId);
        out.writeLong(mDate == null ? -1 : mDate.getTime());
        out.writeInt(mCourtId);
        out.writeInt(mCourtRoomId);
    }

    public static final Parcelable.Creator<CourtDateModel> CREATOR = new Parcelable.Creator<CourtDateModel>() {

        public CourtDateModel createFromParcel(Parcel in) {
            return new CourtDateModel(in);
        }

        public CourtDateModel[] newArray(int size) {
            return new CourtDateModel[size];
        }
    };

    public CourtDateModel(){}

    private CourtDateModel(Parcel in){
        mId = in.readInt();
        long tmpDate = in.readLong();
        mDate =  tmpDate == -1 ? null : new Date(tmpDate);
        mCourtId = in.readInt();
        mCourtRoomId = in.readInt();
    }
}
