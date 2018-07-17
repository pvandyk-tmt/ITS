package za.co.kapsch.iticket.iCam;

import android.graphics.Bitmap;
import android.os.Parcel;
import android.os.Parcelable;

import com.thoughtworks.xstream.annotations.XStreamAlias;
import com.thoughtworks.xstream.annotations.XStreamAsAttribute;
import com.thoughtworks.xstream.annotations.XStreamOmitField;

/**
 * Created by csenekal on 2016-12-14.
 */

@XStreamAlias("infringement")
public class ICamInfringement implements Parcelable {

//Hennie mail Thu 2017/08/31 02:43 PM
//    img-num="1234"
//
//    type="test-shot"
//    type="speed"
//    type="headway"
//    type="red-light"
//    type="stop-line"
//    type="yellow-lane"
//    type="line-violation"
//    type="unknown"


    @XStreamAsAttribute
    @XStreamAlias("timestamp")
    private String mTimestamp;

    @XStreamAsAttribute
    @XStreamAlias("date")
    private String mDate;

    @XStreamAsAttribute
    @XStreamAlias("time")
    private String mTime;

    @XStreamAsAttribute
    @XStreamAlias("speed")
    private String mSpeed;

    @XStreamAsAttribute
    @XStreamAlias("direction")
    private String mDirection;

    @XStreamAsAttribute
    @XStreamAlias("location")
    private String mLocation;

    @XStreamAsAttribute
    @XStreamAlias("highspeed")
    private String mHighspeed;

    @XStreamAsAttribute
    @XStreamAlias("operator")
    private String mOperator;

    @XStreamAsAttribute
    @XStreamAlias("serialnum")
    private String mSerialnum;

    @XStreamAsAttribute
    @XStreamAlias("hwid")
    private String mHwid;

    @XStreamAsAttribute
    @XStreamAlias("distance")
    private String mDistance;

    @XStreamAsAttribute
    @XStreamAlias("zone")
    private String mZone;

    @XStreamAsAttribute
    @XStreamAlias("class")
    private String mClass;

    @XStreamAsAttribute
    @XStreamAlias("filename")
    private String mFilename;

    @XStreamAsAttribute
    @XStreamAlias("img_num")
    private String mImageNumber;

    @XStreamAsAttribute
    @XStreamAlias("type")
    private String mType;

    @XStreamOmitField
    private Bitmap mThumbImage;

    @XStreamOmitField
    private Bitmap mImage;

    @XStreamOmitField
    private ICamVlns mIcamVlns;

    public String getSpeed() {
        return mSpeed;
    }

    public void setSpeed(String speed) {
        mSpeed = speed;
    }

    public String getZone() {
        return mZone;
    }

    public void setZone(String zone) {
        mZone = zone;
    }

    public Bitmap getThumbImage() {
        return mThumbImage;
    }

    public void setThumbImage(Bitmap image) {
        mThumbImage = image;
    }

    public Bitmap getImage() {
        return mImage;
    }

    public void setImage(Bitmap image) {
        mThumbImage = image;
    }


    public String getFilename() {
        return mFilename;
    }

    public void setFilename(String filename) {
        mFilename = filename;
    }

    public String getTimestamp() {
        return mTimestamp;
    }

    public void setTimestamp(String timestamp) {
        mTimestamp = timestamp;
    }

    public String getDate() {
        return mDate;
    }

    public void setDate(String date) {
        mDate = date;
    }

    public String getTime() {
        return mTime;
    }

    public void setTime(String time) {
        mTime = time;
    }

    public String getDirection() {
        return mDirection;
    }

    public void setDirection(String direction) {
        mDirection = direction;
    }

    public String getLocation() {
        return mLocation;
    }

    public void setLocation(String location) {
        mLocation = location;
    }

    public String getHighspeed() {
        return mHighspeed;
    }

    public void setHighspeed(String highspeed) {
        mHighspeed = highspeed;
    }

    public String getOperator() {
        return mOperator;
    }

    public void setOperator(String operator) {
        mOperator = operator;
    }

    public String getSerialnum() {
        return mSerialnum;
    }

    public void setSerialnum(String serialnum) {
        this.mSerialnum = serialnum;
    }

    public String getHwid() {
        return mHwid;
    }

    public void setHwid(String hwid) {
        this.mHwid = hwid;
    }

    public String getDistance() {
        return mDistance;
    }

    public void setDistance(String distance) {
        this.mDistance = distance;
    }

    public String getVehicleClass() {
        return mClass;
    }

    public void setVehicleClass(String vehicleClass) {
        mClass = vehicleClass;
    }

    public void setImageNumber(String imageNumber){
        mImageNumber = imageNumber;
    }

    public String getImageNumber(){
        return mImageNumber;
    }

    public String getType() {
        return mType;
    }

    public void setType(String type) {
        mType = type;
    }

    public ICamVlns getIcamVlns() {
        return mIcamVlns;
    }

    public void setIcamVlns(ICamVlns iCamVlns) {
        mIcamVlns = iCamVlns;
    }

    public ICamInfringement(){}

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(mTimestamp);
        out.writeString(mDate);
        out.writeString(mTime);
        out.writeString(mSpeed);
        out.writeString(mDirection);
        out.writeString(mLocation);
        out.writeString(mHighspeed);
        out.writeString(mOperator);
        out.writeString(mSerialnum);
        out.writeString(mHwid);
        out.writeString(mDistance);
        out.writeString(mZone);
        out.writeString(mClass);
        out.writeString(mFilename);
        out.writeString(mImageNumber);
        out.writeString(mType);
        out.writeParcelable(mThumbImage, flags);
        out.writeParcelable(mImage, flags);
        out.writeParcelable(mIcamVlns, flags);
    }

    public static final Parcelable.Creator<ICamInfringement> CREATOR = new Parcelable.Creator<ICamInfringement>() {

        public ICamInfringement createFromParcel(Parcel in) {
            return new ICamInfringement(in);
        }

        public ICamInfringement[] newArray(int size) {
            return new ICamInfringement[size];
        }
    };

    private ICamInfringement(Parcel in) {
        mTimestamp = in.readString();
        mDate = in.readString();
        mTime = in.readString();
        mSpeed = in.readString();
        mDirection = in.readString();
        mLocation = in.readString();
        mHighspeed = in.readString();
        mOperator = in.readString();
        mSerialnum = in.readString();
        mHwid = in.readString();
        mDistance = in.readString();
        mZone = in.readString();
        mClass = in.readString();
        mFilename = in.readString();
        mImageNumber = in.readString();
        mType = in.readString();
        mThumbImage = in.readParcelable(Bitmap.class.getClassLoader());
        mImage = in.readParcelable(Bitmap.class.getClassLoader());
        mIcamVlns = in.readParcelable(ICamVlns.class.getClassLoader());
    }
}