package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

/**
 * Created by csenekal on 2016-07-14.
 */
public class VehicleModel  implements Parcelable {

    private String mUnknownNumberA;
    private String mUnknownNumberB;
    private String mUnknownNumberC;
    private String mDiscNumber;
    private String mLicenceNumber;
    private String mRegisterNumber;
    private String mDescription;
    private String mVehicleTypeGroup;
    private String mMake;
    private String mType;
    private String mModel;
    private String mColour;
    private String mVehicleIdentificationNumber;
    private String mEngineNumber;
    private String mExpireDate;

    public VehicleModel(){}

    public String getUnknownNumberA() {
        return mUnknownNumberA;
    }

    public String getUnknownNumberB() {
        return mUnknownNumberB;
    }

    public String getUnknownNumberC() {
        return mUnknownNumberC;
    }

    public String getDiscNumber() {
        return mDiscNumber;
    }

    public String getLicenceNumber() {
        return mLicenceNumber;
    }

    public String getRegisterNumber() {
        return mRegisterNumber;
    }

    public String getDescription() {
        return mDescription;
    }

    public String getVehicleTypeGroup() { return mVehicleTypeGroup; }

    public String getMake() {
        return mMake;
    }

    public String getType() { return mType; }

    public String getModel() {
        return mModel;
    }

    public String getColour() {
        return mColour;
    }

    public String getVehicleIdentificationNumber() {
        return mVehicleIdentificationNumber;
    }

    public String getEngineNumber() {
        return mEngineNumber;
    }

    public String getExpireDate() {
        return mExpireDate;
    }

    public void setmUnknownNumberA(String unknownNumberA) { mUnknownNumberA = unknownNumberA; }

    public void setmUnknownNumberB(String unknownNumberB) { mUnknownNumberB = unknownNumberB; }

    public void setmUnknownNumberC(String unknownNumberC) { mUnknownNumberC = unknownNumberC; }

    public void setDiscNumber(String discNumber) {
        mDiscNumber = discNumber;
    }

    public void setLicenceNumber(String licenceNumber) {
        mLicenceNumber = licenceNumber;
    }

    public void setRegisterNumber(String registerNumber) { mRegisterNumber = registerNumber; }

    public void setDescription(String description) {
        mDescription = description;
    }

    public void setVehicleTypeGroup(String vehicleTypeGroup) { mVehicleTypeGroup = vehicleTypeGroup; }

    public void setMake(String make) {
        mMake = make;
    }

    public void setType(String type) {
        mType = type;
    }

    public void setModel(String model) {
        mModel = model;
    }

    public void setColour(String colour) {
        mColour = colour;
    }

    public void setVehicleIdentificationNumber(String vehicleIdentificationNumber) { mVehicleIdentificationNumber = vehicleIdentificationNumber; }

    public void setEngineNumber(String engineNumber) {
        mEngineNumber = engineNumber;
    }

    public void setExpireDate(String expireDate) {
        mExpireDate = expireDate;
    }

    public void setData(String data){

        String[] fields = data.split("%");

        //TODO: fix fields.length when Manatee works sdk has correct licence:
        if (fields.length > 10) {
            mUnknownNumberA = fields[0];
            mUnknownNumberB = fields[1];
            mUnknownNumberC = fields[2];

            mDiscNumber = fields[5];
            mLicenceNumber = fields[6];
            mRegisterNumber = fields[7];
            mDescription = fields[8];
            mMake = fields[9];
            mModel = fields[10];
            mColour = fields[11];
            mVehicleIdentificationNumber = fields[12];
            mEngineNumber = fields[13];
            mExpireDate = fields[14];
        }
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString( mUnknownNumberA);
        out.writeString(mUnknownNumberB);
        out.writeString(mUnknownNumberC);
        out.writeString(mDiscNumber);
        out.writeString(mLicenceNumber);
        out.writeString(mRegisterNumber);
        out.writeString(mDescription);
        out.writeString(mVehicleTypeGroup);
        out.writeString(mMake);
        out.writeString(mType);
        out.writeString(mModel);
        out.writeString(mColour);
        out.writeString(mVehicleIdentificationNumber);
        out.writeString(mEngineNumber);
        out.writeString(mExpireDate);
    }

    public static final Parcelable.Creator<VehicleModel> CREATOR = new Parcelable.Creator<VehicleModel>() {
        public VehicleModel createFromParcel(Parcel in) {
            return new VehicleModel(in);
        }

        public VehicleModel[] newArray(int size) {
            return new VehicleModel[size];
        }
    };

    private VehicleModel(Parcel in){
        mUnknownNumberA = in.readString();
        mUnknownNumberB = in.readString();
        mUnknownNumberC = in.readString();
        mDiscNumber = in.readString();
        mLicenceNumber = in.readString();
        mRegisterNumber = in.readString();
        mDescription = in.readString();
        mVehicleTypeGroup = in.readString();
        mMake = in.readString();
        mType = in.readString();
        mModel = in.readString();
        mColour = in.readString();
        mVehicleIdentificationNumber = in.readString();
        mEngineNumber = in.readString();
        mExpireDate = in.readString();
    }
}
