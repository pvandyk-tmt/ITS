package za.co.kapsch.ipayment.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import za.co.kapsch.ipayment.Enums.BroadcastSource;
import za.co.kapsch.ipayment.Enums.PaymentTransactionStatus;

/**
 * Created by CSenekal on 2017/09/08.
 */
@DatabaseTable(tableName = "TransactionItem")
public class TransactionItemModel {

    @DatabaseField(columnName = "ID", generatedId = true)
    private transient long mID;

    @DatabaseField(columnName = "TransactionID")
    private transient long mTransactionID;

    @SerializedName("ReferenceNumber")
    @DatabaseField(columnName = "ReferenceNumber")
    private String mReferenceNumber;

    @SerializedName("EntityReferenceTypeID")
    private long mEntityReferenceTypeID;

    @SerializedName("TransactionToken")
    @DatabaseField(columnName = "TransactionToken")
    private String mTransactionToken;

    @SerializedName("Amount")
    @DatabaseField(columnName = "Amount")
    private double mAmount;

    @SerializedName("Description")
    @DatabaseField(columnName = "Description")
    private String mDescription;

    @SerializedName("Status")
    @DatabaseField(columnName = "Status")
    private PaymentTransactionStatus mStatus;

    public long getID() {
        return mID;
    }

    public void setID(long ID) {
        mID = ID;
    }

    public String getReferenceNumber() {
        return mReferenceNumber;
    }

    public long getTransactionID() {
        return mTransactionID;
    }

    public void setTransactionID(long transactionID) {
        mTransactionID = transactionID;
    }

    public void setReferenceNumber(String referenceNumber) {
        mReferenceNumber = referenceNumber;
    }

    public long getEntityReferenceTypeID() {
        return mEntityReferenceTypeID;
    }

    public void setEntityReferenceTypeID(long entityReferenceTypeID) {
        mEntityReferenceTypeID = entityReferenceTypeID;
    }

    public String getTransactionToken() {
        return mTransactionToken;
    }

    public void setTransactionToken(String transactionToken) {
        mTransactionToken = transactionToken;
    }

    public double getAmount() {
        return mAmount;
    }

    public void setAmount(double amount) {
        mAmount = amount;
    }

    public String getDescription() {
        return mDescription;
    }

    public void setDescription(String description) {
        mDescription = description;
    }

    public PaymentTransactionStatus getStatus() {
        return mStatus;
    }

    public void setStatus(PaymentTransactionStatus status) {
        mStatus = status;
    }


}
