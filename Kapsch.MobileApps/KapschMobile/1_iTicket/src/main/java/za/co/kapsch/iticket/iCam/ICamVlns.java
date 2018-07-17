package za.co.kapsch.iticket.iCam;

import android.graphics.Bitmap;
import android.os.Parcel;
import android.os.Parcelable;
import android.text.TextUtils;

import com.thoughtworks.xstream.annotations.XStreamAlias;
import com.thoughtworks.xstream.annotations.XStreamAsAttribute;
import com.thoughtworks.xstream.annotations.XStreamOmitField;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by csenekal on 2016-12-13.
 */

@XStreamAlias("vlns")
public class ICamVlns implements Parcelable {

    @XStreamAsAttribute
    @XStreamAlias("timestamp")
    public String mTimestamp;

    @XStreamAsAttribute
    @XStreamAlias("date")
    public String mDate;

    @XStreamAsAttribute
    @XStreamAlias("time")
    public String mTime;

    @XStreamAsAttribute
    @XStreamAlias("serialnum")
    public String mSerialnum;

    @XStreamAsAttribute
    @XStreamAlias("hwid")
    public String mHwid;

    @XStreamAsAttribute
    @XStreamAlias("plates")
    public String mPlates;

    @XStreamAsAttribute
    @XStreamAlias("filename")
    private String mFilename;

    @XStreamOmitField
    private Bitmap mThumbImage;

    @XStreamOmitField
    private Bitmap mImage;

    public List<String> mVosiFields;

    public ICamVlns(){}

    protected ICamVlns(Parcel in) {
        mTimestamp = in.readString();
        mDate = in.readString();
        mTime = in.readString();
        mSerialnum = in.readString();
        mHwid = in.readString();
        mPlates = in.readString();
        mFilename = in.readString();
        mThumbImage = in.readParcelable(Bitmap.class.getClassLoader());
        mVosiFields = in.createStringArrayList();
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {
        dest.writeString(mTimestamp);
        dest.writeString(mDate);
        dest.writeString(mTime);
        dest.writeString(mSerialnum);
        dest.writeString(mHwid);
        dest.writeString(mPlates);
        dest.writeString(mFilename);
        dest.writeParcelable(mThumbImage, flags);
        dest.writeStringList(mVosiFields);
    }

    @Override
    public int describeContents() {
        return 0;
    }

    public static final Creator<ICamVlns> CREATOR = new Creator<ICamVlns>() {
        @Override
        public ICamVlns createFromParcel(Parcel in) {
            return new ICamVlns(in);
        }

        @Override
        public ICamVlns[] newArray(int size) {
            return new ICamVlns[size];
        }
    };

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

    public String getSerialnum() {
        return mSerialnum;
    }

    public void setSerialnum(String serialnum) {
        mSerialnum = serialnum;
    }

    public String getHwid() {
        return mHwid;
    }

    public void setHwid(String hwid) {
        mHwid = hwid;
    }

    public String getPlates() {
        return mPlates;
    }

    public Bitmap getImage() {
        return mImage;
    }

    public void setImage(Bitmap image) {
        mThumbImage = image;
    }


    public Bitmap getThumbImage() {
        return mThumbImage;
    }

    public void setThumbImage(Bitmap thumbImage) {
        mThumbImage = thumbImage;
    }

    public void setPlates(String plates) {
        mPlates = plates;
    }

    public String getFilename() {
        return mFilename;
    }

    public void setFilename(String filename) {
        mFilename = filename;
    }

    public String getPlate(){
        return mPlates.split(",")[0];
    }

    public String getVosiReason(){

        getVosiFields();

        if (mVosiFields.size() > 0) {
            return mVosiFields.get(0);
        }

        return null;
    }

    private List<String> getVosiFields() {

        if(mVosiFields == null) {

            mVosiFields = new ArrayList<>();

            if (hasVosi() == true){
                String[] plateFields = mPlates.split(",");

                if (plateFields.length > 2){
                    if (TextUtils.isEmpty(plateFields[2]) == false){

                        String[] vosiFields = plateFields[2].split("\\*");
                        for (String vosiField: vosiFields) {
                            mVosiFields.add(vosiField);
                        }
                    }
                }
            }
        }

        return mVosiFields;
    }

    public boolean hasVosi(){

        String[] plateFields = mPlates.split(",");

        if (plateFields.length > 2) {
            if (TextUtils.isEmpty(plateFields[2]) == false){
                return true;
            }
        }

        return false;
    }
}
