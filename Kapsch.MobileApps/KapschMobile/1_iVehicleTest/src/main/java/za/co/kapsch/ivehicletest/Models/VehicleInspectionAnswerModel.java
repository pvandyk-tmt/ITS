package za.co.kapsch.ivehicletest.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

/**
 * Created by csenekal on 2017/11/29.
 */

public class VehicleInspectionAnswerModel implements Parcelable {

    @SerializedName("RelationshipID")
    private long mRelationshipID;

    @SerializedName("TestQuestionID")
    private long mTestQuestionID;

    @SerializedName("TestQuestionAnswerID")
    private Long mTestQuestionAnswerID;

    @SerializedName("QuestionAnswerDescription")
    private String mQuestionAnswerDescription;

    @SerializedName("NextQuestionID")
    private long mNextQuestionID;

    @SerializedName("DisplayColour")
    private String mDisplayColour;

    @SerializedName("IsCommentRequired")
    private boolean mIsCommentRequired;

    public void setNextQuestionID(long nextQuestionID){
        mNextQuestionID = nextQuestionID;
    }

    public void setQuestionAnswerDescription(String questionAnswerDescription){
        mQuestionAnswerDescription = questionAnswerDescription;
    }

    public long getRelationshipID() {
        return mRelationshipID;
    }

    public long getTestQuestionId() {
        return mTestQuestionID;
    }

    public Long getTestQuestionAnswerID() {
        return mTestQuestionAnswerID;
    }

    public void setTestQuestionAnswerID(Long testQuestionAnswerID){
         mTestQuestionAnswerID = testQuestionAnswerID;
    }

    public String getQuestionAnswerDescription() {
        return mQuestionAnswerDescription;
    }

    public long getNextQuestionId() {
        return mNextQuestionID;
    }

    public String getDisplayColour() { return mDisplayColour; }

    public boolean getIsCommentRequired() {
        return mIsCommentRequired;
    }

    public void setIsCommentRequired(boolean isCommentRequired) {
        mIsCommentRequired = isCommentRequired;
    }

    public VehicleInspectionAnswerModel() {}

    protected VehicleInspectionAnswerModel(Parcel in) {
        mRelationshipID = in.readLong();
        mTestQuestionID = in.readLong();
        mTestQuestionAnswerID = in.readLong();
        mQuestionAnswerDescription = in.readString();
        mNextQuestionID = in.readLong();
        mDisplayColour = in.readString();
        mIsCommentRequired = in.readByte() != 0;
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {
        dest.writeLong(mRelationshipID);
        dest.writeLong(mTestQuestionID);
        dest.writeLong(mTestQuestionAnswerID);
        dest.writeString(mQuestionAnswerDescription);
        dest.writeLong(mNextQuestionID);
        dest.writeString(mDisplayColour);
        dest.writeByte((byte) (mIsCommentRequired ? 1 : 0));
    }

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Creator<VehicleInspectionAnswerModel> CREATOR = new Creator<VehicleInspectionAnswerModel>() {
        @Override
        public VehicleInspectionAnswerModel createFromParcel(Parcel in) {
            return new VehicleInspectionAnswerModel(in);
        }

        @Override
        public VehicleInspectionAnswerModel[] newArray(int size) {
            return new VehicleInspectionAnswerModel[size];
        }
    };
}


