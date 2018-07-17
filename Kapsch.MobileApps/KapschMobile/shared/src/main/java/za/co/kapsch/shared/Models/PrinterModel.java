package za.co.kapsch.shared.Models;

import android.os.Parcel;
import android.os.Parcelable;

/**
 * Created by csenekal on 2016-09-02.
 */
public class PrinterModel implements Parcelable {
    private String mFriendlyName;
    private String mMacAddress;

    public PrinterModel(String friendName, String macAddress) {
        mFriendlyName = friendName;
        mMacAddress = macAddress;
    }

    @Override
    public String toString(){
        return mFriendlyName;
    }

    public String getFriendlyName() {
        return mFriendlyName;
    }

    public void setFriendlyName(String friendlyName) {
        this.mFriendlyName = friendlyName;
    }

    public String getMacAddress() {
        return mMacAddress;
    }

    public void setMacAddress(String macAddress) {
        this.mMacAddress = macAddress;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(mFriendlyName);
        out.writeString(mMacAddress);
    }

    public static final Creator<PrinterModel> CREATOR = new Creator<PrinterModel>() {

        public PrinterModel createFromParcel(Parcel in) {
            return new PrinterModel(in);
        }

        public PrinterModel[] newArray(int size) {
            return new PrinterModel[size];
        }
    };

    private PrinterModel(Parcel in) {
        mFriendlyName = in.readString();
        mMacAddress = in.readString();
    }
}
