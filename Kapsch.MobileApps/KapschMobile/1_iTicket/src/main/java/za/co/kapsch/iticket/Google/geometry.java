package za.co.kapsch.iticket.Google;

import android.os.Parcel;
import android.os.Parcelable;

/**
 * Created by csenekal on 2016-08-07.
 */
public class geometry implements Parcelable{
    public String location_type;
    public location location;

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(location_type);
        out.writeParcelable(location, flags);
    }

    public static final Parcelable.Creator<geometry> CREATOR = new Parcelable.Creator<geometry>() {
        public geometry createFromParcel(Parcel in) {
            return new geometry(in);
        }

        public geometry[] newArray(int size) {
            return new geometry[size];
        }
    };

    public geometry(){}

    private geometry(Parcel in){
        location_type = in.readString();
        location = in.readParcelable(location.getClass().getClassLoader());
    }
}