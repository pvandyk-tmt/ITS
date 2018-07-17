package za.co.kapsch.iticket.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Models.HandWrittenModel;
import za.co.kapsch.iticket.Models.VosiActionCaptureModel;
import za.co.kapsch.iticket.Models.VosiActionModel;

/**
 * Created by CSenekal on 2017/09/07.
 */
public class VosiActionCaptureRepository {

    public static final String UPLOADED_COLUMN = "Uploaded";

    public static void insert(VosiActionCaptureModel vosiActionCapture) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<VosiActionCaptureModel, Integer> vosiActionCaptureDao = ormDbHelper.getVosiActionCaptureDao();

            vosiActionCaptureDao.create(vosiActionCapture);

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

    public static boolean update(VosiActionCaptureModel vosiActionCapture) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VosiActionCaptureModel, Integer> vosiActionCaptureDao = ormDbHelper.getVosiActionCaptureDao();

            return (vosiActionCaptureDao.update(vosiActionCapture) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

    public static List<VosiActionCaptureModel> getUnSyncedVosiActionCapture() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VosiActionCaptureModel, Integer> vosiActionCaptureDao = ormDbHelper.getVosiActionCaptureDao();

            final List<VosiActionCaptureModel> vosiActionCaptureList = vosiActionCaptureDao.query(vosiActionCaptureDao.queryBuilder()
                    .where()
                    .eq(UPLOADED_COLUMN, false).prepare());

            return vosiActionCaptureList;
        }finally {
            ormDbHelper.close();
        }
    }
}
