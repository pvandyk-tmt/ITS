package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/05/23.
 */
public class SectionModel implements Parcelable{

    @SerializedName("SectionPointA")
    private SectionPointModel mSectionPointStart;
    @SerializedName("SectionPointB")
    private SectionPointModel mSectionPointEnd;
    @SerializedName("AverageSpeed")
    private Double mAverageSpeed;
    @SerializedName("GraceSpeed")
    private Integer mGraceSpeed;
    @SerializedName("Zone")
    private Integer mZone;
    @SerializedName("TripDuration")
    private Double mTripDuration;
    @SerializedName("TravelDistance")
    private Double mTravelDistance;
    @SerializedName("SectionDistanceInMeter")
    private double mSectionDistanceInMeter;
    @SerializedName("SectionDescription")
    private String mSectionDescription;
    @SerializedName("SectionCode")
    private String mSectionCode;
    @SerializedName("Vln")
    private String mVln;
    @SerializedName("DateFormat")
    private String mDateFormat;
    @SerializedName("AverageAnprAccuracy")
    private Double mAverageAnprAccuracy;
    @SerializedName("MachineId")
    private String mMachineId;
    @SerializedName("IsOffence")
    private boolean mIsOffence;
    @SerializedName("FileName")
    private String mFileName;
    @SerializedName("FrameNumber")
    private int mFrameNumber;

    public SectionPointModel getSectionPointStart() {
        return mSectionPointStart;
    }

    public SectionPointModel getSectionPointEnd() {
        return mSectionPointEnd;
    }

    public Double getAverageSpeed() {
        return mAverageSpeed;
    }

    public Integer getZone() {
        return mZone;
    }

    public String getVln() {
        return mVln;
    }

    public String getDateFormat() {
        return mDateFormat;
    }

    public boolean isIsOffence() {
        return mIsOffence;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {

        out.writeParcelable(mSectionPointStart, flags);
        out.writeParcelable(mSectionPointEnd, flags);
        out.writeDouble(mAverageSpeed);
        out.writeInt(mGraceSpeed);
        out.writeInt(mZone);
        out.writeDouble(mTripDuration);
        out.writeDouble(mTravelDistance);
        out.writeDouble(mSectionDistanceInMeter);
        out.writeString(mSectionDescription);
        out.writeString(mSectionCode);
        out.writeString(mVln);
        out.writeString(mDateFormat);
        out.writeDouble(mAverageAnprAccuracy);
        out.writeString(mMachineId);
        out.writeByte((byte) (mIsOffence ? 1 : 0));
        out.writeString(mFileName);
        out.writeInt(mFrameNumber);
    }

    public static final Parcelable.Creator<SectionModel> CREATOR = new Parcelable.Creator<SectionModel>() {
        public SectionModel createFromParcel(Parcel in) {
            return new SectionModel(in);
        }

        public SectionModel[] newArray(int size) {
            return new SectionModel[size];
        }
    };

    public SectionModel(){}

    private SectionModel(Parcel in){

        mSectionPointStart = in.readParcelable(SectionPointModel.class.getClassLoader());
        mSectionPointEnd = in.readParcelable(SectionPointModel.class.getClassLoader());
        mAverageSpeed = in.readDouble();
        mGraceSpeed = in.readInt();
        mZone = in.readInt();
        mTripDuration = in.readDouble();
        mTravelDistance = in.readDouble();
        mSectionDistanceInMeter = in.readDouble();
        mSectionDescription = in.readString();
        mSectionCode = in.readString();
        mVln = in.readString();
        mDateFormat = in.readString();
        mAverageAnprAccuracy = in.readDouble();
        mMachineId = in.readString();
        mIsOffence = in.readByte() != 0;
        mFileName = in.readString();
        mFrameNumber = in.readInt();
    }
}
