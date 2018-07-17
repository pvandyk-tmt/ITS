package za.co.kapsch.shared.Models;

/**
 * Created by CSenekal on 2017/07/25.
 */
import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.dao.ForeignCollection;
import com.j256.ormlite.field.DataType;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.field.ForeignCollectionField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.shared.Enums.UserStatus;

@DatabaseTable(tableName = "User")
public class UserModel implements Parcelable {

    @DatabaseField(columnName = "ID", id = true)
    @SerializedName("ID")
    private long mID;

    @DatabaseField(columnName = "CredentialID")
    @SerializedName("CredentialID")
    public long mCredentialID;

    @SerializedName("UserName")
    @DatabaseField(columnName = "UserName")
    private String mUserName;

    @SerializedName("Password")
    @DatabaseField(columnName = "Password")
    private String mPassword;

    @SerializedName("FirstName")
    @DatabaseField(columnName = "FirstName")
    private String mFirstName;

    @SerializedName("LastName")
    @DatabaseField(columnName = "LastName")
    private String mLastName;

    @SerializedName("MobileNumber")
    @DatabaseField(columnName = "MobileNumber")
    private String mMobileNumber;

    @SerializedName("Email")
    @DatabaseField(columnName = "Email")
    private String mEmail;

    @SerializedName("ExternalID")
    @DatabaseField(columnName = "ExternalID")
    private String mExternalID;

    @SerializedName("Status")
    @DatabaseField(columnName = "Status")
    private UserStatus mStatus;

//    @SerializedName("SystemFunctions")
//    @ForeignCollectionField
//    private ForeignCollection<SystemFunctionModel> mSystemFunctions;

    @SerializedName("SystemFunctions")
    private List<SystemFunctionModel> mSystemFunctions;

    @SerializedName("Districts")
    public List<DistrictModel> mDistricts;

    @DatabaseField(columnName = "SystemFunctionIDs")
    private String mSystemFunctionIDs;

    @DatabaseField(columnName = "DistrictIDs")
    private String mDistrictIDs;

    @SerializedName("IsOfficer")
    @DatabaseField(columnName = "IsOfficer")
    private boolean IsOfficer;

    @DatabaseField(columnName = "Signature", dataType = DataType.BYTE_ARRAY)
    private byte[] mSignature;

    private boolean mSignatureConfirmed;

    public long getId() {
        return mID;
    }

    public void setId(int id) {
        this.mID = id;
    }

    public long getCredentialID() {
        return mCredentialID;
    }

    public void setCredentialID(int CredentialID) {
        mCredentialID = CredentialID;
    }

    public String getUserName() { return mUserName; }

    public void setUserName(String userName) {
        this.mUserName = userName;
    }

    public String getPassword() {
        return mPassword;
    }

    public void setPassword(String password) {
        this.mPassword = password;
    }

    public String getFirstName() {
        return mFirstName;
    }

    public void setFirstName(String firstName) {
        this.mFirstName = firstName;
    }

    public String getLastName() {
        return mLastName;
    }

    public void setLastName(String lastName) {
        mLastName = lastName;
    }

    public String getInfrastructureNumber() {
        return mExternalID;
    }

    public void setInfrastructureNumber(String externalID) {
        mExternalID = externalID;
    }

    public byte[] getSignature() {
        return mSignature;
    }

    public void setSignature(byte[] signature) {
        this.mSignature = signature;
    }

    public boolean isSignatureConfirmed() {
        return mSignatureConfirmed;
    }

    public void setSignatureConfirmed(boolean signatureConfirmed) {
        this.mSignatureConfirmed = signatureConfirmed;
    }

    public List<SystemFunctionModel> getSystemFunctions() {
        return mSystemFunctions;
    }

    public void setSystemFunctions(List<SystemFunctionModel> systemFunctions) {
        mSystemFunctions = systemFunctions;
    }

    public List<DistrictModel> getDistricts() {
        return mDistricts;
    }

