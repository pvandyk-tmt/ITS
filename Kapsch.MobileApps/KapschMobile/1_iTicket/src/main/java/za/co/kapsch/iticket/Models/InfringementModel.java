package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import java.util.Date;

/**
 * Created by csenekal on 2016-08-11.
 */
public class InfringementModel implements Parcelable {

    private Date mPayDate;
    private Date mIssueDate;
    private Date mOffenceDate;
    private String mLocationDescription;
    private String mLocationSuburb;
    private String mLocationTown;
    private String mLocationCode;
    private boolean mIssueDateLocked;
    private String mTicketNumber;
    private String mExternalToken;
    private String mExternalTokenReference;
    private double mLatitude;
    private double mLongitude;
    private InfringementChargeModel[] mInfringementCharges = new InfringementChargeModel[3];
    private String mNotes;
    private boolean mCancelled;
    private String mCancelledReason;
    private String mCameraID;
    private String mEventID;
    private double mSpeed;

    public Date getPayDate() {
        return mPayDate;
    }

    public void setPayDate(Date payDate) {
        this.mPayDate = payDate;
    }

    public Date getIssueDate() {
        return mIssueDate;
    }

    public void setIssueDate(Date issueDate) {
        if (mIssueDateLocked == false) {
            mIssueDate = issueDate;
        }
    }

    public Date getOffenceDate() {
        return mOffenceDate;
    }

    public void setOffenceDate(Date offenceDate) {
        this.mOffenceDate = offenceDate;
    }

    public String getLocationDescription() {
        return mLocationDescription;
    }

    public void setLocationDescription(String locationDescription) {
        mLocationDescription = locationDescription;
    }

    public String getLocationSuburb() {
        return mLocationSuburb;
    }

    public void setLocationSuburb(String locationSuburb) {
        mLocationSuburb= locationSuburb;
    }

    public String getLocationTown() {
        return mLocationTown;
    }

    public void setLocationTown(String locationTown) {
        mLocationTown = locationTown;
    }

    public String getLocationCode() {
        return mLocationCode;
    }

    public void setLocationCode(String locationCode) {
        mLocationCode = locationCode;
    }

    public boolean isIssueDateLocked() {
        return mIssueDateLocked;
    }

    public void setIssueDateLocked(boolean issueDateLocked) {
        mIssueDateLocked = issueDateLocked;
    }

    public String getTicketNumber() {
        return mTicketNumber;
    }

    public void setTicketNumber(String ticketNumber) {
        this.mTicketNumber = ticketNumber;
    }

    public void setExternalToken(String externalToken){
        mExternalToken = externalToken;
    }

    public String getExternalToken() {
        return mExternalToken;
    }

    public void setExternalTokenReference(String externalTokenReference){
        mExternalTokenReference = externalTokenReference;
    }

    public String getExternalTokenReference() {
        return mExternalTokenReference;
    }

    public double getLatitude() {
        return mLatitude;
    }

    public void setLatitude(double latitude) {
        mLatitude = latitude;
    }

    public double getLongitude() {
        return mLongitude;
    }

    public void setLongitude(double mLongitude) {
        this.mLongitude = mLongitude;
    }

    public InfringementChargeModel[] getInfringementCharges() {
        return mInfringementCharges;
    }

    public void setInfringementCharges(InfringementChargeModel[] infringementCharges) {
        this.mInfringementCharges = infringementCharges;
    }

    public void setNotes(String notes){
        mNotes = notes;
    }

    public String getNotes(){
        return mNotes;
    }

    public void setCancelled (boolean cancelled){
        mCancelled = cancelled;
    }

    public boolean getCancelled(){
        return mCancelled;
    }

    public void setCancelledReason (String cancelledReason){
        mCancelledReason = cancelledReason;
    }

    public String getCancelledReason() {
        return mCancelledReason;
    }

    public void setCameraID (String cameraID){
        mCameraID = cameraID;
    }

    public String getCameraID() {
        return mCameraID;
    }

    public void setEventID (String eventID){
        mEventID = eventID;
    }

    public String getEventID() {
        return mEventID;
    }

    public String getGpsCoordinates(){

        if ((mLatitude == 0) && (mLongitude == 0)){
            return null;
        }

        return String.format("%f, %f",mLatitude, mLongitude);
    }

    public double getSpeed() {
        return mSpeed;
    }

    public void setSpeed(double speed) {
        mSpeed = speed;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeLong(mPayDate == null ? -1 : mPayDate.getTime());
        out.writeLong(mIssueDate == null ? -1 : mIssueDate.getTime());
        out.writeLong(mOffenceDate == null ? -1 : mOffenceDate.getTime());
        out.writeString(mLocationDescription);
        out.writeString(mLocationSuburb);
        out.writeString(mLocationTown);
        out.writeString(mLocationCode);
        out.writeByte((byte)(mIssueDateLocked ? 1 : 0));
        out.writeString(mTicketNumber);
        out.writeString(mExternalToken);
        out.writeString(mExternalTokenReference);
        out.writeDouble(mLatitude);
        out.writeDouble(mLongitude);
        out.writeString(mNotes);
        out.writeByte((byte)(mCancelled ? 1 : 0));
        out.writeString(mCancelledReason);
        out.writeString(mCameraID);
        out.writeString(mEventID);
        out.writeDouble(mSpeed);

        if (mInfringementCharges == null) {
            out.writeInt(0);
        }else{
            out.writeInt(mInfringementCharges.length);
            for (int i = 0; i < mInfringementCharges.length; i++) {
                out.writeParcelable(mInfringementCharges[i], flags);
            }
        }
    }

    public static final Parcelable.Creator<InfringementModel> CREATOR = new Parcelable.Creator<InfringementModel>() {
        public InfringementModel createFromParcel(Parcel in) {
            return new InfringementModel(in);
        }

        public InfringementModel[] newArray(int size) {
            return new InfringementModel[size];
        }
    };

    public InfringementModel(){}

    private InfringementModel(Parcel in){

        long tmpPayDate = in.readLong();
        mPayDate = tmpPayDate == -1 ? null : new Date(tmpPayDate);

        long tmpIssueDate = in.readLong();
        mIssueDate = tmpIssueDate == -1 ? null : new Date(tmpIssueDate);

        long tmpOffenceDate = in.readLong();
        mOffenceDate = tmpOffenceDate == -1 ? null : new Date(tmpOffenceDate);

        mLocationDescription = in.readString();
        mLocationSuburb = in.readString();
        mLocationTown = in.readString();
        mLocationCode = in.readString();
        mIssueDateLocked = in.readByte() != 0;
        mTicketNumber = in.readString();
        mExternalToken = in.readString();
        mExternalTokenReference = in.readString();
        mLatitude = in.readDouble();
        mLongitude = in.readDouble();
        mNotes = in.readString();
        mCancelled = in.readByte() != 0;
        mCancelledReason = in.readString();
        mCameraID = in.readString();
        mEventID = in.readString();
        mSpeed = in.readDouble();

        mInfringementCharges = new InfringementChargeModel[in.readInt()];

        for(int i = 0; i < mInfringementCharges.length; i++ ){
           InfringementChargeModel infringementCharge = in.readParcelable(InfringementChargeModel.class.getClassLoader());
           mInfringementCharges[i] = infringementCharge;
        }
    }
}
