package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

/**
 * Created by CSenekal on 2017/05/23.
 */
public class LocationModel implements Parcelable {

    @SerializedName("Description")
    private String mDescription;
    @SerializedName("Code")
    private String mCode;
    @SerializedName("GpsLatitude")
    private String mGpsLatitude;
    @SerializedName("GpsLongitude")
    private String mGpsLongitude;

    public String getDescription() {
        return mDescription;
    }

    public String getCode() {
        return mCode;
    }

    public String getGpsLatitude() {
        return mGpsLatitude;
    }

    public String getGpsLongitude() {
        return mGpsLongitude;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(mDescription);
        out.writeString(mCode);
        out.writeString(mGpsLatitude);
        out.writeString(mGpsLongitude);
    }

    public static final Parcelable.Creator<LocationModel> CREATOR = new Parcelable.Creator<LocationModel>() {
        public LocationModel createFromParcel(Parcel in) {
            return new LocationModel(in);
        }

        public LocationModel[] newArray(int size) {
            return new LocationModel[size];
        }
    };

    public LocationModel(){}

    private LocationModel(Parcel in){
        mDescription = in.readString();
        mCode = in.readString();
        mGpsLatitude = in.readString();
        mGpsLongitude = in.readString();
    }
}