    public void setDistricts(List<DistrictModel> districts) {
        mDistricts = districts;
    }

    public String getSystemFunctionIDs(){
        return mSystemFunctionIDs;
    }

    public String setSystemFunctionIDs(){

        mSystemFunctionIDs = null;

        for(SystemFunctionModel systemFunction : mSystemFunctions ){

            if (mSystemFunctionIDs == null) {
                mSystemFunctionIDs = Long.toString(systemFunction.getID());
            }
            else {
                mSystemFunctionIDs = String.format("%s,%d", mSystemFunctionIDs, systemFunction.getID());
            }
        }

        return mSystemFunctionIDs;
    }

    public String getDistrictIDs(){
        return mDistrictIDs;
    }

    public String setDistrictIDs(){

        mDistrictIDs = null;

        for(DistrictModel district : mDistricts ){

            if (mDistrictIDs == null) {
                mDistrictIDs = Long.toString(district.getID());
            }
            else {
                mDistrictIDs = String.format("%s,%d", mDistrictIDs, district.getID());
            }
        }

        return mDistrictIDs;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeLong(mID);
        out.writeLong(mCredentialID);
        out.writeString(mUserName);
        out.writeString(mPassword);
        out.writeString(mFirstName);
        out.writeString(mLastName);
        out.writeString(mMobileNumber);
        out.writeString(mEmail);
        out.writeString(mExternalID);
        out.writeString(mSystemFunctionIDs);
        out.writeString(mDistrictIDs);
        out.writeValue(mStatus);

        if (mSystemFunctions == null) {
            out.writeInt(0);
        } else {
            out.writeInt(mSystemFunctions.size());
            for (SystemFunctionModel systemFunction : mSystemFunctions) {
                out.writeParcelable(systemFunction, flags);
            }
        }

        if (mDistricts == null) {
            out.writeInt(0);
        } else {
            out.writeInt(mDistricts.size());
            for (DistrictModel district : mDistricts) {
                out.writeParcelable(district, flags);
            }
        }

        out.writeByte((byte)(mSignatureConfirmed ? 1 : 0));

        if (mSignature != null) {
            out.writeInt(mSignature.length);
            if (mSignature != null) out.writeByteArray(mSignature);
        }
        else{
            out.writeInt(0);
        }
    }

    public static final Parcelable.Creator<UserModel> CREATOR = new Parcelable.Creator<UserModel>() {
        public UserModel createFromParcel(Parcel in) {
            return new UserModel(in);
        }

        public UserModel[] newArray(int size) {
            return new UserModel[size];
        }
    };

    public UserModel(){}

    private UserModel(Parcel in){
        mID = in.readLong();
        mCredentialID = in.readLong();
        mUserName = in.readString();
        mPassword = in.readString();
        mFirstName = in.readString();
        mLastName = in.readString();
        mMobileNumber = in.readString();
        mEmail = in.readString();
        mExternalID = in.readString();
        mSystemFunctionIDs = in.readString();
        mDistrictIDs = in.readString();
        mStatus = (UserStatus) in.readValue(UserStatus.class.getClassLoader());

        int systemFunctionCount = in.readInt();
        if (systemFunctionCount > 0) {
            mSystemFunctions = new ArrayList<>();
            for (int i = 0; i < systemFunctionCount; i++) {
                SystemFunctionModel systemFunction =  in.readParcelable(SystemFunctionModel.class.getClassLoader());
                mSystemFunctions.add(systemFunction);
            }
        }

        int districtCount = in.readInt();
        if (districtCount > 0) {
            mDistricts = new ArrayList<>();
            for (int i = 0; i < districtCount; i++) {
                DistrictModel district =  in.readParcelable(DistrictModel.class.getClassLoader());
                mDistricts.add(district);
            }
        }

        mSignatureConfirmed = in.readByte() != 0;

        int signatureLength = in.readInt();
        if (signatureLength > 0) {
            mSignature = new byte[signatureLength];
            in.readByteArray(mSignature);
        }
    }
}