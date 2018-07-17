package za.co.kapsch.ivehicletest.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

import java.util.List;

/**
 * Created by CSenekal on 2018/02/05.
 */

public class VehicleInspectionQueryModel implements Parcelable{

    @SerializedName("BookingID")
    private long mBookingID;

    @SerializedName("TestTypeID")
    private long mTestTypeID;

    @SerializedName("TestTypeDescription")
    private String mTestTypeDescription;

    @SerializedName("SiteName")
    private String mSiteName;

    @SerializedName("SiteID")
    private long mSiteID;

    @SerializedName("TestCategory")
    private String mTestCategory;

    @SerializedName("BarcodeData")
    private String mBarcodeData;

    @SerializedName("IsPhotoRequired")
    private boolean mPhotoRequired;

    @SerializedName("Questions")
    private List<VehicleInspectionQuestionModel> mVehicleInspectionQuestionList;

    protected VehicleInspectionQueryModel(Parcel in) {
        mBookingID = in.readLong();
        mTestTypeID = in.readLong();
        mTestTypeDescription = in.readString();
        mSiteName = in.readString();
        mSiteID = in.readLong();
        mTestCategory = in.readString();
        mBarcodeData = in.readString();
        mPhotoRequired = in.readByte() != 0;
        mVehicleInspectionQuestionList = in.createTypedArrayList(VehicleInspectionQuestionModel.CREATOR);
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {
        dest.writeLong(mBookingID);
        dest.writeLong(mTestTypeID);
        dest.writeString(mTestTypeDescription);
        dest.writeString(mSiteName);
        dest.writeLong(mSiteID);
        dest.writeString(mTestCategory);
        dest.writeString(mBarcodeData);
        dest.writeByte((byte) (mPhotoRequired ? 1 : 0));
        dest.writeTypedList(mVehicleInspectionQuestionList);
    }

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Creator<VehicleInspectionQueryModel> CREATOR = new Creator<VehicleInspectionQueryModel>() {
        @Override
        public VehicleInspectionQueryModel createFromParcel(Parcel in) {
            return new VehicleInspectionQueryModel(in);
        }

        @Override
        public VehicleInspectionQueryModel[] newArray(int size) {
            return new VehicleInspectionQueryModel[size];
        }
    };

    public long getBookingID() {
        return mBookingID;
    }

    public void setBookingID(long bookingID) {
        mBookingID = bookingID;
    }

    public long getTestTypeID() {
        return mTestTypeID;
    }

    public void setTestTypeID(long testTypeID) {
        mTestTypeID = testTypeID;
    }

    public String getTestTypeDescription() {
        return mTestTypeDescription;
    }

    public void setTestTypeDescription(String testTypeDescription) {
        mTestTypeDescription = testTypeDescription;
    }

    public String getSiteName() {
        return mSiteName;
    }

    public void setSiteName(String siteName) {
        mSiteName = siteName;
    }

    public long getSiteID() {
        return mSiteID;
    }

    public void setSiteID(long siteID) {
        mSiteID = siteID;
    }

    public String getTestCategory() {
        return mTestCategory;
    }

    public String getBarcodeData() {
        return mBarcodeData;
    }

    public boolean isPhotoRequired() {
        return mPhotoRequired;
    }

    public void setIsPhotoRequired(boolean photoRequired) {
        mPhotoRequired = photoRequired;
    }

    public void setTestCategory(String testCategory) {
        mTestCategory = testCategory;
    }

    public List<VehicleInspectionQuestionModel> getVehicleInspectionQuestionList() {
        return mVehicleInspectionQuestionList;
    }

    public void setVehicleInspectionQuestionList(List<VehicleInspectionQuestionModel> vehicleInspectionQuestionList) {
        mVehicleInspectionQuestionList = vehicleInspectionQuestionList;
    }
}
