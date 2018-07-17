package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

import za.co.kapsch.iticket.Enums.VehicleClassificationEnum;

/**
 * Created by CSenekal on 2017/05/23.
 */
public class ClassificationZoneModel implements Parcelable {

    @SerializedName("Zone")
    public int mZone;
    @SerializedName("Grace")
    public int mGrace;
    @SerializedName("Classification")
    public VehicleClassificationEnum mClassification;

    public int getZone() {
        return mZone;
    }

    public int getGrace() {
        return mGrace;
    }

    public VehicleClassificationEnum getClassification() {
        return mClassification;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeInt(mZone);
        out.writeInt(mGrace);
        out.writeValue(mClassification);
    }

    public static final Parcelable.Creator<ClassificationZoneModel> CREATOR = new Parcelable.Creator<ClassificationZoneModel>() {
        public ClassificationZoneModel createFromParcel(Parcel in) {
            return new ClassificationZoneModel(in);
        }

        public ClassificationZoneModel[] newArray(int size) {
            return new ClassificationZoneModel[size];
        }
    };

    public ClassificationZoneModel(){}

    private ClassificationZoneModel(Parcel in){
        mZone = in.readInt();
        mGrace = in.readInt();
        mClassification = (VehicleClassificationEnum)in.readValue(VehicleClassificationEnum.class.getClassLoader());
    }
}
