package za.co.kapsch.console.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.sql.SQLException;

import za.co.kapsch.shared.Enums.Environment;
import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.console.orm.ConfigItemRepository;

/**
 * Created by CSenekal on 2017/01/19.
 */
@DatabaseTable(tableName = "ConfigItem")
public class ConfigItemModel {

//    public static Environment mEnvironment = Environment.Training;
//
//    public static String mStagingCoreGateway = "http://192.168.0.33:60001%s";
//
//    public static String mTrainingCoreGateway = "http://10.0.13.71:60001%s";
//
//    public static String mProductionCoreGateway = "http://192.168.0.17:81%s";
//
//
//    public static final String mStagingITSGateway = "http://192.168.0.33:60002%s";
//
//    public static String mTrainingITSGateway = "http://10.0.13.71:60001%s";
//
//    public static final String mProductionITSGateway = "http://192.168.0.17:85%s";
//

    private static final String HttpReadTimeout = "HTTP_READ_TIMEOUT";
    private static final String HttpConnectTimeout = "HTTP_CONNECT_TIMEOUT";
    private static final String TmtApiUser = "TMT_API_USER";
    private static final String GpsInterval = "GPS_INTERVAL";

    private static ConfigItemModel mInstance;

    @DatabaseField(columnName = "ID", generatedId = true)
    @SerializedName("ID")
    private long mID;

    @SerializedName("Name")
    @DatabaseField(columnName = "Name")
    private String mName;

    @SerializedName("Value")
    @DatabaseField(columnName = "Value")
    private String mValue;

    private int mHttpReadTimeOut = 40000;
    private int mHttpConnectTimeOut = 30000;
    private String mTmtApiUser = "TMT_API_USER";
    private int mGpsInterval = 1000*60;//*15; // 15 minutes

    public int getHttpReadTimeOut(){
        return mHttpReadTimeOut;
    }

    public int getHttpConnectTimeOut(){
        return mHttpConnectTimeOut;
    }

    public String getTmtApiUser(){
        return mTmtApiUser;
    }

    public int getGpsInterval(){
        return mGpsInterval;
    }

    public synchronized static ConfigItemModel getInstance()
    {
        if (mInstance == null)
        {
            mInstance = new ConfigItemModel();
        }
        return mInstance;
    }
//
//    public static String getCoreGateway(){
//        switch (mEnvironment){
//            case Staging:
//                return mStagingCoreGateway;
//            case Production:
//                return mProductionCoreGateway;
//            case Training:
//                return mTrainingCoreGateway;
//            default:
//                return null;
//        }
//    }
//
//    public static String getITSGateway(){
//        switch (mEnvironment){
//            case Staging:
//                 return mStagingITSGateway;
//            case Production:
//                return mProductionITSGateway;
//            case Training:
//                return mTrainingITSGateway;
//            default:
//                return null;
//        }
//    }

    public void refreshConfigurationValues(){

        try {
            mHttpReadTimeOut = Integer.parseInt(ConfigItemRepository.getConfigItem(HttpReadTimeout).getValue());
            mHttpConnectTimeOut = Integer.parseInt(ConfigItemRepository.getConfigItem(HttpConnectTimeout).getValue());
            mTmtApiUser = ConfigItemRepository.getConfigItem(TmtApiUser).getValue();
            mGpsInterval = Integer.parseInt(ConfigItemRepository.getConfigItem(GpsInterval).getValue());
        }catch (SQLException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ConfigItemModel::refreshConfigurationValues()"), ErrorSeverity.High);
        }
    }

    public long getID() {
        return mID;
    }

    public void setID(long id) {
        mID = id;
    }

    public String getName() {
        return mName;
    }

    public void setName(String description) {
        mName = description;
    }

    public String getValue() {
        return mValue;
    }

    public void setValue(String value) {
        mValue = value;
    }

    private boolean validateConfigurationValue(String description, ConfigItemModel configurationItem){

        if (configurationItem == null){
            MessageManager.showMessage(String.format("Configuration item %s not found in db. Default value will be used", description), ErrorSeverity.None);
            return false;
        }

        return true;
    }
}
