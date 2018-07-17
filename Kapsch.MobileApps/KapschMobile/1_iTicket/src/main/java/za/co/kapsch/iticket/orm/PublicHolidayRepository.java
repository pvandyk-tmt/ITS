package za.co.kapsch.iticket.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Models.PublicHolidayModel;

/**
 * Created by csenekal on 2016-09-15.
 */
public class PublicHolidayRepository {

    public static void cleanInsert(List<PublicHolidayModel> publicHolidayList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<PublicHolidayModel, Integer> publicHolidayDao = ormDbHelper.getPublicHolidayDao();

            //publicHolidayDao.delete(publicHolidayDao.queryForAll());
            OrmDbHelper.deleteAll(publicHolidayDao);

            for (PublicHolidayModel publicHoliday: publicHolidayList){
                publicHolidayDao.create(publicHoliday);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }
}
