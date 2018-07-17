package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import com.google.gson.annotations.SerializedName;

import java.util.Date;

/**
 * Created by csenekal on 2016-08-02.
 */
public class AddressInfoModel{

    @SerializedName("ID")
    private long mID;

    @SerializedName("AddressTypeID")
    private long mAddressTypeID;

    @SerializedName("SourceID")
    private long mSourceID;

    @SerializedName("PersonInfoID")
    private long mPersonInfoID;

    @SerializedName("Line1")
    private String mLine1;

    @SerializedName("Line2")
    private String mLine2;

    @SerializedName("Suburb")
    private String mSuburb;

    @SerializedName("Town")
    private String mTown;

    @SerializedName("Country")
    private String mCountry;

    @SerializedName("Code")
    private String mCode;

    @SerializedName("Latitude")
    private Double mLatitude;

    @SerializedName("Longitude")
    private Double mLongitude;

    @SerializedName("CreatedUserDetailID")
    private Long mCreatedUserDetailID;

    @SerializedName("CreatedDate")
    private Date mCreatedDate;

    @SerializedName("IsPrefferedIndicator")
    private Long mIsPrefferedIndicator;
}
