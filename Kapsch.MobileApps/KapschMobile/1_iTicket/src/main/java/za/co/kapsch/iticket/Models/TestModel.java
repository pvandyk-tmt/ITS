package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

/**
 * Created by CSenekal on 2017/08/03.
 */
public class TestModel implements Parcelable {
    private Long id;
    private String value;
    private long longValue;


    protected TestModel(Parcel in) {
        value = in.readString();
        longValue = in.readLong();
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {
        dest.writeString(value);
        dest.writeLong(longValue);
    }

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Creator<TestModel> CREATOR = new Creator<TestModel>() {
        @Override
        public TestModel createFromParcel(Parcel in) {
            return new TestModel(in);
        }

        @Override
        public TestModel[] newArray(int size) {
            return new TestModel[size];
        }
    };

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }
}
