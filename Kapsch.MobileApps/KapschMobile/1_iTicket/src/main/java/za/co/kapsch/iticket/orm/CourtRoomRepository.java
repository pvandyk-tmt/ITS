package za.co.kapsch.iticket.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Constants;
import za.co.kapsch.iticket.Models.CourtRoomModel;

/**
 * Created by CSenekal on 2017/01/24.
 */
public class CourtRoomRepository {

    public static void cleanInsert(List<CourtRoomModel> courtRoomList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<CourtRoomModel, Integer> courtRoomDao = ormDbHelper.getCourtRoomDao();

            OrmDbHelper.deleteAll(courtRoomDao);

            for (CourtRoomModel courtRoom: courtRoomList){
                courtRoomDao.create(courtRoom);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static List<CourtRoomModel> getCourtRooms(int courtId) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<CourtRoomModel, Integer> courtRoomDao = ormDbHelper.getCourtRoomDao();

            return courtRoomDao.query(courtRoomDao.queryBuilder()
                    .where()
                    .eq(Constants.TABLE_COURT_ROOM_COURT_ID_COLUMN, courtId).prepare());

        }
        finally {
            ormDbHelper.close();
        }
    }
}