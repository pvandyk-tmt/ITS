package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;

import za.co.kapsch.iticket.Enums.DistanceOverTimeServiceTypeEnum;

/**
 * Created by CSenekal on 2017/06/02.
 */
public class DistanceOverTimeSectionConfigurationModel {

    @SerializedName("ServiceType")
    private DistanceOverTimeServiceTypeEnum mServiceType;

    @SerializedName("PollMs")
    private int mPollMs;

    @SerializedName("UniqueIdentifier")
    private String mUniqueIdentifier;

//    @SerializedName("SectionDistanceInMeter")
//    private int mSectionDistanceInMeter;
//    @SerializedName("SectionCode")
//    private String mSectionCode;
//    @SerializedName("SectionDescription")
//    private String mSectionDescription;
//    @SerializedName("PointALocationCode")
//    private String mPointALocationCode;
//    @SerializedName("PointBLocationCode")
//    private String mPointBLocationCode;
//    @SerializedName("PointAIpAddress")
//    private String mPointAIpAddress;
//    @SerializedName("PointBIpAddress")
//    private String mPointBIpAddress;
//    @SerializedName("PointAPort")
//    private int mPointAPort;
//    @SerializedName("PointBPort")
//    private int mPointBPort;
//    @SerializedName("SourceFilePath")
//    private String mSourceFilePath;
//    @SerializedName("LevenshteinMatchDistance")
//    private int mLevenshteinMatchDistance;

    public DistanceOverTimeServiceTypeEnum getServiceType() {
        return mServiceType;
    }

    public void setServiceType(DistanceOverTimeServiceTypeEnum serviceType) {
        mServiceType = serviceType;
    }

    public int getPollMs()
    {
        return mPollMs;
    };

    public void setPollMs(int pollMs){
        mPollMs = pollMs;
    }

    public void setUniqueIdentifier(String uniqueIdentifier){
        mUniqueIdentifier = uniqueIdentifier;
    }

    public String getUniqueIdentifier(){
        return mUniqueIdentifier;
    };

//    public String getPointAIpAddress() {
//        return mPointAIpAddress;
//    }
//
//    public void setPointAIpAddress(String pointAIpAddress) {
//        mPointAIpAddress = pointAIpAddress;
//    }
//
//    public String getPointBIpAddress() {
//        return mPointBIpAddress;
//    }
//
//    public void setPointBIpAddress(String pointBIpAddress) {
//        mPointBIpAddress = pointBIpAddress;
//    }
//
//    public int getPointAPort() {
//        return mPointAPort;
//    }
//
//    public void setPointAPort(int pointAPort) {
//        mPointAPort = pointAPort;
//    }
//
//    public int getPointBPort() {
//        return mPointBPort;
//    }
//
//    public void setPointBPort(int pointBPort) {
//        mPointBPort = pointBPort;
//    }
}
