package za.co.kapsch.iticket.iCam;

import android.os.Parcel;
import android.os.Parcelable;

/**
 * Created by CSenekal on 2017/08/15.
 */
public class ICamEvent implements Parcelable{

    private ICamVlns mICamVlns;
    private ICamInfringement mICamInfringement;

    public ICamEvent(ICamVlns iCamVlns, ICamInfringement iCamInfringement){

        mICamVlns = iCamVlns;
        mICamInfringement = iCamInfringement;
    }

    protected ICamEvent(Parcel in) {

        mICamVlns = in.readParcelable(ICamVlns.class.getClassLoader());
        mICamInfringement = in.readParcelable(ICamInfringement.class.getClassLoader());
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {

        dest.writeParcelable(mICamVlns, flags);
        dest.writeParcelable(mICamInfringement, flags);
    }

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Creator<ICamEvent> CREATOR = new Creator<ICamEvent>() {
        @Override
        public ICamEvent createFromParcel(Parcel in) {
            return new ICamEvent(in);
        }

        @Override
        public ICamEvent[] newArray(int size) {
            return new ICamEvent[size];
        }
    };

    public ICamVlns getICamVlns() {
        return mICamVlns;
    }

    public void setICamVlns(ICamVlns iCamVlns) {
        mICamVlns = iCamVlns;
    }

    public ICamInfringement getICamInfringement() {
        return mICamInfringement;
    }

    public void setICamInfringement(ICamInfringement iCamInfringement) {
        mICamInfringement = iCamInfringement;
    }
}
