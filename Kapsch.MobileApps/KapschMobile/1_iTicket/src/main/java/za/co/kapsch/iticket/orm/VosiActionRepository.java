package za.co.kapsch.iticket.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Models.VosiActionModel;


/**
 * Created by CSenekal on 2017/09/07.
 */
public class VosiActionRepository {

    public static void cleanInsert(List<VosiActionModel> vosiActionList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<VosiActionModel, Integer> vosiActionDao = ormDbHelper.getVosiActionDao();

            OrmDbHelper.deleteAll(vosiActionDao);

            for (VosiActionModel vosiAction: vosiActionList){
                vosiActionDao.create(vosiAction);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static List<VosiActionModel> getVosiAction() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VosiActionModel, Integer> vosiActionDao = ormDbHelper.getVosiActionDao();

            return vosiActionDao.queryForAll();
        } finally {
            ormDbHelper.close();
        }
    }
}
