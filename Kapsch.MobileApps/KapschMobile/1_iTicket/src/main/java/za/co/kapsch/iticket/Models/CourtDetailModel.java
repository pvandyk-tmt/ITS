package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;

import za.co.kapsch.iticket.Interfaces.IDelimitation;

/**
 * Created by csenekal on 2016-07-25.
 */
@DatabaseTable(tableName = "CourtDetail")
public class CourtDetailModel implements Parcelable {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    private long mID;

    @SerializedName("CourtName")
    @DatabaseField(columnName = "CourtName")
    private String mName;

    @DatabaseField(columnName = "Room")
    private String mRoom;

    @DatabaseField(columnName = "Date")
    private Date mDate;

    //public AddressInfoModel Address;
    @SerializedName("PersonInfoID")
    @DatabaseField(columnName = "PersonInfoID")
    public Long mPersonInfoID;

    @SerializedName("ContemptAmount")
    @DatabaseField(columnName = "ContemptAmount")
    public Long mContemptAmount;

    @SerializedName("ContemptDays")
    @DatabaseField(columnName = "ContemptDays")
    public Long mContemptDays;

    @SerializedName("BankingInfoID")
    @DatabaseField(columnName = "BankingInfoID")
    public Long mBankingInfoID;

    @SerializedName("DistrictID")
    @DatabaseField(columnName = "DistrictID")
    public long mDistrictID;

    @SerializedName("CasePre")
    @DatabaseField(columnName = "CasePre")
    public String mCasePre;

    @SerializedName("CasePost")
    @DatabaseField(columnName = "CasePost")
    public String mCasePost;

    @SerializedName("SequenceName")
    @DatabaseField(columnName = "SequenceName")
    public String mSequenceName;

    @SerializedName("StatusId")
    @DatabaseField(columnName = "StatusId")
    public long mStatusId;

    @SerializedName("WarrantPre")
    @DatabaseField(columnName = "WarrantPre")
    public String mWarrantPre;

    @SerializedName("WarrantPost")
    @DatabaseField(columnName = "WarrantPost")
    public String mWarrantPost;

    @SerializedName("CaptureDate")
    @DatabaseField(columnName = "CaptureDate")
    public Date mCaptureDate;

    @SerializedName("TypeOfServiceAllowed")
    @DatabaseField(columnName = "TypeOfServiceAllowed")
    public Long mTypeOfServiceAllowed;

    @SerializedName("WarrantLetterGrace")
    @DatabaseField(columnName = "WarrantLetterGrace")
    public Long mWarrantLetterGrace;

    @SerializedName("WarrantExpireDays")
    @DatabaseField(columnName = "WarrantExpireDays")
    public Long mWarrantExpireDays;

    @SerializedName("SummonsExpireDays")
    @DatabaseField(columnName = "SummonsExpireDays")
    public Long mSummonsExpireDays;

    @SerializedName("UserId")
    @DatabaseField(columnName = "UserId")
    public Long mUserId;

    @SerializedName("CourtTime")
    @DatabaseField(columnName = "CourtTime")
    public String mCourtTime;

    @SerializedName("DaysToCourtDate")
    @DatabaseField(columnName = "DaysToCourtDate")
    public Long mDaysToCourtDate;

    @SerializedName("OverAllocation")
    @DatabaseField(columnName = "OverAllocation")
    public Long mOverAllocation;

    @SerializedName("ReIssueInvalidServing")
    @DatabaseField(columnName = "ReIssueInvalidServing")
    public long mReIssueInvalidServing;


    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {

        out.writeLong(mID);
        out.writeString(mName);
        out.writeLong(mPersonInfoID);
        out.writeLong(mContemptAmount);
        out.writeLong(mContemptDays);
        out.writeLong(mBankingInfoID);
        out.writeLong(mDistrictID);
        out.writeString(mCasePre);
        out.writeString(mCasePost);
        out.writeString(mSequenceName);
        out.writeLong(mStatusId);
        out.writeString(mWarrantPre);
        out.writeString(mWarrantPost);
        out.writeLong(mCaptureDate == null ? -1 : mCaptureDate.getTime());
        out.writeLong(mTypeOfServiceAllowed);
        out.writeLong(mWarrantLetterGrace);
        out.writeLong(mWarrantExpireDays);
        out.writeLong(mSummonsExpireDays);
        out.writeLong(mUserId);
        out.writeString(mCourtTime);
        out.writeLong(mDaysToCourtDate);
        out.writeLong(mOverAllocation);
        out.writeLong(mReIssueInvalidServing);
    }

    public static final Parcelable.Creator<CourtDetailModel> CREATOR = new Parcelable.Creator<CourtDetailModel>() {
        public CourtDetailModel createFromParcel(Parcel in) {
            return new CourtDetailModel(in);
        }

        public CourtDetailModel[] newArray(int size) {
            return new CourtDetailModel[size];
        }
    };

    public CourtDetailModel(){}

    private CourtDetailModel(Parcel in){
        mID = in.readLong();
        mName = in.readString();
        mPersonInfoID = in.readLong();
        mContemptAmount = in.readLong();
        mContemptDays = in.readLong();
        mBankingInfoID = in.readLong();
        mDistrictID = in.readLong();
        mCasePre = in.readString();
        mCasePost = in.readString();
        mSequenceName = in.readString();
        mStatusId = in.readLong();
        mWarrantPre = in.readString();
        mWarrantPost = in.readString();
        long tmpDate = in.readLong();
        mCaptureDate =  tmpDate == -1 ? null : new Date(tmpDate);
        mTypeOfServiceAllowed = in.readLong();
        mWarrantLetterGrace = in.readLong();
        mWarrantExpireDays = in.readLong();
        mSummonsExpireDays = in.readLong();
        mUserId = in.readLong();
        mCourtTime = in.readString();
        mDaysToCourtDate = in.readLong();
        mOverAllocation = in.readLong();
        mReIssueInvalidServing = in.readLong();
    }

    public String getName() {
        return mName;
    }

    public void setName(String mCourtName) {
        this.mName = mCourtName;
    }

    public String getRoom() {
        return mRoom;
    }

    public void setRoom(String mRoom) {
        this.mRoom = mRoom;
    }

    public Date getDate() {
        return mDate;
    }

    public void setDate(Date mDate) {
        this.mDate = mDate;
    }
}

