package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2016-08-11.
 */
public class InfringementChargeModel  implements Parcelable {

    private Long mId;
    private boolean mIsAlternative;
    private String mChargeCode;
    private String mDescription;
    private String mPrintDescription;
    private String mUserCapturedDescription;
    private String mRegulation;
    private double mFineAmount;
    private int mZone;
    private double mSpeed;
    private Long mOffenceSet;
    private boolean mIsByLaw;
    private int mAlternativeOffenceCodeId;

    public Long getId() {
        return mId;
    }

    public void setId(Long id) {
        mId = id;
    }

    public boolean getIsAlternative() {
        return mIsAlternative;
    }

    public void setIsAlternative(boolean isAlternative) {
        this.mIsAlternative = isAlternative;
    }

    public String getChargeCode() {
        return mChargeCode;
    }

    public void setChargeCode(String chargeCode) {
        mChargeCode = chargeCode;
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

    public void setPrintDescription(String printDescription) {
        mPrintDescription = printDescription;
    }

    public String getUserCapturedDescription() {
        return mUserCapturedDescription;
    }

    public void setUserCapturedDescription(String userCapturedDescription) {
        mUserCapturedDescription = userCapturedDescription;
    }

    public String getRegulation() {
        return mRegulation;
    }

    public void setRegulation(String regulation) {
        this.mRegulation = regulation;
    }

    public double getFineAmount() {
        return mFineAmount;
    }

    public void setFineAmount(double fineAmount) {
        this.mFineAmount = fineAmount;
    }

    public int getZone() {
        return mZone;
    }

    public void setZone(int zone) {
        this.mZone = zone;
    }

    public double getSpeed() {
        return mSpeed;
    }

    public void setSpeed(double speed) {
        this.mSpeed = speed;
    }

    public long getOffenceSet() {
        return mOffenceSet;
    }

    public void setOffenceSet(Long offenceSet) {
        mOffenceSet = offenceSet;
    }

    public boolean getIsByLaw(){
        return mIsByLaw;
    }

    public void setIsByLaw(boolean isByLaw){
        mIsByLaw = isByLaw;
    }

    public int getAlternativeOffenceCodeId() {
        return mAlternativeOffenceCodeId;
    }

    public void setAlternativeOffenceCodeId(int alternativeOffenceCodeId) {
        mAlternativeOffenceCodeId = alternativeOffenceCodeId;
    }


    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeLong(mId);
        out.writeByte((byte)(mIsAlternative ? 1 : 0));
        out.writeString(mChargeCode);
        out.writeString(mDescription);
        out.writeString(mPrintDescription);
        out.writeString(mUserCapturedDescription);
        out.writeString(mRegulation);
        out.writeDouble(mFineAmount);
        out.writeInt(mZone);
        out.writeDouble(mSpeed);
        Utilities.writeNullableLong(out, mOffenceSet);
        out.writeByte((byte)(mIsByLaw ? 1 : 0));
        out.writeInt(mAlternativeOffenceCodeId);
    }

    public static final Parcelable.Creator<InfringementChargeModel> CREATOR = new Parcelable.Creator<InfringementChargeModel>() {
        public InfringementChargeModel createFromParcel(Parcel in) {
            return new InfringementChargeModel(in);
        }

        public InfringementChargeModel[] newArray(int size) {
            return new InfringementChargeModel[size];
        }
    };

    public InfringementChargeModel(){}

    private InfringementChargeModel(Parcel in){
        mId = in.readLong();
        mIsAlternative = in.readByte() != 0;
        mChargeCode = in.readString();
        mDescription = in.readString();
        mPrintDescription = in.readString();;
        mUserCapturedDescription = in.readString();
        mRegulation = in.readString();
        mFineAmount = in.readDouble();
        mZone = in.readInt();
        mSpeed = in.readDouble();
        mOffenceSet = Utilities.readNullableLong(in);
        mIsByLaw = in.readByte() != 0;
        mAlternativeOffenceCodeId = in.readInt();
     }
}
