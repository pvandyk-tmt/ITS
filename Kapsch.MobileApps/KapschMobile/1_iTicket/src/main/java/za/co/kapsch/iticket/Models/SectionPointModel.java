package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

import java.util.Date;

import za.co.kapsch.iticket.Enums.DirectionEnum;

/**
 * Created by CSenekal on 2017/05/23.
 */
public class SectionPointModel implements Parcelable {

    @SerializedName("SectionPointCode")
    private String mSectionPointCode;
    @SerializedName("TextLin")
    private String mTextLine;
    @SerializedName("EventDateTime")
    private Date mEventDateTime;
    @SerializedName("AnprAccuracy")
    private double mAnprAccuracy;
    @SerializedName("ShotDistance")
    private double mShotDistance;
    @SerializedName("PointLocation")
    private LocationModel mPointLocation;
    @SerializedName("ShotDirection")
    private DirectionEnum mShotDirection;
    @SerializedName("Classification")
    private ClassificationZoneModel  mClassification;
    @SerializedName("Speed")
    private Integer mSpeed;
    @SerializedName("SerialNumber")
    private String mSerialNumber;
    @SerializedName("MachineId")
    private String mMachineId;
    @SerializedName("Vln")
    private String mVln;
    @SerializedName("HashVln")
    private String mHashVln;
    @SerializedName("VosiReason")
    private String mVosiReason;
    @SerializedName("Image")
    private String mImage;
    @SerializedName("ImagePhysicalFileAndPath")
    private String mImagePhysicalFileAndPath;
    @SerializedName("ImageName")
    private String mImageName;
    @SerializedName("PlateImagePhysicalFileAndPath")
    private String mPlateImagePhysicalFileAndPath;
    @SerializedName("PlateImageName")
    private String mPlateImageName;

    public Date getEventDateTime() {
        return mEventDateTime;
    }

    public LocationModel getPointLocation() {
        return mPointLocation;
    }

    public ClassificationZoneModel getClassification() {
        return mClassification;
    }

    public Integer getSpeed() {
        return mSpeed;
    }

    public String getVln() {
        return mVln;
    }

    public String getImage() {
        return mImage;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {

        out.writeString(mSectionPointCode);
        out.writeString(mTextLine);
        out.writeLong(mEventDateTime == null ? -1 : mEventDateTime.getTime());
        out.writeDouble(mAnprAccuracy);
        out.writeDouble(mShotDistance);
        out.writeParcelable(mPointLocation, flags);
        out.writeValue(mShotDirection);
        out.writeParcelable(mClassification, flags);
        out.writeInt(mSpeed);
        out.writeString(mSerialNumber);
        out.writeString(mMachineId);
        out.writeString(mVln);
        out.writeString(mHashVln);
        out.writeString(mVosiReason);
        out.writeString(mImage);
        out.writeString(mImagePhysicalFileAndPath);
        out.writeString(mImageName);
        out.writeString(mPlateImagePhysicalFileAndPath);
        out.writeString(mPlateImageName);
    }

    public static final Parcelable.Creator<SectionPointModel> CREATOR = new Parcelable.Creator<SectionPointModel>() {
        public SectionPointModel createFromParcel(Parcel in) {
            return new SectionPointModel(in);
        }

        public SectionPointModel[] newArray(int size) {
            return new SectionPointModel[size];
        }
    };

    public SectionPointModel(){}

    private SectionPointModel(Parcel in){
        mSectionPointCode = in.readString();
        mTextLine = in.readString();
        long tmpDate = in.readLong();
        mEventDateTime = tmpDate == -1 ? null : new Date(tmpDate);
        mAnprAccuracy = in.readDouble();
        mShotDistance = in.readDouble();
        mPointLocation = in.readParcelable(LocationModel.class.getClassLoader());
        mShotDirection = (DirectionEnum)in.readValue(DirectionEnum.class.getClassLoader());
        mClassification = in.readParcelable(ClassificationZoneModel.class.getClassLoader());
        mSpeed = in.readInt();
        mSerialNumber = in.readString();
        mMachineId = in.readString();
        mVln = in.readString();
        mHashVln = in.readString();
        mVosiReason = in.readString();
        mImage = in.readString();
        mImagePhysicalFileAndPath = in.readString();
        mImageName = in.readString();
        mPlateImagePhysicalFileAndPath = in.readString();
        mPlateImageName = in.readString();
    }
}
