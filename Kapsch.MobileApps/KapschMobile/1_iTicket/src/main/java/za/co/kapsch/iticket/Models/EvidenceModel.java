package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DataType;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;

import za.co.kapsch.iticket.Enums.EvidenceType;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.Enums.ErrorSeverity;

/**
 * Created by csenekal on 2016-10-20.
 */
@DatabaseTable(tableName = "Evidence")
public class EvidenceModel implements Parcelable {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", generatedId = true)
    private int mID;

    @SerializedName("TicketNumber")
    @DatabaseField(columnName = "TicketNumber")
    private String mTicketNumber;

    @SerializedName("EvidenceType")
    @DatabaseField(columnName = "EvidenceType", dataType = DataType.ENUM_INTEGER)
    private EvidenceType mEvidenceType;

    @SerializedName("Evidence")
    @DatabaseField(columnName = "Evidence", dataType = DataType.BYTE_ARRAY)
    private byte[] mEvidence;

    @SerializedName("Uploaded")
    @DatabaseField(columnName = "Uploaded")
    private boolean mUploaded;

    @SerializedName("UploadDateTime")
    @DatabaseField(columnName = "UploadDateTime")
    private Date mUploadDateTime;

    @SerializedName("EvidenceDate")
    @DatabaseField(columnName = "EvidenceDate")
    private Date mEvidenceDate;

    public int getID() {
        return mID;
    }

    public void setID(int ID) {
        mID = ID;
    }

    public String getTicketNumber() {
        return mTicketNumber;
    }

    public void setTicketNumber(String ticketNumber) {
        mTicketNumber = ticketNumber;
    }

    public EvidenceType getEvidenceType() {
        return mEvidenceType;
    }

    public void setEvidenceType(EvidenceType evidenceType) {
        mEvidenceType = evidenceType;
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

    @Override
    public String toString(){
        return getServerFilename();
    }

    public static EvidenceModel getEvidence(EvidenceType evidenceType, byte[] data, String ticketNumber){
        if (data == null) return null;

        EvidenceModel evidence = new EvidenceModel();
        evidence.setEvidence(data);
        evidence.setTicketNumber(ticketNumber);
        evidence.setEvidenceType(evidenceType);
        evidence.setUploaded(false);
        evidence.setUploadDateTime(null);
        evidence.setEvidenceDate(Calendar.getInstance().getTime());
        return evidence;
    }

    public String getServerFilename(){
        String[] ticketNumberSplit = mTicketNumber.split("/");

        String ticketNumber = String.format("%s-%s-%s-%s", ticketNumberSplit[0], ticketNumberSplit[1], ticketNumberSplit[2], ticketNumberSplit[3]);

        SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyyMMdd");
        String date = simpleDateFormat.format(mEvidenceDate);

        String ext = mEvidenceType == EvidenceType.VoiceRecording ? "mp4" : "jpg";

        return String.format("%s.%s.%d.%s.%s",
                ticketNumber,
                EvidenceType.evidenceToString(mEvidenceType),
                mID,
                date,
                ext);
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        try {
            out.writeInt(mID);
            out.writeString(mTicketNumber);
            out.writeValue(mEvidenceType);
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

    public static final Parcelable.Creator<EvidenceModel> CREATOR = new Parcelable.Creator<EvidenceModel>() {
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
        mTicketNumber = in.readString();
        mEvidenceType = (EvidenceType)in.readValue(EvidenceType.class.getClassLoader());
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
