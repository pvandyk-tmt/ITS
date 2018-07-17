package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;

import java.sql.Date;

/**
 * Created by csenekal on 2017-01-13.
 * This object is used to inteface with the iSolution system(Device event repository and monitor)
 */

public class ISolutionCoreDeviceModel {

    @SerializedName("AdapterType")
    private String AdapterType;
    @SerializedName("DeviceID")
    private String DeviceID;
    @SerializedName("SolutionID")
    private String SolutionID;
    @SerializedName("SolutionName")
    private String SolutionName;
    @SerializedName("FriendlyName")
    private String FriendlyName;
    @SerializedName("ConnectToHost")
    private boolean ConnectToHost;
    @SerializedName("GpsLatitude")
    private double GpsLatitude;
    @SerializedName("GpsLongitude")
    private double GpsLongitude;
    @SerializedName("DataSourceID")
    private String DataSourceID;
    @SerializedName("DeviceConnectionType")
    private String DeviceConnectionType;
    @SerializedName("IsEnabled")
    private boolean IsEnabled;
    @SerializedName("ConfigJson")
    private String ConfigJson;
    @SerializedName("CreatedTimeStamp")
    private Date CreatedTimeStamp;
    @SerializedName("UpdatedTimeStamp")
    private Date UpdatedTimeStamp;
    @SerializedName("DeviceStatus")
    private String DeviceStatus;
}
