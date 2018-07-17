package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import za.co.kapsch.iticket.Interfaces.IDelimitation;

/**
 * Created by CSenekal on 2017/01/24.
 */
@DatabaseTable(tableName = "CourtRoom")
public class CourtRoomModel implements IDelimitation,  Parcelable {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    private int mId;

    @SerializedName("CourtID")
    @DatabaseField(columnName = "CourtID")
    private String mCourtId;

    @SerializedName("RoomNumber")
    @DatabaseField(columnName = "RoomNumber")
    private String mRoomNumber;

    public String getRoomNumber() {
        return mRoomNumber;
    }

    public void setRoomNumber(String room) {
        mRoomNumber = room;
    }

    @Override
    public int getId(){
        return mId;
    }

    @Override
    public String toString() {
        return mRoomNumber;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeInt(mId);
        out.writeString(mCourtId);
        out.writeString(mRoomNumber);
    }

    public static final Parcelable.Creator<CourtRoomModel> CREATOR = new Parcelable.Creator<CourtRoomModel>() {
        public CourtRoomModel createFromParcel(Parcel in) {
            return new CourtRoomModel(in);
        }

        public CourtRoomModel[] newArray(int size) {
            return new CourtRoomModel[size];
        }
    };

    public CourtRoomModel(){}

    private CourtRoomModel(Parcel in){
        mId = in.readInt();
        mCourtId = in.readString();
        mRoomNumber = in.readString();
    }
}