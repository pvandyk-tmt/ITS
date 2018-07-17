package za.co.kapsch.shared.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

import java.sql.SQLException;
import java.util.Calendar;
import java.util.Date;
import java.util.TimeZone;

import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Enums.SessionType;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2016-09-08.
 */
public class SessionModel implements Parcelable {

    private static SessionModel mInstance;

    //public static final String DLC_SERIALIZER_RSA_LIC = "dlcSerializerRsaLic";

    @SerializedName("SessionToken")
    private String mSessionToken;
    @SerializedName("EntityType")
    private long mEntityType;
    @SerializedName("EntityID")
    private long mUserId;
    @SerializedName("UserName")
    private String mUserName;
    @SerializedName("SessionType")
    private SessionType mSessionType;
    @SerializedName("CreatedTimestamp")
    private Date mCreatedTimestamp;
    @SerializedName("ExpiryTimestamp")
    private Date mExpiryTimestamp;
    @SerializedName("Password")
    private transient String mPassword;
    private transient boolean mRequestNewSession;
    private transient double mLatitude;
    private transient double mLongitude;
    private transient String mInfrastructureNumber;
    private transient String mCurrentGpsAddress;
    //private transient String mDlcSerializerRsaLic;
    //private transient long mDistrictID;
    private transient String mOffenceLocation;
    private transient long mInternalDeviceID;
    private transient UserModel mUser;
    private transient DistrictModel mDistrict;
    private transient MobileDeviceModel mMobileDevice;
    private transient String mPrinterMacAddress;

    public synchronized static SessionModel getInstance()
    {
        if (mInstance == null)
        {
            mInstance = new SessionModel();
        }
        return mInstance;
    }

    public void setSession(SessionModel session){
        mSessionToken = session.getSessionToken();
        mEntityType = session.mEntityType;
        mUserId = session.mUserId;
        mUserName = session.getUserName();
        mSessionType = session.getSessionType();
        mCreatedTimestamp = session.getCreatedTimestamp();
        mExpiryTimestamp = session.getExpiryTimestamp();
    }

    public void clearSession(){
        mSessionToken = null;
        mEntityType = -1;
        mUserId = -1;
        mUserName = null;
        mSessionType = null;
        mCreatedTimestamp = null;
        mExpiryTimestamp = null;
        //mLocalUserId = -1;
    }

    public String getSessionToken() {
        return mSessionToken;
    }

    public long getUserId() {
        return mUserId;
    }

    public void setUserId(long userId) {
        mUserId = userId;
    }

    public String getUserName() {
        return mUserName;
    }

    public void setUserName(String userName) {
        mUserName = userName;
    }

    public SessionType getSessionType() {
        return mSessionType;
    }

    public Date getCreatedTimestamp() {
        return mCreatedTimestamp;
    }

    public Date getExpiryTimestamp() {
        return mExpiryTimestamp;
    }

    public String getPassword() {
        return mPassword;
    }

    public void setPassword(String password) {
        mPassword = password;
    }

    public boolean isRequestNewSession() {
        return mRequestNewSession;
    }

    public void setRequestNewSession(boolean requestNewSession) {
        mRequestNewSession = requestNewSession;
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

    public void setLongitude(double longitude) {
        mLongitude = longitude;
    }

    public String getInfrastructureNumber() {
        return mInfrastructureNumber;
    }

    public void setInfrastructureNumber(String infrastructureNumber) {
        mInfrastructureNumber = infrastructureNumber;
    }

    public String getCurrentGpsAddress() {
        return mCurrentGpsAddress;
    }

    public void setCurrentGpsAddress(String currentAddress) {
        mCurrentGpsAddress = currentAddress;
    }

    public String getOffenceLocation() {
        return mOffenceLocation;
    }

    public void setOffenceLocation(String offenceLocation) {
        mOffenceLocation = offenceLocation;
    }

    public long getInternalDeviceID() {
        return mInternalDeviceID;
    }

    public void setInternalDeviceID(long internalDeviceID) {
        mInternalDeviceID = internalDeviceID;
    }

    public UserModel getUser() {
        return mUser;
    }

    public void setUser(UserModel user) {
        mUser = user;
    }

    public DistrictModel getDistrict() {
        return mDistrict;
    }

    public MobileDeviceModel getMobileDevice() {
        return mMobileDevice;
    }

    public void setMobileDevice(MobileDeviceModel mobileDevice) {
        mMobileDevice = mobileDevice;
    }

    public void setDistrict(DistrictModel district) {
        mDistrict = district;
    }

    public String getPrinterMacAddress() {
        return mPrinterMacAddress;
    }

    public void setPrinterMacAddress(String printerMacAddress) {
        mPrinterMacAddress = printerMacAddress;
    }

    public boolean sessionTokenExpired(){

        if (mExpiryTimestamp == null) return true;

        Calendar calendar = Calendar.getInstance(TimeZone.getTimeZone("GMT"));
        Date utcNow = calendar.getTime();

        Date adjustedExpiryTimestamp =  Utilities.getCalendar(
                mExpiryTimestamp.getYear(),
                mExpiryTimestamp.getMonth(),
                mExpiryTimestamp.getDate(),
                mExpiryTimestamp.getHours(),
                mExpiryTimestamp.getMinutes()-15,
                mExpiryTimestamp.getSeconds()).getTime();

        return utcNow.after(adjustedExpiryTimestamp);
    }

    //TODO add some kind of security here
    public boolean isAgencyUser(){
        return (mUserName.toString().equals("TMT_API_USER") && (mPassword.toString().equals("TMT_API_USER")));
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(mSessionToken);
        out.writeLong(mUserId);
        out.writeString(mUserName);
        out.writeValue(mSessionType);
        out.writeLong(mCreatedTimestamp.getTime());
        out.writeLong(mExpiryTimestamp.getTime());
        out.writeDouble(mLatitude);
        out.writeDouble(mLongitude);
        out.writeString(mInfrastructureNumber);
        out.writeString(mCurrentGpsAddress);
        out.writeString(mOffenceLocation);
        out.writeLong(mInternalDeviceID);
        out.writeParcelable(mUser, flags);
        out.writeParcelable(mDistrict, flags);
        out.writeParcelable(mMobileDevice, flags);
        out.writeString(mPrinterMacAddress);

    }

    public static final Creator<SessionModel> CREATOR = new Creator<SessionModel>() {
        public SessionModel createFromParcel(Parcel in) {
            return new SessionModel(in);
        }

        public SessionModel[] newArray(int size) {
            return new SessionModel[size];
        }
    };

    public SessionModel(){
        mRequestNewSession = false;
    }

    private SessionModel(Parcel in) {
        mSessionToken = in.readString();
        mUserId = in.readLong();
        mUserName = in.readString();
        mSessionType = (SessionType) in.readValue(SessionType.class.getClassLoader());
        mCreatedTimestamp = new Date(in.readLong());
        mExpiryTimestamp = new Date(in.readLong());
        mLatitude = in.readDouble();
        mLongitude = in.readDouble();
        mInfrastructureNumber = in.readString();
        mCurrentGpsAddress = in.readString();
        mOffenceLocation = in.readString();
        mInternalDeviceID = in.readLong();
        mUser = in.readParcelable(UserModel.class.getClassLoader());
        mDistrict = in.readParcelable(DistrictModel.class.getClassLoader());
        mMobileDevice = in.readParcelable(MobileDeviceModel.class.getClassLoader());
        mPrinterMacAddress = in.readString();
    }
}
