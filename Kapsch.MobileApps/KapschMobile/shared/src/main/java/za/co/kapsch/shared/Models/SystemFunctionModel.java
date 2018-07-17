package za.co.kapsch.shared.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import za.co.kapsch.shared.Enums.UserStatus;

/**
 * Created by CSenekal on 2017/07/04.
 */

@DatabaseTable(tableName = "SystemFunction")
public class SystemFunctionModel implements Parcelable {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    public long mID;
    @SerializedName("Name")
    @DatabaseField(columnName = "Name")
    public String mName;
    @SerializedName("Description")
    @DatabaseField(columnName = "Description")
    public String mDescription;

//    @DatabaseField(foreign=true, foreignAutoRefresh=true)
//    private UserModel mUser;

    public long getID() {
        return mID;
    }

    public String getName() {
        return mName;
    }

    public String getDescription() {
        return mDescription;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {

        out.writeLong(mID);
        out.writeString(mName);
        out.writeString(mDescription);
    }

    public static final Parcelable.Creator<SystemFunctionModel> CREATOR = new Parcelable.Creator<SystemFunctionModel>() {
        public SystemFunctionModel createFromParcel(Parcel in) {
            return new SystemFunctionModel(in);
        }

        public SystemFunctionModel[] newArray(int size) {
            return new SystemFunctionModel[size];
        }
    };

    public SystemFunctionModel(){}

    private SystemFunctionModel(Parcel in){
        mID = in.readLong();
        mName = in.readString();
        mDescription = in.readString();
    }
}
