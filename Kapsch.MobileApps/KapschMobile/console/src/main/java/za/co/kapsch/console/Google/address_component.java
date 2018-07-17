package za.co.kapsch.console.Google;

import android.os.Parcel;
import android.os.Parcelable;

/**
 * Created by csenekal on 2016-08-07.
 */
public class address_component  implements Parcelable {
    public String long_name;
    public String short_name;
    public String[] types;

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(long_name);
        out.writeString(short_name);

        int typeSize = types.length;
        out.writeInt(typeSize);
        for(String type: types) {
            out.writeString(type);
        }
    }

    public static final Parcelable.Creator<address_component> CREATOR = new Parcelable.Creator<address_component>() {
        public address_component createFromParcel(Parcel in) {
            return new address_component(in);
        }

        public address_component[] newArray(int size) {
            return new address_component[size];
        }
    };

    public address_component(){}

    private address_component(Parcel in){
        long_name = in.readString();
        short_name = in.readString();

        int typeSize = in.readInt();
        types = new String[typeSize];
        for(int i = 0; i < typeSize; i++){
            types[i] = in.readString();
        }
    }
}
