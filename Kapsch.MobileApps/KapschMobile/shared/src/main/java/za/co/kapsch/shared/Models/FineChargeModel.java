package za.co.kapsch.shared.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/09/05.
 */
public class FineChargeModel implements Parcelable{

    @SerializedName("Code")
    public String Code;

    @SerializedName("Description")
    public String Description;

    @SerializedName("SecondaryDescription")
    public String SecondaryDescription;

    @SerializedName("ShortDescription")
    public String ShortDescription;

    @SerializedName("RegulationDescription")
    public String RegulationDescription;

    @SerializedName("FineAmount")
    public double FineAmount;

    protected FineChargeModel(Parcel in) {
        Code = in.readString();
        Description = in.readString();
        SecondaryDescription = in.readString();
        ShortDescription = in.readString();
        RegulationDescription = in.readString();
        FineAmount = in.readDouble();
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {
        dest.writeString(Code);
        dest.writeString(Description);
        dest.writeString(SecondaryDescription);
        dest.writeString(ShortDescription);
        dest.writeString(RegulationDescription);
        dest.writeDouble(FineAmount);
    }

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Creator<FineChargeModel> CREATOR = new Creator<FineChargeModel>() {
        @Override
        public FineChargeModel createFromParcel(Parcel in) {
            return new FineChargeModel(in);
        }

        @Override
        public FineChargeModel[] newArray(int size) {
            return new FineChargeModel[size];
        }
    };

    public String getCode() {
        return Code;
    }

    public void setCode(String code) {
        Code = code;
    }

    public String getDescription() {
        return Description;
    }

    public void setDescription(String description) {
        Description = description;
    }

    public Double getFineAmount() {
        return FineAmount;
    }

    public void setFineAmount(Double fineAmount) {
        FineAmount = fineAmount;
    }

    public String getSecondaryDescription() {
        return SecondaryDescription;
    }

    public void setSecondaryDescription(String secondaryDescription) {
        SecondaryDescription = secondaryDescription;
    }

    public String getShortDescription() {
        return ShortDescription;
    }

    public void setShortDescription(String shortDescription) {
        ShortDescription = shortDescription;
    }

    public String getRegulationDescription() {
        return RegulationDescription;
    }

    public void setRegulationDescription(String regulationDescription) {
        RegulationDescription = regulationDescription;
    }
}
