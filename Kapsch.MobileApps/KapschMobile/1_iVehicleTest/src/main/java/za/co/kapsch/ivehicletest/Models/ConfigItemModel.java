package za.co.kapsch.ivehicletest.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.sql.SQLException;

import za.co.kapsch.ivehicletest.orm.ConfigItemRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/01/19.
 */
@DatabaseTable(tableName = "ConfigItem")
public class ConfigItemModel {

    private static final String HttpReadTimeout = "HTTP_READ_TIMEOUT";
    private static final String HttpConnectTimeout = "HTTP_CONNECT_TIMEOUT";
    private static final String NagAmount = "NAG_AMOUNT";
    private static final String PayDateFromCourtDate = "PAY_DATE_FROM_COURT_DATE";
    private static final String CourtDateFromNow = "COURT_DATE_FROM_NOW";
    private static final String OffenceMinutesFromNow = "OFFENCE_MINUTES_FROM_NOW";
    private static final String TmtApiUser = "TMT_API_USER";
    private static final String MinTickets = "MIN_TICKETS";
    private static final String TicketBookSize = "TICKET_BOOK_SIZE";
    private static final String GpsInterval = "GPS_INTERVAL";

    private static final String paymentDetailHeading = "PAYMENT_DETAILS_HEADING";
    private static final String paymentDetailInfo = "PAYMENT_DETAIL_INFO";

    private static ConfigItemModel mInstance;

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", generatedId = true)
    private int mID;

    @SerializedName("Name")
    @DatabaseField(columnName = "Name")
    private String mDescription;

    @SerializedName("Value")
    @DatabaseField(columnName = "Value")
    private String mValue;

    private int mHttpReadTimeOut = 40000;
    private int mHttpConnectTimeOut = 30000;
    private int mNagAmount = 9999;
    private int mPayDateFromCourtDate = -14;
    private int mCourtDateFromNow = 1;
    private int mOffenceMinutesFromNow = -10;
    private String mTmtApiUser = "TMT_API_USER";
    private int mMinTickets = 5;
    private int mTicketBookSize = 10;
    private int mGpsInterval = 1000*60;//*15; // 15 minutes
    private String mPaymentDetailHeading = "PAYMENT LOCATION";
    private String mPaymentDetailInfo = "Lusaka Magistrate Court: John Mbita Road";

    public int getHttpReadTimeOut(){
        return mHttpReadTimeOut;
    }

    public int getHttpConnectTimeOut(){
        return mHttpConnectTimeOut;
    }

    public int getNagAmount(){
        return mNagAmount;
    }

    public int getPayDateFromCourtDate(){
        return mPayDateFromCourtDate;
    }

    public int getCourtDateFromNow(){
        return mCourtDateFromNow;
    }

    public int getOffenceMinutesFromNow(){
        return mOffenceMinutesFromNow;
    }

    public String getTmtApiUser(){
        return mTmtApiUser;
    }

    public int getMinTickets(){
        return mMinTickets;
    }

    public int getTicketBookSize(){
        return mTicketBookSize;
    }

    public int getGpsInterval(){
        return mGpsInterval;
    }

    public String getPaymentDetailsHeading() { return mPaymentDetailHeading; }

    public String getPaymentDetailsInfo() { return mPaymentDetailInfo; }

    public synchronized static ConfigItemModel getInstance()
    {
        if (mInstance == null)
        {
            mInstance = new ConfigItemModel();
        }
        return mInstance;
    }

    public void refreshConfigurationValues(){

        try {
            mHttpReadTimeOut = Integer.parseInt(ConfigItemRepository.getConfigItem(HttpReadTimeout).getValue());
            mHttpConnectTimeOut = Integer.parseInt(ConfigItemRepository.getConfigItem(HttpConnectTimeout).getValue());
            mTmtApiUser = ConfigItemRepository.getConfigItem(TmtApiUser).getValue();
         }catch (SQLException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ConfigItemModel::refreshConfigurationValues()"), ErrorSeverity.High);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ConfigItemModel::refreshConfigurationValues()"), ErrorSeverity.High);
        }
    }

    public int getID() {
        return mID;
    }

    public void setID(int id) {
        mID = id;
    }

    public String getDescription() {
        return mDescription;
    }

    public void setDescription(String description) {
        mDescription = description;
    }

    public String getValue() {
        return mValue;
    }

    public void setValue(String value) {
        mValue = value;
    }
}
