package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2016-08-12.
 */
@DatabaseTable(tableName = "ChargeInfo")
public class ChargeInfoModel implements Parcelable{

    @SerializedName("Id")
    @DatabaseField(columnName = "ID", id = true)
    private Long mID;

    @SerializedName("OffenceSetID")
    @DatabaseField(columnName = "OffenceSetID")
    private Long mOffenceSetID;

    @SerializedName("Code")
    @DatabaseField(columnName = "Code")
    private String mCode;

    @SerializedName("Description")
    @DatabaseField(columnName = "Description")
    private String mDescription;

    @SerializedName("PrintDescription")
    @DatabaseField(columnName = "PrintDescription")
    private String mPrintDescription;

    @SerializedName("RegulationDescription")
    @DatabaseField(columnName = "RegulationDescription")
    private String mRegulationDescription;

    @SerializedName("FineAmount")
    @DatabaseField(columnName = "FineAmount")
    private double mFineAmount;

    @SerializedName("VehicleType")
    @DatabaseField(columnName = "VehicleType")
    private String mVehicleType;

    @SerializedName("Zone")
    @DatabaseField(columnName = "Zone")
    private int mZone;

    @SerializedName("MinSpeed")
    @DatabaseField(columnName = "MinSpeed")
    private int mMinSpeed;

    @SerializedName("MaxSpeed")
    @DatabaseField(columnName = "MaxSpeed")
    private int mMaxSpeed;

    @SerializedName("WimVehicleTypeID")
    @DatabaseField(columnName = "WimVehicleTypeID")
    private Long mWimVehicleTypeID;

    @SerializedName("WimOffenceDescription")
    @DatabaseField(columnName = "WimOffenceDescription")
    private String mWimOffenceDescription;

    @SerializedName("MimOverWeightPersent")
    @DatabaseField(columnName = "MimOverWeightPersent")
    private Integer mMimOverWeightPersent;

    @SerializedName("MaxOverWeightPersent")
    @DatabaseField(columnName = "MaxOverWeightPersent")
    private Integer mMaxOverWeightPersent;

    @SerializedName("CaseTypeID")
    @DatabaseField(columnName = "CaseTypeID")
    private Long mCaseTypeID;

    @SerializedName("IsFavourite")
    @DatabaseField(columnName = "IsFavourite")
    private boolean mIsFavourite;

    private transient String mVehicleRegistrationNumber;

    public int mSpeed;

    public long getId() { return mID; }

    public boolean getIsFavourite() {
        return mIsFavourite;
    }

    public void setIsFavourite(boolean isFavourite) {
        mIsFavourite = isFavourite;
    }

    public String getCode() {
        return mCode;
    }

    public void setCode(String code) {
        mCode = code;
    }

    public String getDescription() {
        return mDescription;
    }

    public void setDescription(String description) {
        mDescription = description;
    }

    public String getPrintDescription() {
        return mPrintDescription;
    }

    public void setPrintDescription(String printdescription) {
        mPrintDescription = printdescription;
    }

    public String getRegulationDescription() { return mRegulationDescription; }

    public void setRegulationDescription(String regulationDescription) {
        mRegulationDescription = regulationDescription;
    }

    public double getFineAmount() {
        return mFineAmount;
    }

    public void setFineAmount(double fineAmount) {
        mFineAmount = fineAmount;
    }

    public int getZone() {
        return mZone;
    }

    public void setZone(int zone) {
        mZone = zone;
    }

    public int getMinSpeed() {
        return mMinSpeed;
    }

    public void setMinSpeed(int minSpeed) {
        mMinSpeed = minSpeed;
    }

    public int getMaxSpeed() { return mMaxSpeed; }

    public void setMaxSpeed(int maxSpeed) { mMaxSpeed = maxSpeed; }

    public Long getWimVehicleTypeID() {
        return mWimVehicleTypeID;
    }

    public String getWimOffenceDescription() {
        return mWimOffenceDescription;
    }

    public Integer getMimOverWeightPersent() {
        return mMimOverWeightPersent;
    }

    public Integer getMaxOverWeightPersent() {
        return mMaxOverWeightPersent;
    }

    public Long getCaseTypeID() {
        return mCaseTypeID;
    }

    public String getVehicleType() { return mVehicleType; }

    public void setVehicleType(String vehicleType) { mVehicleType = vehicleType; }

    public int getSpeed() {
        return mSpeed;
    }

    public void setSpeed(int speed) {
        mSpeed = speed;
    }

    public Long getOffenceSet() {
        return mOffenceSetID;
    }

    public void setOffenceSet(Long offenceSetID) {
        mOffenceSetID = offenceSetID;
    }

    public void setVehicleRegistrationNumber(String vehicleRegistrationNumber){ mVehicleRegistrationNumber = vehicleRegistrationNumber; }

    public String getVehicleRegistrationNumber(){return mVehicleRegistrationNumber;}

    @Override
    public String toString() {
        return mCode;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {

        out.writeLong(mID);
        Utilities.writeNullableLong(out, mOffenceSetID);
        out.writeString(mCode);
        out.writeString(mDescription);
        out.writeString(mPrintDescription);
        out.writeString(mRegulationDescription);
        out.writeDouble(mFineAmount);
        out.writeString(mVehicleType);
        out.writeInt(mZone);
        out.writeInt(mMinSpeed);
        out.writeInt(mMaxSpeed);
        out.writeInt(mSpeed);

        Utilities.writeNullableLong(out, mWimVehicleTypeID);
        out.writeString(mWimOffenceDescription);
        Utilities.writeNullableInteger(out, mMimOverWeightPersent);
        Utilities.writeNullableInteger(out, mMaxOverWeightPersent);
        Utilities.writeNullableLong(out, mCaseTypeID);

        out.writeString(mVehicleRegistrationNumber);
        out.writeByte((byte)(mIsFavourite ? 1 : 0));
    }

    public static final Parcelable.Creator<ChargeInfoModel> CREATOR = new Parcelable.Creator<ChargeInfoModel>() {
        public ChargeInfoModel createFromParcel(Parcel in) {
            return new ChargeInfoModel(in);
        }

        public ChargeInfoModel[] newArray(int size) {
            return new ChargeInfoModel[size];
        }
    };

    public ChargeInfoModel(){}

    private ChargeInfoModel(Parcel in){

        mID = in.readLong();
        mOffenceSetID = Utilities.readNullableLong(in);
        mCode = in.readString();
        mDescription = in.readString();
        mPrintDescription = in.readString();
        mRegulationDescription = in.readString();
        mFineAmount = in.readDouble();
        mVehicleType = in.readString();
        mZone = in.readInt();
        mMinSpeed = in.readInt();
        mMaxSpeed = in.readInt();
        mSpeed = in.readInt();

        mWimVehicleTypeID = Utilities.readNullableLong(in);
        mWimOffenceDescription = in.readString();
        mMimOverWeightPersent = Utilities.readNullableInteger(in);
        mMaxOverWeightPersent = Utilities.readNullableInteger(in);
        mCaseTypeID = Utilities.readNullableLong(in);

        mVehicleRegistrationNumber = in.readString();
        mIsFavourite = in.readByte() != 0;
    }
}
