package za.co.kapsch.iticket.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Models.CourtDetailModel;

/**
 * Created by csenekal on 2016-09-14.
 */
public class CourtDetailRepository {

    public static void cleanInsert(List<CourtDetailModel> courtList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<CourtDetailModel, Integer> courtDao = ormDbHelper.getCourtDetailDao();

            OrmDbHelper.deleteAll(courtDao);

            for (CourtDetailModel court: courtList){
                courtDao.create(court);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static List<CourtDetailModel> getCourts() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<CourtDetailModel, Integer> courtDao = ormDbHelper.getCourtDetailDao();

            return courtDao.queryForAll();
        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean hasCourts() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<CourtDetailModel, Integer> courtDao = ormDbHelper.getCourtDetailDao();

            return courtDao.countOf() > 0;

        } finally {
            ormDbHelper.close();
        }
    }
}
