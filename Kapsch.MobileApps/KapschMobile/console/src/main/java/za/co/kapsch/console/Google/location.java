package za.co.kapsch.console.Google;

import android.os.Parcel;
import android.os.Parcelable;

/**
 * Created by csenekal on 2016-08-07.
 */
public class location  implements Parcelable {
    public String lat;
    public String lng;

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(lat);
        out.writeString(lng);
    }

    public static final Parcelable.Creator<location> CREATOR = new Parcelable.Creator<location>() {
        public location createFromParcel(Parcel in) {
            return new location(in);
        }

        public location[] newArray(int size) {
            return new location[size];
        }
    };

    public location(){}

    private location(Parcel in){
        lat = in.readString();
        lng = in.readString();
    }
}
