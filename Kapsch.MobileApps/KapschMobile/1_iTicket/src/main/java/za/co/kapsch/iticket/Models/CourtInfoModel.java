package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

/**
 * Created by CSenekal on 2017/01/28.
 */
public class CourtInfoModel  extends DelimitationResultModel implements Parcelable {

    public CourtModel getCourt(){
        return (CourtModel)getLevelOneObject();
    }

    public void setCourt(CourtModel court){
        setLevelOneObject(court);
    }

    public CourtRoomModel getCourtRoom(){
        return (CourtRoomModel)getLevelTwoObject();
    }

    public void setCourtRoom(CourtRoomModel courtRoom){
        setLevelTwoObject(courtRoom);
    }

    public CourtDateModel getCourtDate(){
        return (CourtDateModel)getLevelThreeObject();
    }

    public void setCourtDate(CourtDateModel courtDate){
        setLevelThreeObject(courtDate);
    }


//    @SerializedName("Courts")
//    public CourtDetailModel mCourt;
//
//    @SerializedName("CourtRooms")
//    public CourtRoomModel mCourtRoom;
//
//    @SerializedName("CourtDates")
//    public CourtDateModel mCourtDate;
//
//    public CourtDetailModel getCourt() {
//        return mCourt;
//    }
//
//    public CourtRoomModel getCourtRoom() {
//        return mCourtRoom;
//    }
//
//    public CourtDateModel getCourtDate() {
//        return mCourtDate;
//    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        write(getLevelOneObject(), out, flags);
        write(getLevelTwoObject(), out, flags);
        write(getLevelThreeObject(), out, flags);
    }

    public static final Parcelable.Creator<CourtInfoModel> CREATOR = new Parcelable.Creator<CourtInfoModel>() {
        public CourtInfoModel createFromParcel(Parcel in) {
            return new CourtInfoModel(in);
        }

        public CourtInfoModel[] newArray(int size) {
            return new CourtInfoModel[size];
        }
    };

    public CourtInfoModel(){}

    private CourtInfoModel(Parcel in){
        setLevelOneObject(read(in));
        setLevelTwoObject(read(in));
        setLevelThreeObject(read(in));    }

//    @Override
//    public void setFirstLevel(Object object) {
//        mCourt = (CourtDetailModel) object;
//    }
//
//    @Override
//    public void setSecondLevel(Object object) {
//        mCourtRoom = (CourtRoomModel) object;
//    }
//
//    @Override
//    public void setThirdLevel(Object object) {
//        mCourtDate = (CourtDateModel) object;
//    }
//
//    @Override
//    public void setFourthLevel(Object object) {
//        //not used
//    }
//
//    @Override
//    public void setFifthLevel(Object object) {
//        //not used
//    }
}
