package za.co.kapsch.console.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

/**
 * Created by csenekal on 2016-09-08.
 */
public class CredentialModel implements Parcelable{

    @SerializedName("UserName")
    private String mUserName;
    @SerializedName("Password")
    private String mPassword;

    public CredentialModel(String userName, String password){
        mUserName = userName;
        mPassword = password;
    }

    public String getUserName() {
        return mUserName;
    }

    public void setUserName(String userName) {
        mUserName = userName;
    }

    public String getPassword() {
        return mPassword;
    }

    public void setPassword(String password) {
        mPassword = password;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(mUserName);
        out.writeString(mPassword);
    }

    public static final Parcelable.Creator<CredentialModel> CREATOR = new Parcelable.Creator<CredentialModel>() {
        public CredentialModel createFromParcel(Parcel in) {
            return new CredentialModel(in);
        }

        public CredentialModel[] newArray(int size) {
            return new CredentialModel[size];
        }
    };

    private CredentialModel(Parcel in){
        mUserName = in.readString();
        mPassword = in.readString();
    }
}
