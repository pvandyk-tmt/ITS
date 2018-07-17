package za.co.kapsch.iticket.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Constants;
import za.co.kapsch.iticket.Models.CourtDateModel;

/**
 * Created by CSenekal on 2017/01/24.
 */
public class CourtDateRepository  {

    public static void cleanInsert(List<CourtDateModel> courtList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<CourtDateModel, Integer> courtDateDao = ormDbHelper.getCourtDateDao();

            OrmDbHelper.deleteAll(courtDateDao);

            for (CourtDateModel courtDate: courtList){
                courtDateDao.create(courtDate);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static List<CourtDateModel> getCourtDates(int roomId) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<CourtDateModel, Integer> courtDateDao = ormDbHelper.getCourtDateDao();

            return courtDateDao.query(courtDateDao.queryBuilder()
                    .where()
                    .eq(Constants.TABLE_COURT_DATE_ROOM_ID_COLUMN, roomId).prepare());

        }
        finally {
            ormDbHelper.close();
        }
    }
}