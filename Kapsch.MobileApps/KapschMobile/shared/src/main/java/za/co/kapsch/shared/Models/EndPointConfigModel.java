package za.co.kapsch.shared.Models;

import android.os.Message;

import java.sql.SQLException;

import za.co.kapsch.shared.Enums.Environment;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.orm.ConfigItemRepository;

/**
 * Created by CSenekal on 2017/10/12.
 */

public class EndPointConfigModel {

    private static EndPointConfigModel mInstance;

    public static Environment mEnvironment = Environment.Staging;

    private static String mCoreGateway;
    private static String mITSGateway;
    private static String mEVRGateway;

    public static final String mStagingEnforcementGateway = "http://192.168.0.33:82%s";
    public static final String mStagingDistanceOverTimeGateway = "http://192.168.0.33:88%s";

    private EndPointConfigModel() {

    }

    public synchronized static EndPointConfigModel getInstance()
    {
        if (mInstance == null)
        {
            mInstance = new EndPointConfigModel();
        }
        return mInstance;
    }

    public String getCoreGateway() {
        return mCoreGateway;
    }

    public void setCoreGateway(String coreGateway) {
        mCoreGateway = coreGateway;
    }

    public String getITSGateway() {
        return mITSGateway;
    }

    public void setITSGateway(String iTSGateway) {
        mITSGateway = iTSGateway;
    }

    public String getEVRGateway() {
        return mEVRGateway;
    }

    public void setEVRGateway(String eVRGateway) {
        mEVRGateway = eVRGateway;
    }

    public static String getEnforcementGateway(){
        switch (mEnvironment){
            case Staging:
                return mStagingEnforcementGateway;
            default:
                return null;
        }
    }

    public static String getDistanceOverTimeGateway(){
        switch (mEnvironment){
            case Staging:
                return mStagingDistanceOverTimeGateway;
            default:
                return null;
        }
    }
}
