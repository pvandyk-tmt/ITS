package za.co.kapsch.ipayment.Models;

import com.google.gson.annotations.SerializedName;

/**
 * Created by csenekal on 2017/11/15.
 */

public class DumaPayReceipt {

    @SerializedName("id")
    public long Id;

    @SerializedName("Result")
    public String Result;

    @SerializedName("ResultExplanation")
    public String ResultExplanation;

    @SerializedName("TransToken")
    public String TransToken;

    @SerializedName("CompanyToken")
    public String CompanyToken;

    @SerializedName("ReceiptName")
    public String ReceiptName;

    @SerializedName("DateCreated")
    public String DateCreated;

    @SerializedName("CardBin")
    public String CardBin;

    @SerializedName("CardLast4")
    public String CardLast4;

    @SerializedName("TransactionSource")
    public String TransactionSource;

    @SerializedName("PaymentRRN")
    public String PaymentRRN;

    @SerializedName("IcmpResponse")
    public String IcmpResponse;

    @SerializedName("CustomerName")
    public String CustomerName;

    @SerializedName("CustomerCredit")
    public String CustomerCredit;

    @SerializedName("TransactionApproval")
    public String TransactionApproval;

    @SerializedName("TransactionCurrency")
    public String TransactionCurrency;

    @SerializedName("TransactionAmount")
    public String TransactionAmount;

    @SerializedName("FraudAlert")
    public String FraudAlert;

    @SerializedName("FraudExplnation")
    public String FraudExplnation;

    @SerializedName("TransactionNetAmount")
    public String TransactionNetAmount;

    @SerializedName("TransactionSettlementDate")
    public String TransactionSettlementDate;

    @SerializedName("TransactionRollingReserveAmount")
    public String TransactionRollingReserveAmount;

    @SerializedName("TransactionRollingReserveDate")
    public String TransactionRollingReserveDate;

    @SerializedName("CustomerPhone")
    public String CustomerPhone;

    @SerializedName("CustomerCountry")
    public String CustomerCountry;

    @SerializedName("CustomerAddress")
    public String CustomerAddress;

    @SerializedName("CustomerCity")
    public String CustomerCity;

    @SerializedName("CustomerZip")
    public String CustomerZip;

    @SerializedName("MobilePaymentRequest")
    public String MobilePaymentRequest;

    @SerializedName("AccRef")
    public String AccRef;
}
