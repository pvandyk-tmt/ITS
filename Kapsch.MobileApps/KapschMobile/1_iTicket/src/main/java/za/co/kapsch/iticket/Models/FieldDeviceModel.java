package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;

import java.util.Date;

/**
 * Created by csenekal on 2016-09-12.
 */
public class FieldDeviceModel {

    @SerializedName("Id")
    private long mId;
    @SerializedName("DeviceId")
    private String mDeviceId;
    @SerializedName("TechnovolveDeviceId")
    private String mTechnovolveDeviceId;
    @SerializedName("SerialNumber")
    private String mSerialNumber;
    @SerializedName("DistrictId")
    private long mDistrictId;
    @SerializedName("ITicketAppVersion")
    private String mITicketAppVersion;
    @SerializedName("OffenceSetLkVersion")
    private String mOffenceSetLkVersion;
    @SerializedName("OfficerLkVersion")
    private String mOfficerLkVersion;
    @SerializedName("CourtsLkVersion")
    private String mCourtsLkVersion;
    @SerializedName("VehiclesLkVersion")
    private String mVehiclesLkVersion;
    @SerializedName("DatabaseUpdateVersion")
    private String mDatabaseUpdateVersion;
    @SerializedName("PublicHolidayListVersion")
    private String mPublicHolidayListVersion;
    @SerializedName("Active")
    private String mActive;
    @SerializedName("Date_Added")
    private Date mDate_Added;
    @SerializedName("ServerUtcNow")
    private Date mServerUtcNow;
    @SerializedName("FieldDeviceDlcLic")
    private FieldDeviceDlcLicModel mFieldDeviceDlcLic;
    @SerializedName("DeviceConfigurationVersion")
    private String mDeviceConfigurationVersion;

    public long getId() {
        return mId;
    }

    public void setId(long id) {
        mId = id;
    }

    public String getDeviceId() {
        return mDeviceId;
    }

    public void setDeviceId(String deviceId) {
        mDeviceId = deviceId;
    }

    public String getTechnovolveDeviceId() {return mTechnovolveDeviceId; }

    public void setTechnovolveDeviceId(String technovolveDeviceId){
        mTechnovolveDeviceId = technovolveDeviceId;
    }

    public String getSerialNumber() {
        return mSerialNumber;
    }

    public void setSerialNumber(String serialNumber) {
        mSerialNumber = serialNumber;
    }

    public long getDistrictId() {
        return mDistrictId;
    }

    public void setDistrictId(long districtId) {
        mDistrictId = districtId;
    }

    public String getITicketAppVersion() {
        return mITicketAppVersion;
    }

    public void setITicketAppVersion(String ITicketAppVersion) {
        mITicketAppVersion = ITicketAppVersion;
    }

    public String getOffenceSetLkVersion() {
        return mOffenceSetLkVersion;
    }

    public void setOffenceSetLkVersion(String offenceSetLkVersion) {
        mOffenceSetLkVersion = offenceSetLkVersion;
    }

    public String getOfficerLkVersion() {
        return mOfficerLkVersion;
    }

    public void setOfficerLkVersion(String officerLkVersion) {
        mOfficerLkVersion = officerLkVersion;
    }

    public String getCourtsLkVersion() {
        return mCourtsLkVersion;
    }

    public void setCourtsLkVersion(String courtsLkVersion) {
        mCourtsLkVersion = courtsLkVersion;
    }

    public String getVehiclesLkVersion(){
        return mVehiclesLkVersion;
    }

    public void setVehiclesLkVersion(String vehiclesLkVersion) {
        mVehiclesLkVersion = vehiclesLkVersion;
    }

    public String getDatabaseUpdateVersion() {
        return mDatabaseUpdateVersion;
    }

    public void setDatabaseUpdateVersion(String databaseUpdateVersion) {
        mDatabaseUpdateVersion = databaseUpdateVersion;
    }

    public String getPublicHolidayListVersion() {
        return mPublicHolidayListVersion;
    }

    public void setPublicHolidayListVersion(String publicHolidayListVersion) {
        mPublicHolidayListVersion = publicHolidayListVersion;
    }

    public String getActive() {
        return mActive;
    }

    public void setActive(String active) {
        mActive = active;
    }

    public Date getDate_Added() {
        return mDate_Added;
    }

    public void setDate_Added(Date date_Added) {
        mDate_Added = date_Added;
    }

    public Date getServerUtcNow() {
        return mServerUtcNow;
    }

    public void setServerUtcNow(Date serverUtcNow) {
        mServerUtcNow = serverUtcNow;
    }

    public FieldDeviceDlcLicModel getDlcSerializerRsaLic() {
        return mFieldDeviceDlcLic;
    }

    public void setDlcSerializerRsaLic(FieldDeviceDlcLicModel fFieldDeviceDlcLic) {
        mFieldDeviceDlcLic = fFieldDeviceDlcLic;
    }

    public String getDeviceConfigurationVersion() {
        return mDeviceConfigurationVersion;
    }

    public void setDeviceConfigurationVersion(String deviceConfigurationVersion) {
        mDeviceConfigurationVersion = deviceConfigurationVersion;
    }
}
