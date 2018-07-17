package za.co.kapsch.ivehicletest.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

import java.util.List;

import za.co.kapsch.ivehicletest.Enums.QuestionType;

import static za.co.kapsch.ivehicletest.Enums.QuestionType.MultipleChoice;

/**
 * Created by csenekal on 2017/11/29.
 */

public class VehicleInspectionQuestionModel implements Parcelable {

    @SerializedName("VehicleTestBookingID")
    private long mBookingID;

    @SerializedName("TestQuestionID")
    private long mTestQuestionID;

    @SerializedName("TestQuestionDescription")
    private String mTestQuestionDescription;

    @SerializedName("QuestionTypeID")
    private long mQuestionTypeID;

    @SerializedName("ValuePattern")
    private String mValuePattern;

    @SerializedName("HasComment")
    private boolean mHasComment;

    @SerializedName("IsDoubleEntry")
    private boolean mIsDoubleEntry;

    @SerializedName("IsReadOnly")
    private boolean mIsReadOnly;

    @SerializedName("IsCompared")
    private boolean mIsCompared;

    @SerializedName("Weight")
    private double mWeight;

    @SerializedName("Citeria")
    private String mCiteria;

    @SerializedName("ComparedValue")
    private String mComparedValue;

    @SerializedName("CorrectQuestionAnswerID")
    private String mCorrectQuestionAnswerID;

    @SerializedName("Answers")
    private List<VehicleInspectionAnswerModel> mVehicleInspectionAnswerList;

    private transient boolean mPhotoRequired;

    public String getCorrectQuestionAnswerID() {
        return mCorrectQuestionAnswerID;
    }

    public void setCorrectQuestionAnswerID(String correctQuestionAnswerID) {
        mCorrectQuestionAnswerID = correctQuestionAnswerID;
    }

    public boolean isCorrectQuestionAnswerID(long ID){

        String[] correctQuestionAnswerIDs = mCorrectQuestionAnswerID.split(",");
        for (String correctQuestionAnswerID : correctQuestionAnswerIDs) {
            if (ID == Long.parseLong(correctQuestionAnswerID.trim())){
                return true;
            }
        }

        return false;
    }

    public long getBookingID(){
        return mBookingID;
    }

    public void setBookingID(long bookingID){
        mBookingID = bookingID;
    }

    public long getTestQuestionID(){
        return mTestQuestionID;
    }

    public long getQuestionTypeID(){
        return mQuestionTypeID;
    }

    public String getTestQuestionDescription() {
        return mTestQuestionDescription;
    }

    public void setTestQuestionDescription(String testQuestionDescription) {
        mTestQuestionDescription = testQuestionDescription;
    }

    public boolean getHasComment() {
        return mHasComment;
    }

    public boolean getIsDoubleEntry() {
        return mIsDoubleEntry;
    }

    public boolean getIsReadOnly() {
        return mIsReadOnly;
    }

    public boolean getIsCompared() {
        return mIsCompared;
    }

    public boolean isPhotoRequired() {
        return mPhotoRequired;
    }

    public void setPhotoRequired(boolean photoRequired) {
        mPhotoRequired = photoRequired;
    }

    public QuestionType getQuestionType(){

        switch((int)mQuestionTypeID) {

            case 1: return QuestionType.Text;
            case 2: return QuestionType.MultipleChoice;
            case 3: return QuestionType.TextMultipleChoice;
            case 4: return QuestionType.ChangeMultipleChoice;
            default: return null;
        }
    }

    public String getValuePattern(){
        return mValuePattern;
    }

    public String getComparedValue(){
        return  mComparedValue;
    }

    public Double getWeight(){
        return mWeight;
    }

    public List<VehicleInspectionAnswerModel> getVehicleInspectionAnswerList() {
        return mVehicleInspectionAnswerList;
    }

    public void setVehicleInspectionAnswerList(List<VehicleInspectionAnswerModel> vehicleInspectionAnswerList){
        mVehicleInspectionAnswerList = vehicleInspectionAnswerList;
    }

    public VehicleInspectionQuestionModel() {}

    protected VehicleInspectionQuestionModel(Parcel in) {
        mBookingID = in.readLong();
        mTestQuestionID = in.readLong();
        mTestQuestionDescription = in.readString();
        mQuestionTypeID = in.readLong();
        mValuePattern = in.readString();
        mHasComment = in.readByte() != 0;
        mIsDoubleEntry = in.readByte() != 0;
        mIsReadOnly = in.readByte() != 0;
        mIsCompared = in.readByte() != 0;
        mWeight = in.readDouble();
        mCiteria = in.readString();
        mComparedValue = in.readString();
        mCorrectQuestionAnswerID = in.readString();
        mVehicleInspectionAnswerList = in.createTypedArrayList(VehicleInspectionAnswerModel.CREATOR);
        mPhotoRequired = in.readByte() != 0;
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {
        dest.writeLong(mBookingID);
        dest.writeLong(mTestQuestionID);
        dest.writeString(mTestQuestionDescription);
        dest.writeLong(mQuestionTypeID);
        dest.writeString(mValuePattern);
        dest.writeByte((byte) (mHasComment ? 1 : 0));
        dest.writeByte((byte) (mIsDoubleEntry ? 1 : 0));
        dest.writeByte((byte) (mIsReadOnly ? 1 : 0));
        dest.writeByte((byte) (mIsCompared ? 1 : 0));
        dest.writeDouble(mWeight);
        dest.writeString(mCiteria);
        dest.writeString(mComparedValue);
        dest.writeString(mCorrectQuestionAnswerID);
        dest.writeTypedList(mVehicleInspectionAnswerList);
        dest.writeByte((byte) (mPhotoRequired ? 1 : 0));
    }

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Creator<VehicleInspectionQuestionModel> CREATOR = new Creator<VehicleInspectionQuestionModel>() {
        @Override
        public VehicleInspectionQuestionModel createFromParcel(Parcel in) {
            return new VehicleInspectionQuestionModel(in);
        }

        @Override
        public VehicleInspectionQuestionModel[] newArray(int size) {
            return new VehicleInspectionQuestionModel[size];
        }
    };
}
