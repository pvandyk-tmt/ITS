package za.co.kapsch.shared.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

import java.util.Date;
import java.util.List;

import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Enums.OffenceStatus;
import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2017/08/23.
 */
public class FineModel implements Parcelable{

    @SerializedName("ReferenceNumber")
    private String mReferenceNumber;

    @SerializedName("OfficerCredentialID")
    public long mOfficerCredentialID;

    @SerializedName("OfficerFirstName")
    public String mOfficerFirstName;

    @SerializedName("OfficerLastName")
    public String mOfficerLastName;

    @SerializedName("ExternalID")
    public String mExternalID;

    @SerializedName("DistrictID")
    private Long mDistrictID;

    @SerializedName("DistrictName")
    private String mDistrictName;

    @SerializedName("PaymentOptions")
    private String mPaymentOptions;

    @SerializedName("CourtID")
    public Long mCourtID;

    @SerializedName("CourtName")
    public String mCourtName;

    @SerializedName("CourtDate")
    public Date mCourtDate;

    @SerializedName("OffenderIDNumber")
    private String mOffenderIDNumber;

    @SerializedName("OffenderIDType")
    private long mOffenderIDType;

    @SerializedName("OffenderEmail")
    public String mOffenderEmail;

    @SerializedName("OffenderMobileNumber")
    public String mOffenderMobileNumber;

    @SerializedName("OffenderLastName")
    private String mOffenderLastName;

    @SerializedName("OffenderFirstName")
    private String mOffenderFirstName;

    @SerializedName("OffenderAddressLine1")
    private String mOffenderAddressLine1;

    @SerializedName("OffenderAddressLine2")
    private String mOffenderAddressLine2;

    @SerializedName("OffenderAddressSuburb")
    private String mOffenderAddressSuburb;

    @SerializedName("OffenderAddressTown")
    private String mOffenderAddressTown;

    @SerializedName("VLN")
    private String mVLN;

    @SerializedName("VehicleMake")
    private String mVehicleMake;

    @SerializedName("VehicleModel")
    private String mVehicleModel;

    @SerializedName("SpeedLimit")
    private Double mSpeedLimit;

    @SerializedName("OffenceDate")
    private Date mOffenceDate;

    @SerializedName("OffenceSpeed")
    private Double mOffenceSpeed;

    @SerializedName("OffenceLocation")
    private String mOffenceLocation;

    @SerializedName("OffenceAmount")
    private double mOffenceAmount;

    @SerializedName("OutstandingAmount")
    private double mOutstandingAmount;

    @SerializedName("PaidAmount")
    private double mPaidAmount;

    @SerializedName("FirstPrintDate")
    private Date mFirstPrintDate;

    @SerializedName("TransactionToken")
    private String mTransactionToken;

    @SerializedName("Status")
    private OffenceStatus mStatus;

    @SerializedName("FineEvidenceModels")
    private List<FineEvidenceModel> mFineEvidenceModels;

    @SerializedName("FineChargeModels")
    private List<FineChargeModel> mFineChargeModels;

    private boolean mChecked;

    private SearchFinesCriteriaType mSearchFinesCriteriaType;

