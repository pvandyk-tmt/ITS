package za.co.kapsch.iticket;

import android.opengl.Visibility;
import android.os.Parcel;
import android.os.Parcelable;

import za.co.kapsch.iticket.Interfaces.IDelimitationRepository;
import za.co.kapsch.iticket.Interfaces.IDelimitationResult;
import za.co.kapsch.iticket.Models.DelimitationResultModel;

/**
 * Created by CSenekal on 2017/01/23.
 */

public class DelimitationDescriptor<T extends Parcelable, U extends Parcelable>  implements Parcelable {

    private String mTitle;
    private String mLevelOneLabelText;
    private String mLevelTwoLabelText;
    private String mLevelThreeLabelText;
    private String mLevelFourLabelText;
    private String mLevelFiveLabelText;
    private IDelimitationRepository mDelimitationRepository;
    private DelimitationResultModel mDelimitationResult;
    private boolean mCascade;

    private boolean mLevelOneVisibility;
    private boolean mLevelTwoVisibility;
    private boolean mLevelThreeVisibility;

    public DelimitationDescriptor(
            String title,
            String levelOneLabelText,
            String levelTwoLabelText,
            String levelThreeLabelText,
            String levelFourLabelText,
            String levelFiveLabelText,
            IDelimitationRepository delimitationRepository,
            DelimitationResultModel delimitationResult,
            boolean caseCade){

        mTitle = title;
        mLevelOneLabelText = levelOneLabelText;
        mLevelTwoLabelText = levelTwoLabelText;
        mLevelThreeLabelText = levelThreeLabelText;
        mLevelFourLabelText = levelFourLabelText;
        mLevelFiveLabelText = levelFiveLabelText;
        mDelimitationRepository =  delimitationRepository;
        mDelimitationResult = delimitationResult;
        mCascade = caseCade;
    }

    public String getTitle() {
        return mTitle;
    }

    public String getLevelOneLabelText() {
        return mLevelOneLabelText;
    }

    public String getLevelTwoLabelText() {
        return mLevelTwoLabelText;
    }

    public String getLevelThreeLabelText() {
        return mLevelThreeLabelText;
    }

    public String getLevelFourLabelText() {
        return mLevelFourLabelText;
    }

    public String getLevelFiveLabelText() {
        return mLevelFiveLabelText;
    }

    public IDelimitationRepository getDelimitationRepository() {
        return mDelimitationRepository;
    }

    public DelimitationResultModel getDelimitationResult() {
        return mDelimitationResult;
    }

    public void setResult(DelimitationResultModel result) {
        mDelimitationResult = result;
    }

    public boolean isCascade() {
        return mCascade;
    }

    public void setCascade(boolean cascade) {
        mCascade = cascade;
    }

    public boolean getLevelThreeVisibility(){ return mLevelThreeVisibility; }

    public DelimitationDescriptor(){}

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Parcelable.Creator<DelimitationDescriptor> CREATOR = new Parcelable.Creator<DelimitationDescriptor>() {
        public DelimitationDescriptor createFromParcel(Parcel in) {
            return new DelimitationDescriptor(in);
        }

        public DelimitationDescriptor[] newArray(int size) {
            return new DelimitationDescriptor[size];
        }
    };

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(mTitle);
        out.writeString(mLevelOneLabelText);
        out.writeString(mLevelTwoLabelText);
        out.writeString(mLevelThreeLabelText);
        out.writeString(mLevelFourLabelText);
        out.writeString(mLevelFiveLabelText);
        out.writeParcelable((Parcelable)mDelimitationRepository, flags);
        out.writeParcelable((Parcelable)mDelimitationResult, flags);
        out.writeByte((byte)(mCascade ? 1 : 0));
        out.writeByte((byte)(mLevelThreeVisibility ? 1 : 0));
    }

    private DelimitationDescriptor(Parcel in){
        mTitle = in.readString();
        mLevelOneLabelText = in.readString();
        mLevelTwoLabelText = in.readString();
        mLevelThreeLabelText = in.readString();
        mLevelFourLabelText = in.readString();
        mLevelFiveLabelText = in.readString();
        mDelimitationRepository = in.readParcelable(IDelimitationRepository.class.getClassLoader());
        mDelimitationResult = in.readParcelable(IDelimitationResult.class.getClassLoader());
        mCascade = in.readByte() != 0;
        mLevelThreeVisibility  = in.readByte() != 0;
    }

    private void write(Parcelable genericObject, Parcel out, int flags){

        if (genericObject == null) {
            out.writeInt(0);
        } else {
            out.writeInt(1);
            final Class<?> objectsType = genericObject.getClass();
            out.writeSerializable(objectsType);
            out.writeParcelable(genericObject, flags);
        }
    }

    private Parcelable read(Parcel in){
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
