package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

/**
 * Created by csenekal on 2016-10-06.
 */
@DatabaseTable(tableName = "CancellationReason")
public class CancellationReasonModel implements Parcelable {

    @DatabaseField(columnName = "ID", generatedId = true)
    private int mId;

    @DatabaseField(columnName = "Reason")
    private String mReason;

    public int getId() {
        return mId;
    }

    public void setId(int id) {
        this.mId = id;
    }

    public String getReason() {
        return mReason;
    }

    public void setReason(String reason) {
        this.mReason = reason;
    }

    @Override
    public String toString() {
        return mReason;
    }

    @Override
    public int describeContents() {
        return 0;
    }


    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeInt(mId);
        out.writeString(mReason);
    }

    public static final Parcelable.Creator<CancellationReasonModel> CREATOR = new Parcelable.Creator<CancellationReasonModel>() {
        public CancellationReasonModel createFromParcel(Parcel in) {
            return new CancellationReasonModel(in);
        }

        public CancellationReasonModel[] newArray(int size) {
            return new CancellationReasonModel[size];
        }
    };

    public CancellationReasonModel(){}

    private CancellationReasonModel(Parcel in){
        mId = in.readInt();
        mReason = in.readString();
    }
}
