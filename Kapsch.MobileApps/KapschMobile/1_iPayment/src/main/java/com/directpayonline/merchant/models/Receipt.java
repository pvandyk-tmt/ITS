package com.directpayonline.merchant.models;

/**
 * Created by obake on 8/1/2017.
 */

import android.os.Parcel;
import android.os.Parcelable;
import android.support.annotation.NonNull;

import java.util.Date;

public class Receipt implements Comparable<Receipt>, Parcelable {

    public long id;

    public String TransToken;
    public String CompanyToken;
    public String ReceiptName;
    public String DateCreated;
    public String CardBin;
    public String CardLast4;
    public String TransactionSource;
    public String PaymentRRN;
    public String IcmpResponse;
    public String Transaction_Sequence;
    public String createTokenRequestXML;
    public String createTokenResponseXML;
    public String Result;
    public String ResultExplanation;
    public String CustomerName;
    public String CustomerCredit;
    public String TransactionApproval;
    public String TransactionCurrency;
    public String TransactionAmount;
    public String FraudAlert;
    public String FraudExplnation;
    public String TransactionNetAmount;
    public String TransactionSettlementDate;
    public String TransactionRollingReserveAmount;
    public String TransactionRollingReserveDate;
    public String CustomerPhone;
    public String CustomerCountry;
    public String CustomerAddress;
    public String CustomerCity;
    public String CustomerZip;
    public String MobilePaymentRequest;
    public String AccRef;

    protected Receipt(Parcel in) {
        id = in.readLong();
        TransToken = in.readString();
        CompanyToken = in.readString();
        ReceiptName = in.readString();
        DateCreated = in.readString();
        CardBin = in.readString();
        CardLast4 = in.readString();
        TransactionSource = in.readString();
        PaymentRRN = in.readString();
        IcmpResponse = in.readString();
        Transaction_Sequence = in.readString();
        createTokenRequestXML = in.readString();
        createTokenResponseXML = in.readString();
        Result = in.readString();
        ResultExplanation = in.readString();
        CustomerName = in.readString();
        CustomerCredit = in.readString();
        TransactionApproval = in.readString();
        TransactionCurrency = in.readString();
        TransactionAmount = in.readString();
        FraudAlert = in.readString();
        FraudExplnation = in.readString();
        TransactionNetAmount = in.readString();
        TransactionSettlementDate = in.readString();
        TransactionRollingReserveAmount = in.readString();
        TransactionRollingReserveDate = in.readString();
        CustomerPhone = in.readString();
        CustomerCountry = in.readString();
        CustomerAddress = in.readString();
        CustomerCity = in.readString();
        CustomerZip = in.readString();
        MobilePaymentRequest = in.readString();
        AccRef = in.readString();
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {
        dest.writeLong(id);
        dest.writeString(TransToken);
        dest.writeString(CompanyToken);
        dest.writeString(ReceiptName);
        dest.writeString(DateCreated);
        dest.writeString(CardBin);
        dest.writeString(CardLast4);
        dest.writeString(TransactionSource);
        dest.writeString(PaymentRRN);
        dest.writeString(IcmpResponse);
        dest.writeString(Transaction_Sequence);
        dest.writeString(createTokenRequestXML);
        dest.writeString(createTokenResponseXML);
        dest.writeString(Result);
        dest.writeString(ResultExplanation);
        dest.writeString(CustomerName);
        dest.writeString(CustomerCredit);
        dest.writeString(TransactionApproval);
        dest.writeString(TransactionCurrency);
        dest.writeString(TransactionAmount);
        dest.writeString(FraudAlert);
        dest.writeString(FraudExplnation);
        dest.writeString(TransactionNetAmount);
        dest.writeString(TransactionSettlementDate);
        dest.writeString(TransactionRollingReserveAmount);
        dest.writeString(TransactionRollingReserveDate);
        dest.writeString(CustomerPhone);
        dest.writeString(CustomerCountry);
        dest.writeString(CustomerAddress);
        dest.writeString(CustomerCity);
        dest.writeString(CustomerZip);
        dest.writeString(MobilePaymentRequest);
        dest.writeString(AccRef);
    }

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Creator<Receipt> CREATOR = new Creator<Receipt>() {
        @Override
        public Receipt createFromParcel(Parcel in) {
            return new Receipt(in);
        }

        @Override
        public Receipt[] newArray(int size) {
            return new Receipt[size];
        }
    };

    public String getResult() {
        return Result;
    }

    public String getResultExplanation() {
        return ResultExplanation;
    }

    public String getCustomerName() {
        return CustomerName;
    }

    public String getCustomerCredit() {
        return CustomerCredit;
    }

    public String getTransactionApproval() {
        return TransactionApproval;
    }

    public String getTransactionCurrency() {
        return TransactionCurrency;
    }

    public String getTransactionAmount() {
        return TransactionAmount;
    }

    public String getFraudAlert() {
        return FraudAlert;
    }

    public String getFraudExplnation() {
        return FraudExplnation;
    }

    public String getTransactionNetAmount() {
        return TransactionNetAmount;
    }

    public String getTransactionSettlementDate() {
        return TransactionSettlementDate;
    }

    public String getTransactionRollingReserveAmount() {
        return TransactionRollingReserveAmount;
    }

    public String getTransactionRollingReserveDate() {
        return TransactionRollingReserveDate;
    }

    public String getCustomerPhone() {
        return CustomerPhone;
    }

    public String getCustomerCountry() {
        return CustomerCountry;
    }

    public String getCustomerAddress() {
        return CustomerAddress;
    }

    public String getCustomerCity() {
        return CustomerCity;
    }

    public String getCustomerZip() {
        return CustomerZip;
    }

    public String getMobilePaymentRequest() {
        return MobilePaymentRequest;
    }

    public String getAccRef() {
        return AccRef;
    }


    public Receipt(){

    }


    @Override
    public int compareTo(@NonNull Receipt o) {
        if(o!=null){
            Date d = new Date(this.DateCreated);
            Date e = new Date(o.DateCreated);

            return  d.compareTo(e);
        }
        else{
            return 0;
        }

    }

    public long getId() {
        return id;
    }

    public String getTransToken() {
        return TransToken;
    }

    public String getCardBin() {
        return CardBin;
    }

    public String getCompanyToken() {
        return CompanyToken;
    }

    public String getReceiptName() {
        return ReceiptName;
    }

    public String getDateCreated() {
        return DateCreated;
    }

    public String getCardLast4() {
        return CardLast4;
    }

    public String getTransactionSource() {
        return TransactionSource;
    }

    public String getPaymentRRN() {
        return PaymentRRN;
    }

    public String getIcmpResponse() {
        return IcmpResponse;
    }

    public String getTransaction_Sequence() {
        return Transaction_Sequence;
    }

    public String getCreateTokenRequestXML() {
        return createTokenRequestXML;
    }

    public String getCreateTokenResponseXML() {
        return createTokenResponseXML;
    }


}
