package za.co.kapsch.ivehicletest.Models;

import android.graphics.Color;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import za.co.kapsch.ivehicletest.Enums.QuestionType;

/**
 * Created by csenekal on 2017/12/06.
 */

@DatabaseTable(tableName = "VehicleInspectionResult")
public class VehicleInspectionResultModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", generatedId = true)
    private long mID;

    //Foreign Key to primary key/ID of VehicleInspectionResults
    @DatabaseField(columnName = "VehicleInspectionResultsID")
    private long mVehicleInspectionResultsID;

    @DatabaseField(columnName = "Uploaded")
    private transient boolean mUploaded;

    @SerializedName("VehicleTestBookingID")
    @DatabaseField(columnName = "BookingID")
    private long mBookingID;

    @SerializedName("TestTypeID")
    @DatabaseField(columnName = "TestTypeID")
    private long mTestTypeID;

    @SerializedName("TestQuestionsID")
    @DatabaseField(columnName = "TestQuestionID")
    private long mTestQuestionID;

    @SerializedName("TextAnswer")
    @DatabaseField(columnName = "Answer")
    private String mAnswer;

    @SerializedName("TestQuestionsAnswersID")
    @DatabaseField(columnName = "TestQuestionAnswerID")
    private Long mTestQuestionAnswerID;

    @SerializedName("TestQuestionsAnswersIDRelID")
    @DatabaseField(columnName = "RelationshipID")
    private Long mRelationshipID;

    @SerializedName("Comments")
    @DatabaseField(columnName = "Comments")
    private String mComments;

    @SerializedName("IsPassed")
    @DatabaseField(columnName = "IsPassed")
    private int mIsPassed;

    private transient double mWeight;

    private transient long mQuestionTypeID;

    private String mPassFailedText;

    public long getID(){
        return mID;
    }

    public void setID(long id){
        mID = id;
    }

    public long getVehicleInspectionResultsID() {
        return mVehicleInspectionResultsID;
    }

    public void setVehicleInspectionResultsID(long vehicleInspectionResultsID) {
        mVehicleInspectionResultsID = vehicleInspectionResultsID;
    }

    private transient int mDisplayColour;

    private transient int mColour;

    private transient String mCompareValue;

    private transient String mQuestion;

    private transient String mMultipleChoiceAnswer;

    public boolean getUploaded(){
        return mUploaded;
    }

    public void setUploaded(boolean uploaded){
        mUploaded = uploaded;
    }

    public long getBookingID() {
        return mBookingID;
    }

    public void setBookingID(long bookingID) {
        mBookingID = bookingID;
    }

    public long getTestQuestionID() {
        return mTestQuestionID;
    }

    public void setTestTypeID(long testTypeID) {
        mTestTypeID = testTypeID;
    }

    public void setTestQuestionID(long testQuestionID) {
        mTestQuestionID = testQuestionID;
    }

    public void setAnswer(String answer) {
        mAnswer = answer;
    }

    public String getAnswer(){
        return mAnswer;
    }

    public void setTestQuestionAnswerID(Long testQuestionAnswerID) {
        mTestQuestionAnswerID = testQuestionAnswerID;
    }

    public Long getTestQuestionAnswerID() {
        return mTestQuestionAnswerID;
    }

    public void setRelationshipID(Long relationshipID) {
        mRelationshipID = relationshipID;
    }

    public Long getRelationshipID(){
        return mRelationshipID;
    }

    public void setComments(String comments) {
        mComments = comments;
    }

    public String getComments(){
        return mComments;
    }

    public void setDisplayColour(int displayColour){
        mDisplayColour = displayColour;
    }

    public int getDisplayColour(){
        return mDisplayColour;
    }

    public void setQuestion(String question){
        mQuestion = question;
    }

    public String getQuestion(){
        return mQuestion;
    }

    public void setMultipleChoiceAnswer(String multipleChoiceAnswer){
        mMultipleChoiceAnswer = multipleChoiceAnswer;
    }

    public String getMultipleChoiceAnswer(){
        return mMultipleChoiceAnswer;
    }

    public String getCompareValue() {
        return mCompareValue;
    }

    public void setCompareValue(String compareValue) {
        mCompareValue = compareValue;
    }

    public int getIsPassed() {
        return mIsPassed;
    }

    public void setIsPassed(int isPassed) {
        mIsPassed = isPassed;
    }

    public double getWeight() {
        return mWeight;
    }

    public void setWeight(double weight) {
        mWeight = weight;
    }

    public void setQuestionTypeID(long questionTypeID){
        mQuestionTypeID = questionTypeID;
    }

    public String getMultipleChoiceText() {
        return mPassFailedText;
    }

    public void setPassFailedText(String passFailedText) {
        mPassFailedText = passFailedText;
    }

    public String getPassFailedText() {
       return mPassFailedText;
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
}
