package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;

/**
 * Created by csenekal on 2016-10-04.
 */

public class PersonModel {
    @SerializedName("Id")
    private int mId;
    @SerializedName("Name")
    private String mName;
    @SerializedName("Surname")
    private String mSurname;
    @SerializedName("MiddleNames")
    private String mMiddleNames;
    @SerializedName("IdNumber")
    private String mIdNumber;
    @SerializedName("Telephone")
    private String mTelephone;
    @SerializedName("PhysicalAddressId")
    private int mPhysicalAddressId;
    @SerializedName("Postal")
    private DataServiceAddressModel mPostal;
    @SerializedName("Physical")
    private DataServiceAddressModel mPhysical;

    public int getId() {
        return mId;
    }

    public void setId(int id) {
        mId = id;
    }

    public String getName() {
        return mName;
    }

    public void setName(String name) {
        mName = name;
    }

    public String getSurname() {
        return mSurname;
    }

    public void setSurname(String surname) {
        mSurname = surname;
    }

    public String getMiddleNames() {
        return mMiddleNames;
    }

    public void setMiddleNames(String middleNames) {
        mMiddleNames = middleNames;
    }

    public String getIdNumber() {
        return mIdNumber;
    }

    public void setIdNumber(String idNumber) {
        mIdNumber = idNumber;
    }

    public String getTelephone() {
        return mTelephone;
    }

    public void setTelephone(String telephone) {
        mTelephone = telephone;
    }

    public int getPhysicalAddressId() {
        return mPhysicalAddressId;
    }

    public void setPhysicalAddressId(int physicalAddressId) {
        mPhysicalAddressId = physicalAddressId;
    }

    public DataServiceAddressModel getPostal() {
        return mPostal;
    }

    public void setPostal(DataServiceAddressModel postal) {
        mPostal = postal;
    }

    public DataServiceAddressModel getPhysical() {
        return mPhysical;
    }

    public void setPhysical(DataServiceAddressModel physical) {
        mPhysical = physical;
    }
}
