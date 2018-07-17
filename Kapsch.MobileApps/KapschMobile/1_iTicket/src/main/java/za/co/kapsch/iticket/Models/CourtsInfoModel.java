package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;

import java.util.List;

/**
 * Created by CSenekal on 2017/01/25.
 */
public class CourtsInfoModel {

    @SerializedName("Courts")
    public List<CourtModel> mCourts;

    @SerializedName("CourtRooms")
    public List<CourtRoomModel> mCourtRooms;

    @SerializedName("CourtDates")
    public List<CourtDateModel> mCourtDates;

    public List<CourtModel> getCourts() {
        return mCourts;
    }

    public List<CourtRoomModel> getCourtRooms() {
        return mCourtRooms;
    }

    public List<CourtDateModel> getCourtDates() {
        return mCourtDates;
    }
}
