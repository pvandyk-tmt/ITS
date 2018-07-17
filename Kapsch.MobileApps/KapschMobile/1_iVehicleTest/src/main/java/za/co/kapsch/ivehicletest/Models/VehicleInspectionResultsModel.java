package za.co.kapsch.ivehicletest.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;
import java.util.List;

/**
 * Created by CSenekal on 2017/12/11.
 */

@DatabaseTable(tableName = "VehicleInspectionResults")
public class VehicleInspectionResultsModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", generatedId = true)
    private long mID;

    @DatabaseField(columnName = "Uploaded")
    private transient boolean mUploaded;

    @DatabaseField(columnName = "BookingID")
    private transient long mBookingID;

    @SerializedName("TestStartTime")
    @DatabaseField(columnName = "TestStartTime")
    private Date mTestStartTime;

    @SerializedName("TestEndTime")
    @DatabaseField(columnName = "TestEndTime")
    private Date mTestEndTime;

    @SerializedName("inspectorID")
    @DatabaseField(columnName = "CredentialID")
    private long mCredentialID;

    @SerializedName("IsPassed")
    @DatabaseField(columnName = "IsPassed")
    private boolean mIsPassed;

    @SerializedName("questionAnswerResults")
    private List<VehicleInspectionResultModel> mVehicleInspectionResultList;

    public long getID() { return mID; }

    public long getBookingID() {
        return mBookingID;
    }

    public void setBookingID(long bookingID) {
        mBookingID = bookingID;
    }

    public Date getTestStartTime() {
        return mTestStartTime;
    }

    public void setTestStartTime(Date testStartTime) {
        mTestStartTime = testStartTime;
    }

    public Date getTestEndTime() {
        return mTestEndTime;
    }

    public void setTestEndTime(Date testEndTime) {
        mTestEndTime = testEndTime;
    }

    public void setCredentialID(long credentialID) {
        mCredentialID = credentialID;
    }

    public boolean isUploaded() {
        return mUploaded;
    }

    public void setUploaded(boolean uploaded) {
        mUploaded = uploaded;
    }

    public void setIsPassed(boolean isPassed){
        mIsPassed = isPassed;
    }

    public boolean getIsPassed(){
        return mIsPassed;
    }

    public void setVehicleInspectionResultList(List<VehicleInspectionResultModel> vehicleInspectionResultList) {
        mVehicleInspectionResultList = vehicleInspectionResultList;
    }

    public List<VehicleInspectionResultModel> getVehicleInspectionResultList(){
        return mVehicleInspectionResultList;
    }
}
