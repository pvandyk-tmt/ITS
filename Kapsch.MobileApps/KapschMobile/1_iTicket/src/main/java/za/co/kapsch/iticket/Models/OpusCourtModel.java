package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/01/23.
 */
public class OpusCourtModel implements Parcelable {

    @SerializedName("RefCourt")
    private int mRefCourt;
    @SerializedName("CourtName")
    private String mCourtName;
    @SerializedName("CourtNumber")
    private String mCourtNumber;
    @SerializedName("Address_RefTown")
    private int mAddressRefTown;
    @SerializedName("Address_Suburb")
    private String mAddressSuburb;
    @SerializedName("Address_Line1")
    private String mAddressLine1;
    @SerializedName("Address_Line2")
    private String mAddressLine2;
    @SerializedName("Address_PostalCode")
    private String mAddressPostalCode;
    @SerializedName("PaymentAddress_RefTown")
    private int mPaymentAddressRefTown;
    @SerializedName("PaymentAddress_Suburb")
    private String mPaymentAddressSuburb;
    @SerializedName("PaymentAddress_Line1")
    private String mPaymentAddressLine1;
    @SerializedName("PaymentAddress_Line2")
    private String mPaymentAddressLine2;
    @SerializedName("PaymentAddress_PostalCode")
    private String mPaymentAddressPostalCode;
    @SerializedName("RefAuthority")
    private int mRefAuthority;
    @SerializedName("CourtFullName")
    private String mCourtFullName;

    public int getRefCourt() {
        return mRefCourt;
    }

    public String getCourtName() {
        return mCourtName;
    }

    public String getCourtNumber() {
        return mCourtNumber;
    }

    public int getAddress_RefTown() {
        return mAddressRefTown;
    }

    public String getAddress_Suburb() {
        return mAddressSuburb;
    }

    public String getAddress_Line1() {
        return mAddressLine1;
    }

    public String getAddress_Line2() {
        return mAddressLine2;
    }

    public String getAddress_PostalCode() {
        return mAddressPostalCode;
    }

    public int getPaymentAddress_RefTown() {
        return mPaymentAddressRefTown;
    }

    public String getPaymentAddress_Suburb() {
        return mPaymentAddressSuburb;
    }

    public String getPaymentAddress_Line1() {
        return mPaymentAddressLine1;
    }

    public String getPaymentAddress_Line2() {
        return mPaymentAddressLine2;
    }

    public String getPaymentAddress_PostalCode() {
        return mPaymentAddressPostalCode;
    }

    public int getRefAuthority() {
        return mRefAuthority;
    }

    public String getCourtFullName() {
        return mCourtFullName;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {

        out.writeInt(mRefCourt);
        out.writeString(mCourtName);
        out.writeString(mCourtNumber);
        out.writeInt(mAddressRefTown);
        out.writeString(mAddressSuburb);
        out.writeString(mAddressLine1);
        out.writeString(mAddressLine2);
        out.writeString(mAddressPostalCode);
        out.writeInt(mPaymentAddressRefTown);
        out.writeString(mPaymentAddressSuburb);
        out.writeString(mPaymentAddressLine1);
        out.writeString(mPaymentAddressLine2);
        out.writeString(mPaymentAddressPostalCode);
        out.writeInt(mRefAuthority);
        out.writeString(mCourtFullName);
    }

    public static final Parcelable.Creator<OpusCourtModel> CREATOR = new Parcelable.Creator<OpusCourtModel>() {
        public OpusCourtModel createFromParcel(Parcel in) {
            return new OpusCourtModel(in);
        }

        public OpusCourtModel[] newArray(int size) {
            return new OpusCourtModel[size];
        }
    };

    public OpusCourtModel(){}

    private OpusCourtModel(Parcel in){
        mRefCourt = in.readInt();
        mCourtName = in.readString();
        mCourtNumber = in.readString();
        mAddressRefTown = in.readInt();
        mAddressSuburb = in.readString();
        mAddressLine1 = in.readString();
        mAddressLine2 = in.readString();
        mAddressPostalCode = in.readString();
        mPaymentAddressRefTown = in.readInt();
        mPaymentAddressSuburb = in.readString();
        mPaymentAddressLine1 = in.readString();
        mPaymentAddressLine2 = in.readString();
        mPaymentAddressPostalCode = in.readString();
        mRefAuthority = in.readInt();
        mCourtFullName = in.readString();
    }
}
