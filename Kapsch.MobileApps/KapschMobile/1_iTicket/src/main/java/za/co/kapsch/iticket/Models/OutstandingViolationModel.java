package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

import java.util.Date;

import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;

/**
 * Created by CSenekal on 2018/04/17.
 */

public class OutstandingViolationModel implements Parcelable {

    @SerializedName("ReferenceNumber")
    private String mReferenceNumber;

    @SerializedName("OffenderIDNumber")
    private String mOffenderIDNumber;

    @SerializedName("VLN")
    private String mVLN;

    @SerializedName("OffenceDate")
    private Date mOffenceDate;

    @SerializedName("OffenceAmount")
    private double mOffenceAmount;

    @SerializedName("OutstandingAmount")
    private double mOutstandingAmount;

    @SerializedName("PaidAmount")
    private double mPaidAmount;

    private boolean mChecked;

    private SearchFinesCriteriaType mSearchFinesCriteriaType;

    protected OutstandingViolationModel(Parcel in) {
        mReferenceNumber = in.readString();
        mOffenderIDNumber = in.readString();
        mVLN = in.readString();
        long offenceDate = in.readLong();
        mOffenceDate = offenceDate == -1 ? null : new Date(offenceDate);
        mOffenceAmount = in.readDouble();
        mOutstandingAmount = in.readDouble();
        mPaidAmount = in.readDouble();
        mChecked = in.readByte() != 0;
        mSearchFinesCriteriaType = (SearchFinesCriteriaType)in.readValue(SearchFinesCriteriaType.class.getClassLoader());
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {

        dest.writeString(mReferenceNumber);
        dest.writeString(mOffenderIDNumber);
        dest.writeString(mVLN);
        dest.writeLong(mOffenceDate == null ? -1 : mOffenceDate.getTime());
        dest.writeDouble(mOffenceAmount);
        dest.writeDouble(mOutstandingAmount);
        dest.writeDouble(mPaidAmount);
        dest.writeByte((byte) (mChecked ? 1 : 0));
        dest.writeValue(mSearchFinesCriteriaType);
    }

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Parcelable.Creator<OutstandingViolationModel> CREATOR = new Parcelable.Creator<OutstandingViolationModel>() {
        @Override
        public OutstandingViolationModel createFromParcel(Parcel in) {
            return new OutstandingViolationModel(in);
        }

        @Override
        public OutstandingViolationModel[] newArray(int size) {
            return new OutstandingViolationModel[size];
        }
    };

    public boolean isChecked() {
        return mChecked;
    }

    public void setChecked(boolean checked) {
        mChecked = checked;
    }

    public String getReferenceNumber() {
        return mReferenceNumber;
    }

    public void setReferenceNumber(String referenceNumber) {
        mReferenceNumber = referenceNumber;
    }

    public String getOffenderIDNumber() {
        return mOffenderIDNumber;
    }

    public String getVLN() {
        return mVLN;
    }

    public void setVLN(String vln) {
        mVLN = vln;
    }

    public Date getOffenceDate() {
        return mOffenceDate;
    }

    public void setOffenceDate(Date offenceDate) {
        mOffenceDate = offenceDate;
    }

    public double getOffenceAmount() {
        return mOffenceAmount;
    }

    public double getOutstandingAmount() {
        return mOutstandingAmount;
    }

    public void setOutstandingAmount(double outstandingAmount) {
        mOutstandingAmount = outstandingAmount;
    }

    public SearchFinesCriteriaType getSearchFinesCriteriaType() {
        return mSearchFinesCriteriaType;
    }

    public void setSearchFinesCriteriaType(SearchFinesCriteriaType searchFinesCriteriaType) {
        mSearchFinesCriteriaType = searchFinesCriteriaType;
    }
}
