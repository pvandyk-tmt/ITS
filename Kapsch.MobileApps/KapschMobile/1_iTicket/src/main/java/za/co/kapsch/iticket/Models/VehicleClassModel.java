package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import java.util.Date;

/**
 * Created by csenekal on 2016-08-04.
 */
public class VehicleClassModel implements Parcelable {
    private String mCode;
    private String mVehicleRestriction;
    private Date mFirstIssueDate;

    public VehicleClassModel(){}

    public String getCode() {
        return mCode;
    }

    public void setCode(String mCode) {
        this.mCode = mCode;
    }

    public String getVehicleRestriction() {
        return mVehicleRestriction;
    }

    public void setVehicleRestriction(String mVehicleRestriction) {
        this.mVehicleRestriction = mVehicleRestriction;
    }

    public Date getFirstIssueDate() {
        return mFirstIssueDate;
    }

    public void setFirstIssueDate(Date mFirstIssueDate) {
        this.mFirstIssueDate = mFirstIssueDate;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(mCode);
        out.writeString(mVehicleRestriction);
        out.writeLong(mFirstIssueDate.getTime());
    }

    public static final Parcelable.Creator<VehicleClassModel> CREATOR = new Parcelable.Creator<VehicleClassModel>() {
        public VehicleClassModel createFromParcel(Parcel in) {
            return new VehicleClassModel(in);
        }

        public VehicleClassModel[] newArray(int size) {
            return new VehicleClassModel[size];
        }
    };

    private VehicleClassModel(Parcel in) {
        mCode = in.readString();
        mVehicleRestriction = in.readString();
        mFirstIssueDate = new Date(in.readLong());
    }
}
