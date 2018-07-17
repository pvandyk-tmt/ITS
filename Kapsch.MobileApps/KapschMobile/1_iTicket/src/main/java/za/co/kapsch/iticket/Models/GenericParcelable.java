package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

/**
 * Created by CSenekal on 2017/01/23.
 */
public class GenericParcelable<T extends Parcelable> implements Parcelable {

    private T mItem;

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Parcelable.Creator<GenericParcelable> CREATOR = new Parcelable.Creator<GenericParcelable>() {
        public GenericParcelable createFromParcel(Parcel in) {
            return new GenericParcelable(in);
        }

        public GenericParcelable[] newArray(int size) {
            return new GenericParcelable[size];
        }
    };

    public GenericParcelable(){}

    private GenericParcelable(Parcel in){
        int size = in.readInt();
        if (size == 0) {
            mItem = null;
        }else {
            Class<?> type = (Class<?>) in.readSerializable();
            mItem = in.readParcelable(type.getClassLoader());
        }
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {

        if (mItem == null) {
            out.writeInt(0);
        } else {
            final Class<?> objectsType = mItem.getClass();
            out.writeSerializable(objectsType);
            out.writeParcelable(mItem, flags);
        }
    }
}