    protected FineModel(Parcel in) {

        mReferenceNumber = in.readString();
        mOfficerCredentialID = in.readLong();
        mOfficerFirstName = in.readString();
        mOfficerLastName = in.readString();
        mExternalID = in.readString();

        mDistrictID = in.readLong();
        mDistrictName = in.readString();
        mPaymentOptions = in.readString();
        mCourtID = in.readLong();
        mCourtName = in.readString();

        long courtDate = in.readLong();
        mCourtDate = courtDate == -1 ? null : new Date(courtDate);

        mOffenderIDNumber = in.readString();
        mOffenderIDType = in.readLong();
        mOffenderEmail = in.readString();
        mOffenderMobileNumber = in.readString();
        mOffenderLastName = in.readString();
        mOffenderFirstName = in.readString();

        mOffenderAddressLine1 = in.readString();
        mOffenderAddressLine2 = in.readString();
        mOffenderAddressSuburb = in.readString();
        mOffenderAddressTown = in.readString();

        mVLN = in.readString();
        mVehicleMake = in.readString();
        mVehicleModel = in.readString();

        mSpeedLimit = in.readDouble();

        long offenceDate = in.readLong();
        mOffenceDate = offenceDate == -1 ? null : new Date(offenceDate);

        mOffenceSpeed = in.readDouble();
        mOffenceLocation = in.readString();
        mOffenceAmount = in.readDouble();
        mOutstandingAmount = in.readDouble();
        mPaidAmount = in.readDouble();

        long firstPrintDate = in.readLong();
        mFirstPrintDate = firstPrintDate == -1 ? null : new Date(firstPrintDate);

        mTransactionToken = in.readString();
        mStatus = (OffenceStatus)in.readValue(OffenceStatus.class.getClassLoader());
        mFineEvidenceModels = in.createTypedArrayList(FineEvidenceModel.CREATOR);
        mFineChargeModels = in.createTypedArrayList(FineChargeModel.CREATOR);
        mChecked = in.readByte() != 0;
        mSearchFinesCriteriaType = (SearchFinesCriteriaType)in.readValue(SearchFinesCriteriaType.class.getClassLoader());
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {

        try {
            dest.writeString(mReferenceNumber);
            dest.writeLong(mOfficerCredentialID);
            dest.writeString(mOfficerFirstName);
            dest.writeString(mOfficerLastName);
            dest.writeString(mExternalID);

            dest.writeLong(mDistrictID);
            dest.writeString(mDistrictName);
            dest.writeString(mPaymentOptions);
            dest.writeLong(mCourtID);
            dest.writeString(mCourtName);
            dest.writeLong(mCourtDate == null ? -1 : mCourtDate.getTime());

            dest.writeString(mOffenderIDNumber);
            dest.writeLong(mOffenderIDType);
            dest.writeString(mOffenderEmail);
            dest.writeString(mOffenderMobileNumber);
            dest.writeString(mOffenderLastName);
            dest.writeString(mOffenderFirstName);

            dest.writeString(mOffenderAddressLine1);
            dest.writeString(mOffenderAddressLine2);
            dest.writeString(mOffenderAddressSuburb);
            dest.writeString(mOffenderAddressTown);

            dest.writeString(mVLN);
            dest.writeString(mVehicleMake);
            dest.writeString(mVehicleModel);

            dest.writeDouble(mSpeedLimit == null ? 0 : mSpeedLimit);
            dest.writeLong(mOffenceDate == null ? -1 : mOffenceDate.getTime());
            dest.writeDouble(mOffenceSpeed);

            dest.writeString(mOffenceLocation);
            dest.writeDouble(mOffenceAmount);
            dest.writeDouble(mOutstandingAmount);
            dest.writeDouble(mPaidAmount);

            dest.writeLong(mFirstPrintDate == null ? -1 : mFirstPrintDate.getTime());

            dest.writeString(mTransactionToken);
            dest.writeValue(mStatus);
            dest.writeTypedList(mFineEvidenceModels);
            dest.writeTypedList(mFineChargeModels);
            dest.writeByte((byte) (mChecked ? 1 : 0));
            dest.writeValue(mSearchFinesCriteriaType == null ? SearchFinesCriteriaType.Unknown: mSearchFinesCriteriaType);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "FindModel::writeToParcel()"), ErrorSeverity.High);
        }
    }

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Creator<FineModel> CREATOR = new Creator<FineModel>() {
        @Override
        public FineModel createFromParcel(Parcel in) {
            return new FineModel(in);
        }

        @Override
        public FineModel[] newArray(int size) {
            return new FineModel[size];
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

    public long getOfficerCredentialID() { return mOfficerCredentialID; }


    public String getOfficerFirstName() { return mOfficerFirstName; }
    public String getOfficerLastName() { return mOfficerLastName; }
    public String getExternalID() { return mExternalID; }

    public String getOffenderEmail() { return mOffenderEmail; }
    public String getOffenderMobileNumber() { return mOffenderMobileNumber; }

    public String getOffenderAddressLine1() { return mOffenderAddressLine1; }
    public String getOffenderAddressLine2() { return mOffenderAddressLine2; }
    public String getOffenderAddressSuburb() { return mOffenderAddressSuburb; }
    public String getOffenderAddressTown() { return mOffenderAddressTown; }

    public String getVehicleMake() { return mVehicleMake; }
    public String getVehicleModel() { return mVehicleModel; }

    public long getDistrictID() {
        return mDistrictID;
    }

    public String getOffenderIDNumber() {
        return mOffenderIDNumber;
    }

    public long getOffenderIDType() { return mOffenderIDType; }

    public String getOffenderLastName() {
        return mOffenderLastName;
    }

    public void setOffenderLastName(String offenderLastName) {
        mOffenderLastName = offenderLastName;
    }

    public String getOffenderFirstName() {
        return mOffenderFirstName;
    }

    public void setOffenderFirstName(String offenderFirstName) {
        mOffenderFirstName = offenderFirstName;
    }

    public String getVLN() {
        return mVLN;
    }

    public void setVLN(String vln) {
        mVLN = vln;
    }

    public Double getSpeedLimit() {
        return mSpeedLimit;
    }

    public Date getOffenceDate() {
        return mOffenceDate;
    }

    public void setOffenceDate(Date offenceDate) {
        mOffenceDate = offenceDate;
    }

    public Double getOffenceSpeed() {
        return mOffenceSpeed;
    }

    public String getOffenceLocation() {
        return mOffenceLocation;
    }

    public double getOffenceAmount() {
        return mOffenceAmount;
    }

    public double getOutstandingAmount() {
        return mOutstandingAmount;
    }

    public String getCourtName() { return mCourtName; }

    public Date getCourtDate() { return mCourtDate; }

    public void setOutstandingAmount(double outstandingAmount) {
        mOutstandingAmount = outstandingAmount;
    }

    public Date getFirstPrintDate(){
        return mFirstPrintDate;
    }

    public String getDistrictName(){
        return mDistrictName;
    }

    public String getPaymentOptions(){
        return mPaymentOptions;
    }

    public String getTransactionToken() {
        return mTransactionToken;
    }

    public void setTransactionToken(String transactionToken) {
        mTransactionToken = transactionToken;
    }

    public SearchFinesCriteriaType getSearchFinesCriteriaType() {
        return mSearchFinesCriteriaType;
    }

    public void setSearchFinesCriteriaType(SearchFinesCriteriaType searchFinesCriteriaType) {
        mSearchFinesCriteriaType = searchFinesCriteriaType;
    }

    public List<FineChargeModel> getFineChargeModels(){ return mFineChargeModels; }

    public List<FineEvidenceModel> getFineEvidenceModels(){ return mFineEvidenceModels; }
}
