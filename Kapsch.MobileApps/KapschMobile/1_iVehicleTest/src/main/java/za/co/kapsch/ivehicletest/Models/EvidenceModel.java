package za.co.kapsch.ivehicletest.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DataType;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Calendar;
import java.util.Date;

import za.co.kapsch.ivehicletest.Enums.InspectionEvidenceType;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2016-10-20.
 */
@DatabaseTable(tableName = "Evidence")
public class EvidenceModel implements Parcelable {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", generatedId = true)
    private int mID;

    @SerializedName("BookingID")
    @DatabaseField(columnName = "BookingID")
    private long mBookingID;

    @SerializedName("InspectionEvidenceType")
    @DatabaseField(columnName = "EvidenceType", dataType = DataType.ENUM_INTEGER)
    private InspectionEvidenceType mInspectionEvidenceType;

    @SerializedName("Evidence")
    @DatabaseField(columnName = "Evidence", dataType = DataType.BYTE_ARRAY)
    private byte[] mEvidence;

    @SerializedName("SiteID")
    @DatabaseField(columnName = "SiteID")
    private long mSiteID;

    @DatabaseField(columnName = "Uploaded")
    private transient boolean mUploaded;

    @DatabaseField(columnName = "UploadDateTime")
    private transient Date mUploadDateTime;

    @DatabaseField(columnName = "EvidenceDate")
    private transient Date mEvidenceDate;

    @DatabaseField(columnName = "Submit")
    private transient boolean mSubmit;

    public int getID() {
        return mID;
    }

    public void setID(int ID) {
        mID = ID;
    }

    public long getBookingID() {
        return mBookingID;
    }

    public void setBookingID(long bookingID) {
        mBookingID = bookingID;
    }

    public long getSiteID() {
        return mSiteID;
    }

    public void setSiteID(long siteID) {
        mSiteID = siteID;
    }

    public InspectionEvidenceType getInspectionEvidenceType() {
        return mInspectionEvidenceType;
    }

    public void setEvidenceType(InspectionEvidenceType inspectionEvidenceType) {
        mInspectionEvidenceType = inspectionEvidenceType;
    }

    public byte[] getEvidence() {
        return mEvidence;
    }

    public void setEvidence(byte[] evidence) {
        mEvidence = evidence;
    }

    public boolean isUploaded() {
        return mUploaded;
    }

    public void setUploaded(boolean uploaded) {
        mUploaded = uploaded;
    }

    public Date getUploadDateTime() {
        return mUploadDateTime;
    }

    public void setUploadDateTime(Date uploadDateTime) {
        mUploadDateTime = uploadDateTime;
    }

    public Date getEvidenceDate() {
        return mEvidenceDate;
    }

    public void setEvidenceDate(Date evidenceDate) {
        mEvidenceDate = evidenceDate;
    }

    public void setSubmit(boolean submit) {
        mSubmit = submit;
    }

    public boolean getSubmit() {
        return mSubmit;
    }

    public static EvidenceModel getEvidence(InspectionEvidenceType evidenceType, byte[] data, long bookingID, long siteID){
        if (data == null) return null;

        EvidenceModel evidence = new EvidenceModel();
        evidence.setEvidence(data);
        evidence.setBookingID(bookingID);
        evidence.setSiteID(siteID);
        evidence.setEvidenceType(evidenceType);
        evidence.setUploaded(false);
        evidence.setUploadDateTime(null);
        evidence.setEvidenceDate(Calendar.getInstance().getTime());
        return evidence;
    }

      @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        try {
            out.writeInt(mID);
            out.writeLong(mBookingID);
            out.writeValue(mInspectionEvidenceType);
            out.writeLong(mSiteID);
            out.writeByte((byte) (mUploaded ? 1 : 0));
            out.writeLong(mUploadDateTime == null ? -1 : mUploadDateTime.getTime());
            out.writeLong(mEvidenceDate == null ? -1 : mEvidenceDate.getTime());

            if (mEvidence != null) {
                out.writeInt(mEvidence.length);
                if (mEvidence != null) out.writeByteArray(mEvidence);
            } else {
                out.writeInt(0);
            }

        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "TicketModel(Parcel in)"), ErrorSeverity.High);
        }
    }

    public static final Creator<EvidenceModel> CREATOR = new Creator<EvidenceModel>() {
        public EvidenceModel createFromParcel(Parcel in) {
            return new EvidenceModel(in);
        }

        public EvidenceModel[] newArray(int size) {
            return new EvidenceModel[size];
        }
    };

    public EvidenceModel(){}

    private EvidenceModel(Parcel in){
        mID = in.readInt();
        mBookingID = in.readLong();
        mInspectionEvidenceType = (InspectionEvidenceType)in.readValue(InspectionEvidenceType.class.getClassLoader());
        mSiteID = in.readLong();
        mUploaded = in.readByte() != 0;
        long uploadDateTime = in.readLong();
        mUploadDateTime = uploadDateTime == -1 ? null : new Date(uploadDateTime);

        long evidenceDateTime = in.readLong();
        mEvidenceDate = evidenceDateTime == -1 ? null : new Date(evidenceDateTime);

        int evidenceLength = in.readInt();
        if (evidenceLength > 0) {
            mEvidence = new byte[evidenceLength];
            in.readByteArray(mEvidence);
        }
    }
}
