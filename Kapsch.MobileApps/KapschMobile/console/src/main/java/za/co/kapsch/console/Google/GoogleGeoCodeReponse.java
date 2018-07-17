package za.co.kapsch.console.Google;

import android.os.Parcel;
import android.os.Parcelable;

/**
 * Created by csenekal on 2016-08-07.
 */
public class GoogleGeoCodeReponse implements Parcelable {
    public String status;
    public Results[] results;

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(status);

        int resultsSize = results.length;
        out.writeInt(resultsSize);
        for(Results result: results) {
            out.writeParcelable(result, flags);
        }
    }

    public static final Parcelable.Creator<GoogleGeoCodeReponse> CREATOR = new Parcelable.Creator<GoogleGeoCodeReponse>() {
        public GoogleGeoCodeReponse createFromParcel(Parcel in) {
            return new GoogleGeoCodeReponse(in);
        }

        public GoogleGeoCodeReponse[] newArray(int size) {
            return new GoogleGeoCodeReponse[size];
        }
    };

    public GoogleGeoCodeReponse(){}

    private GoogleGeoCodeReponse(Parcel in){
        status = in.readString();

        int resultsSize = in.readInt();
        results = new Results[resultsSize];
        for(int i = 0; i < resultsSize; i++){
            results[i] = in.readParcelable(results.getClass().getClassLoader());
        }
    }
}

