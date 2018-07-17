package za.co.kapsch.shared.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.sql.SQLException;

import za.co.kapsch.shared.orm.ConfigItemRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/01/19.
 */
@DatabaseTable(tableName = "ConfigItem")
public class ConfigItemModel {

    private static final String CoreGateway = "CORE_GATEWAY";
    private static final String ITSGateway = "ITS_GATEWAY";
    private static final String EVRGateway = "EVR_GATEWAY";

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

    private String mCoreGateway = "http://192.168.0.33:60001%s";
    private String mITSGateway = "http://192.168.0.33:60002%s";
    private String mEVRGateway = "http://192.168.0.33:60004%s";

    public String getCoreGateway(){
        return mCoreGateway;
    }
    public String getITSGateway(){
        return mITSGateway;
    }
    public String getEVRGateway(){
        return mEVRGateway;
    }

    private ConfigItemModel() {

    }

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
            mCoreGateway = ConfigItemRepository.getConfigItem(CoreGateway).getValue();
            mITSGateway = ConfigItemRepository.getConfigItem(ITSGateway).getValue();
            mEVRGateway = ConfigItemRepository.getConfigItem(EVRGateway).getValue();
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
