package za.co.kapsch.iticket.orm;

import android.os.Parcel;
import android.os.Parcelable;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.Date;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Constants;
import za.co.kapsch.iticket.Interfaces.IDelimitationRepository;
import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.iticket.Models.CourtDateModel;
import za.co.kapsch.iticket.Models.CourtDetailModel;
import za.co.kapsch.iticket.Models.CourtInfoModel;
import za.co.kapsch.iticket.Models.CourtModel;
import za.co.kapsch.iticket.Models.CourtRoomModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/01/27.
 */
public class CourtsInfoRepository implements IDelimitationRepository, Parcelable {

    @Override
    public <T> T getFirstLevel() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<CourtModel, Integer> courtDao = ormDbHelper.getCourtDao();
            return (T)courtDao.queryForAll();
        } finally {
            ormDbHelper.close();
        }
    }

    @Override
    public <T> T getSecondLevel(int firstLevelId) throws SQLException  {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<CourtRoomModel, Integer> courtRoomDao = ormDbHelper.getCourtRoomDao();

            List<CourtRoomModel> courtRooms = courtRoomDao.query(courtRoomDao.queryBuilder()
                                                .where()
                                                .eq(Constants.TABLE_COURT_ROOM_COURT_ID_COLUMN, firstLevelId).prepare());

            if (courtRooms.size() > 0){
                return (T)courtRooms;
            }

            return (T)courtRoomDao.queryForAll();

        } finally {
            ormDbHelper.close();
        }
    }

//    public <T> T getThirdLevel(int secondLevelId) throws SQLException  {
//
//        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());
//
//        try {
//            final Dao<CourtDateModel, Integer> courtDateDao = ormDbHelper.getCourtDateDao();
//
//            List<CourtDateModel> courtDates = courtDateDao.queryForAll();
//
//            return (T)courtDateDao.query(courtDateDao.queryBuilder()
//                    .where()
//                    .eq(Constants.TABLE_COURT_DATE_ROOM_ID_COLUMN, secondLevelId)
//                    .and()
//                    .gt(Constants.TABLE_COURT_DATE_DATE_COLUMN, Utilities.addDaysToDate(ConfigurationModel.COURT_DATE_FROM_NOW)).prepare());
//
//        } finally {
//            ormDbHelper.close();
//        }
//    }

    public <T> T getThirdLevel(int secondLevelId) throws SQLException  {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<CourtDateModel, Integer> courtDateDao = ormDbHelper.getCourtDateDao();

            List<CourtDateModel> courtDates = courtDateDao.queryForAll();

            return (T)courtDateDao.query(courtDateDao.queryBuilder()
                    .orderBy(Constants.TABLE_COURT_DATE_DATE_COLUMN, true)
                    .where()
                    .eq(Constants.TABLE_COURT_DATE_ROOM_ID_COLUMN, secondLevelId)
                    .and()
                    .gt(Constants.TABLE_COURT_DATE_DATE_COLUMN, Utilities.addDaysToDate(1)/*Utilities.addDaysToDate(ConfigItemModel.getInstance().getCourtDateFromNow())*/).prepare());

        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean hasCourts() throws SQLException  {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<CourtModel, Integer> courtDao = ormDbHelper.getCourtDao();
            final Dao<CourtRoomModel, Integer> courtRoomDao = ormDbHelper.getCourtRoomDao();
            final Dao<CourtDateModel, Integer> courtDateDao = ormDbHelper.getCourtDateDao();

            return (courtDao.countOf() > 0) && (courtRoomDao.countOf() > 0) && (courtDateDao.countOf() > 0);

        } finally {
            ormDbHelper.close();
        }

    }

    public <T> T getFourthLevel(int thirdLevelId) throws SQLException  {
        return null;
    }

    public <T> T getFifthLevel(int fourthLevelId) throws SQLException  {
        return null;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {

    }

    public static final Parcelable.Creator<CourtsInfoRepository> CREATOR = new Parcelable.Creator<CourtsInfoRepository>() {
        public CourtsInfoRepository createFromParcel(Parcel in) {
            return new CourtsInfoRepository(in);
        }

        public CourtsInfoRepository[] newArray(int size) {
            return new CourtsInfoRepository[size];
        }
    };

    public CourtsInfoRepository(){}

    private CourtsInfoRepository(Parcel in){
    }
}