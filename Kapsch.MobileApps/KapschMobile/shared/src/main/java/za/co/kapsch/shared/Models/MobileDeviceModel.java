package za.co.kapsch.shared.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

/**
 * Created by CSenekal on 2017/06/29.
 */
@DatabaseTable(tableName = "MobileDevice")
public class MobileDeviceModel implements Parcelable{

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    public long mID;

    @SerializedName("DeviceID")
    @DatabaseField(columnName = "DeviceID")
    public String mDeviceID;

    @SerializedName("DistrictID")
    @DatabaseField(columnName = "DistrictID")
    public Long mDistrictID;

    @SerializedName("SerialNumber")
    @DatabaseField(columnName = "SerialNumber")
    public String mSerialNumber;

    public MobileDeviceModel(){};

    protected MobileDeviceModel(Parcel in) {
        mID = in.readLong();
        mDeviceID = in.readString();
        mDistrictID = in.readLong();
        mSerialNumber = in.readString();
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {
        dest.writeLong(mID);
        dest.writeString(mDeviceID);
        dest.writeLong(mDistrictID);
        dest.writeString(mSerialNumber);
    }

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Creator<MobileDeviceModel> CREATOR = new Creator<MobileDeviceModel>() {
        @Override
        public MobileDeviceModel createFromParcel(Parcel in) {
            return new MobileDeviceModel(in);
        }

        @Override
        public MobileDeviceModel[] newArray(int size) {
            return new MobileDeviceModel[size];
        }
    };

    public long getID() {
        return mID;
    }

    public void setID(long ID) {
        mID = ID;
    }

    public String getDeviceID() {
        return mDeviceID;
    }

    public void setDeviceID(String deviceID) {
        mDeviceID = deviceID;
    }

    public Long getDistrictID() {
        return mDistrictID;
    }

    public void setDistrictID(Long districtID) {
        mDistrictID = districtID;
    }

    public void setSerialNumber(String serialNumber) {
        mSerialNumber = serialNumber;
    }
}
