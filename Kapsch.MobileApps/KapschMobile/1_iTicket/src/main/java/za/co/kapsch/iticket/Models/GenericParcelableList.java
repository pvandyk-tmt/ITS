package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by CSenekal on 2017/01/23.
 */
public class GenericParcelableList<T extends Parcelable> implements Parcelable {

    private String mFieldName;
    private List<T> mItems;

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Parcelable.Creator<GenericParcelableList> CREATOR = new Parcelable.Creator<GenericParcelableList>() {
        public GenericParcelableList createFromParcel(Parcel in) {
            return new GenericParcelableList(in);
        }

        public GenericParcelableList[] newArray(int size) {
            return new GenericParcelableList[size];
        }
    };

    public GenericParcelableList(){}

    private GenericParcelableList(Parcel in){
        mFieldName = in.readString();

        int size = in.readInt();
        if (size == 0) {
            mItems = null;
        }else {
            Class<?> type = (Class<?>) in.readSerializable();
            mItems = new ArrayList<>(size);
            in.readList(mItems, type.getClassLoader());
        }
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(mFieldName);

        if (mItems == null || mItems.size() == 0) {
            out.writeInt(0);
        } else {
            out.writeInt(mItems.size());
            final Class<?> objectsType = mItems.get(0).getClass();
            out.writeSerializable(objectsType);
            out.writeList(mItems);
        }
    }
}
