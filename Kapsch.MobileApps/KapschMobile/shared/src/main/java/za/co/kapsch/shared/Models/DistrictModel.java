package za.co.kapsch.shared.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.annotations.SerializedName;
import com.google.gson.reflect.TypeToken;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.lang.reflect.Type;
import java.util.List;

import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2016-07-28.
 */
@DatabaseTable(tableName = "District")
public class DistrictModel implements Parcelable {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", id = true)
    private long mID;

    @SerializedName("Town")
    @DatabaseField(columnName = "Name")
    private String mName;

    @SerializedName("BranchName")
    @DatabaseField(columnName = "BranchName")
    private String mBranchName;

    @SerializedName("PaymentOptions")
    @DatabaseField(columnName = "PaymentOptions")
    private String mPaymentOptions;

    public String getName() {
        return mName;
    }

    public void setName(String name) {
        mName = name;
    }

    public String getBranchName() {
        return mBranchName;
    }

    public void setBranchName(String branchName) {
        mBranchName = branchName;
    }

    public long getID() { return mID; }

    public void setID(int ID){
        mID = ID;
    }

    @Override
    public String toString() {
        return mBranchName;
    }

    public void setPaymentOptions(String paymentOptions){
        mPaymentOptions = paymentOptions;
    }

    public List<PaymentOption> getPaymentOptions(){

        try {
            Gson gson = new Gson();
            return gson.fromJson(mPaymentOptions, new TypeToken<List<PaymentOption>>() {}.getType());
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "DistrictModel::getPaymentOptions()"), ErrorSeverity.High);
            return null;
        }
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeLong(mID);
        out.writeString(mName);
        out.writeString(mBranchName);
        out.writeString(mPaymentOptions);
    }

    public static final Creator<DistrictModel> CREATOR = new Creator<DistrictModel>() {
        public DistrictModel createFromParcel(Parcel in) {
            return new DistrictModel(in);
        }

        public DistrictModel[] newArray(int size) {
            return new DistrictModel[size];
        }
    };

    public DistrictModel(){}

    private DistrictModel(Parcel in){
        mID = in.readLong();
        mName = in.readString();
        mBranchName  = in.readString();
        mPaymentOptions = in.readString();
    }
}
