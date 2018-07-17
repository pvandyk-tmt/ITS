package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

/**
 * Created by CSenekal on 2017/01/30.
 */
public class DelimitationResultModel implements Parcelable {

    private Parcelable mLevelOneObject;

    private Parcelable mLevelTwoObject;

    private Parcelable mLevelThreeObject;

    private Parcelable mLevelFourObject;

    private Parcelable mLevelFiveObject;

    public Parcelable getLevelOneObject() {
        return mLevelOneObject;
    }

    public void setLevelOneObject(Parcelable levelOneObject) {
        this.mLevelOneObject = levelOneObject;
    }

    public Parcelable getLevelTwoObject() {
        return mLevelTwoObject;
    }

    public void setLevelTwoObject(Parcelable levelTwoObject) {
        this.mLevelTwoObject = levelTwoObject;
    }

    public Parcelable getLevelThreeObject() {
        return mLevelThreeObject;
    }

    public void setLevelThreeObject(Parcelable levelThreeObject) {
        this.mLevelThreeObject = levelThreeObject;
    }

    public Parcelable getLevelFourObject() {
        return mLevelFourObject;
    }

    public void setLevelFourObject(Parcelable levelFourObject) {
        this.mLevelFourObject = levelFourObject;
    }

    public Parcelable getLevelFiveObject() {
        return mLevelFiveObject;
    }

    public void setLevelFiveObject(Parcelable levelFiveObject) {
        this.mLevelFiveObject = levelFiveObject;
    }
    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
//        write(mLevelOneObject, out, flags);
//        write(mLevelTwoObject, out, flags);
//        write(mLevelThreeObject, out, flags);
//        write(mLevelFourObject, out, flags);
//        write(mLevelFiveObject, out, flags);
    }

    public static final Parcelable.Creator<DelimitationResultModel> CREATOR = new Parcelable.Creator<DelimitationResultModel>() {
        public DelimitationResultModel createFromParcel(Parcel in) {
            return new DelimitationResultModel(in);
        }

        public DelimitationResultModel[] newArray(int size) {
            return new DelimitationResultModel[size];
        }
    };

    public DelimitationResultModel(){}

    private DelimitationResultModel(Parcel in){
//        mLevelOneObject = read(in);
//        mLevelTwoObject = read(in);
//        mLevelThreeObject = read(in);
//        mLevelFourObject = read(in);
//        mLevelFiveObject = read(in);
    }

    protected void write(Parcelable genericObject, Parcel out, int flags){

        if (genericObject == null) {
            out.writeInt(0);
        } else {
            out.writeInt(1);
            final Class<?> objectsType = genericObject.getClass();
            out.writeSerializable(objectsType);
            out.writeParcelable(genericObject, flags);
        }
    }

    protected Parcelable read(Parcel in){
        Parcelable genericObject;

        int size = in.readInt();
        if (size == 0) {
            genericObject = null;
        }else {
            Class<?> type = (Class<?>) in.readSerializable();
            genericObject = in.readParcelable(type.getClassLoader());
        }

        return genericObject;
    }
}
