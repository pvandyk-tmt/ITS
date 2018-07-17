package za.co.kapsch.ipayment.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Date;
import java.util.List;

import za.co.kapsch.ipayment.Enums.BroadcastSource;
import za.co.kapsch.ipayment.Enums.PaymentMethod;
import za.co.kapsch.ipayment.Enums.PaymentTransactionStatus;
import za.co.kapsch.ipayment.Enums.TerminalType;

/**
 * Created by CSenekal on 2017/09/08.
 */

@DatabaseTable(tableName = "TransactionRegister")
public class TransactionModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", generatedId = true)
    private long mID;

    @SerializedName("Receipt")
    @DatabaseField(columnName = "Receipt")
    private String mReceipt;

    @SerializedName("ReceiptTimestamp")
    @DatabaseField(columnName = "ReceiptTimeStamp")
    private Date mReceiptTimeStamp;

    @SerializedName("TransactionToken")
    @DatabaseField(columnName = "TransactionToken")
    private String mTransactionToken;

    @SerializedName("TerminalType")
    @DatabaseField(columnName = "TerminalType")
    private TerminalType mTerminalType;

    @SerializedName("TerminalUUID")
    @DatabaseField(columnName = "TerminalUUID")
    private String mTerminalUUID;

    @SerializedName("PaymentSource")
    @DatabaseField(columnName = "PaymentSource")
    private PaymentMethod mPaymentSource;

    @SerializedName("CustomerFirstName")
    private String mCustomerFirstName;

    @SerializedName("CustomerLastName")
    private String mCustomerLastName;

    @SerializedName("CustomerIDNumber")
    private String mCustomerIDNumber;

    @SerializedName("CustomerContactNumber")
    private String mCustomerContactNumber;

    @SerializedName("Amount")
    @DatabaseField(columnName = "Amount")
    private double mAmount;

    @SerializedName("UserID")
    @DatabaseField(columnName = "UserID")
    private long mUserID;

    @SerializedName("CreatedTimestamp")
    private Date mCreatedTimestamp;

    @SerializedName("ModifiedTimestamp")
    private Date mModifiedTimestamp;

    @SerializedName("Status")
    @DatabaseField(columnName = "Status")
    private PaymentTransactionStatus mStatus;

    @SerializedName("PaymentTransactionItems")
    private List<TransactionItemModel> mTransactionItems;

    @DatabaseField(columnName = "UserName")
    private transient String mUserName;

    @SerializedName("ConfirmationSource")
    private transient BroadcastSource mConfirmationSource;

    @DatabaseField(columnName = "ConfirmedTransactionToken")
    private transient String mConfirmedTransactionToken;

    @DatabaseField(columnName = "ConfirmedAmount")
    private transient double mConfirmedAmount;

    public long getID() {
        return mID;
    }

    public void setID(long ID) {
        mID = ID;
    }

    public String getReceipt() {
        return mReceipt;
    }

    public void setReceipt(String receipt) {
        mReceipt = receipt;
    }

    public Date getReceiptTimeStamp() {
        return mReceiptTimeStamp;
    }

    public void setReceiptTimeStamp(Date receiptTimestamp) {
        mReceiptTimeStamp = receiptTimestamp;
    }

    public String getTransactionToken() {
        return mTransactionToken;
    }

    public void setTransactionToken(String transactionToken) {
        mTransactionToken = transactionToken;
    }

    public TerminalType getTerminalType() {
        return mTerminalType;
    }

    public void setTerminalType(TerminalType mTerminalType) {
        this.mTerminalType = mTerminalType;
    }

    public String getTerminalUUID() {
        return mTerminalUUID;
    }

    public void setTerminalUUID(String terminalUUID) {
        mTerminalUUID = terminalUUID;
    }

    public PaymentMethod getPaymentSource() {
        return mPaymentSource;
    }

    public void setPaymentSource(PaymentMethod paymentSource) {
        mPaymentSource = paymentSource;
    }

    public String getCustomerFirstName() {
        return mCustomerFirstName;
    }

    public void setCustomerFirstName(String customerFirstName) {
        mCustomerFirstName = customerFirstName;
    }

    public String getCustomerLastName() {
        return mCustomerLastName;
    }

    public void setCustomerLastName(String customerLastName) {
        mCustomerLastName = customerLastName;
    }

    public String getCustomerIDNumber() {
        return mCustomerIDNumber;
    }

    public void setCustomerIDNumber(String customerIDNumber) {
        mCustomerIDNumber = customerIDNumber;
    }

    public String getCustomerContactNumber() {
        return mCustomerContactNumber;
    }

    public void setCustomerContactNumber(String customerContactNumber) {
        mCustomerContactNumber = customerContactNumber;
    }

    public double getAmount() {
        return mAmount;
    }

    public void setAmount(double amount) {
        mAmount = amount;
    }

    public long getUserID() {
        return mUserID;
    }

    public void setUserID(long userID) {
        mUserID = userID;
    }

    public Date getCreatedTimestamp() {
        return mCreatedTimestamp;
    }

    public void setCreatedTimestamp(Date createdTimestamp) {
        mCreatedTimestamp = createdTimestamp;
    }

    public Date getModifiedTimestamp() {
        return mModifiedTimestamp;
    }

    public void setModifiedTimestamp(Date modifiedTimestamp) {
       mModifiedTimestamp = modifiedTimestamp;
    }

    public PaymentTransactionStatus getStatus() {
        return mStatus;
    }

    public void setStatus(PaymentTransactionStatus status) {
        mStatus = status;
    }

    public List<TransactionItemModel> getTransactionItems() {
        return mTransactionItems;
    }

    public void setTransactionItems(List<TransactionItemModel> transactionItems) {
        mTransactionItems = transactionItems;
    }

    public String getUserName() {
        return mUserName;
    }

    public void setUserName(String userName) {
        mUserName = userName;
    }

    public BroadcastSource getConfirmationSource() {
        return mConfirmationSource;
    }

    public void setConfirmationSource(BroadcastSource confirmationSource) {
        mConfirmationSource = confirmationSource;
    }

    public String getConfirmedTransactionToken() {
        return mConfirmedTransactionToken;
    }

    public void setConfirmedTransactionToken(String confirmedTransactionToken) {
        this.mConfirmedTransactionToken = confirmedTransactionToken;
    }

    public double getConfirmedAmount() {
        return mConfirmedAmount;
    }

    public void setConfirmedAmount(double confirmedAmount) {
        this.mConfirmedAmount = confirmedAmount;
    }
}
