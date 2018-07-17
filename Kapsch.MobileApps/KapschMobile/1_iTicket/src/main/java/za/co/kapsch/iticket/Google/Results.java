package za.co.kapsch.iticket.Google;

import android.os.Parcel;
import android.os.Parcelable;

/**
 * Created by csenekal on 2016-08-07.
 */
public class Results implements Parcelable{
    public String formatted_address;
    public geometry geometry;
    public String[] types;
    public address_component[] address_components;

    @Override
    public String toString(){
        return formatted_address;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(formatted_address);
        out.writeParcelable(geometry, flags);

        int typeSize = types.length;
        out.writeInt(typeSize);
        for(String type: types) {
            out.writeString(type);
        }

        int address_componentSize = address_components.length;
        out.writeInt(address_componentSize);
        for(address_component address_component: address_components) {
            out.writeParcelable(address_component, flags);
        }
     }

    public static final Parcelable.Creator<Results> CREATOR = new Parcelable.Creator<Results>() {
        public Results createFromParcel(Parcel in) {
            return new Results(in);
        }

        public Results[] newArray(int size) {
            return new Results[size];
        }
    };

    public Results(){}

    private Results(Parcel in){
        formatted_address = in.readString();
        geometry = in.readParcelable(geometry.getClass().getClassLoader());

        int typeSize = in.readInt();
        types = new String[typeSize];
        for(int i = 0; i < typeSize; i++){
            types[i] = in.readString();
        }

        int address_componentSize = in.readInt();
        address_components = new address_component[address_componentSize];
        for(int i = 0; i < address_componentSize; i++){
            address_components[i] = in.readParcelable(address_components.getClass().getClassLoader());
        }
    }
}
