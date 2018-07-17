package za.co.kapsch.shared.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

import za.co.kapsch.shared.Enums.EvidenceType;

/**
 * Created by CSenekal on 2017/09/05.
 */
public class FineEvidenceModel implements Parcelable {

    @SerializedName("ID")
    public long mID;

    @SerializedName("EvidenceType")
    public EvidenceType mEvidenceType;

    @SerializedName("ReferenceNumber")
    public String mReferenceNumber;

    @SerializedName("MimeType")
    public String mMimeType;

    protected FineEvidenceModel(Parcel in) {
        mID = in.readLong();
        mEvidenceType = (EvidenceType)in.readValue(EvidenceType.class.getClassLoader());
        mReferenceNumber = in.readString();
        mMimeType = in.readString();
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {
        dest.writeLong(mID);
        dest.writeValue(mEvidenceType);
        dest.writeString(mReferenceNumber);
        dest.writeString(mMimeType);
    }

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Creator<FineEvidenceModel> CREATOR = new Creator<FineEvidenceModel>() {
        @Override
        public FineEvidenceModel createFromParcel(Parcel in) {
            return new FineEvidenceModel(in);
        }

        @Override
        public FineEvidenceModel[] newArray(int size) {
            return new FineEvidenceModel[size];
        }
    };

    public long getID() { return  mID; }
}
