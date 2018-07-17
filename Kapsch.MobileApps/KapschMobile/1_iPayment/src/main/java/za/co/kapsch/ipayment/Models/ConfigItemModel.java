package za.co.kapsch.ipayment.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.sql.SQLException;

import za.co.kapsch.shared.Enums.Environment;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.ipayment.orm.ConfigItemRepository;

/**
 * Created by CSenekal on 2017/01/19.
 */
@DatabaseTable(tableName = "ConfigItem")
public class ConfigItemModel {

    public static Environment mEnvironment = Environment.Staging;

    //staging
    public static String mStagingCoreGateway = "http://192.168.0.33:60001%s";
    //Live
    public static String mProductionCoreGateway = "http://192.168.0.17:81%s";

    //staging
    public static final String mStagingITSGateway = "http://192.168.0.33:60002%s";
    //Live
    public static final String mProductionITSGateway = "http://192.168.0.17:85%s";

    private static final String HttpReadTimeout = "HTTP_READ_TIMEOUT";
    private static final String HttpConnectTimeout = "HTTP_CONNECT_TIMEOUT";
    private static final String ReceiptWebsiteAddress = "RECEIPT_WEBSITE_ADDRESS";
    private static final String ReceiptCallCentreNo = "RECEIPT_CALL_CENTRE_NO";

    private static ConfigItemModel mInstance;

    @DatabaseField(columnName = "ID", generatedId = true)
    @SerializedName("ID")
    private long mID;

    @SerializedName("Name")
    @DatabaseField(columnName = "Name")
    private String mDescription;

    @SerializedName("Value")
    @DatabaseField(columnName = "Value")
    private String mValue;

    private int mHttpReadTimeOut = 40000;
    private int mHttpConnectTimeOut = 30000;
    private String mReceiptWebsiteAddress = "RECEIPT_WEBSITE_ADDRESS";
    private String mReceiptCallCentreNo = "RECEIPT_CALL_CENTRE_NO";

    public int getHttpReadTimeOut(){
        return mHttpReadTimeOut;
    }

    public int getHttpConnectTimeOut(){
        return mHttpConnectTimeOut;
    }

    public synchronized static ConfigItemModel getInstance()
    {
        if (mInstance == null)
        {
            mInstance = new ConfigItemModel();
        }
        return mInstance;
    }

    public static String getCoreGateway(){
        switch (mEnvironment){
            case Staging:
                return mStagingCoreGateway;
            case Production:
                return mProductionCoreGateway;
            default:
                return null;
        }
    }

    public static String getITSGateway(){
        switch (mEnvironment){
            case Staging:
                 return mStagingITSGateway;
            case Production:
                return mProductionITSGateway;
            default:
                return null;
        }
    }

    public void refreshConfigurationValues(){

         try {
            mHttpReadTimeOut = Integer.parseInt(ConfigItemRepository.getConfigItem(HttpReadTimeout).getValue());
            mHttpConnectTimeOut = Integer.parseInt(ConfigItemRepository.getConfigItem(HttpConnectTimeout).getValue());
            mReceiptWebsiteAddress = ConfigItemRepository.getConfigItem(ReceiptWebsiteAddress).getValue();
            mReceiptCallCentreNo = ConfigItemRepository.getConfigItem(ReceiptCallCentreNo).getValue();
         }catch (SQLException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ConfigItemModel::refreshConfigurationValues()"), ErrorSeverity.High);
        }catch (Exception ex){
            MessageManager.showMessage(Utilities.exceptionMessage(ex, "ConfigItemModel::refreshConfigurationValues()"), ErrorSeverity.High);
        }
    }

    public long getID() {
        return mID;
    }

    public void setID(long id) {
        mID = id;
    }

    public String getName() {
        return mDescription;
    }

    public void setName(String description) {
        mDescription = description;
    }

    public String getValue() {
        return mValue;
    }

    public void setValue(String value) {
        mValue = value;
    }

    public String getDescription() {
        return mDescription;
    }

    public void setDescription(String description) {
        mDescription = description;
    }

     public String getReceiptWebsiteAddress() {
        return mReceiptWebsiteAddress;
    }

    public void setReceiptWebsiteAddress(String receiptWebsiteAddress) {
        mReceiptWebsiteAddress = receiptWebsiteAddress;
    }

    public String getReceiptCallCentreNo() {
        return mReceiptCallCentreNo;
    }

    public void setReceiptCallCentreNo(String receiptCallCentreNo) {
        mReceiptCallCentreNo = receiptCallCentreNo;
    }
}
