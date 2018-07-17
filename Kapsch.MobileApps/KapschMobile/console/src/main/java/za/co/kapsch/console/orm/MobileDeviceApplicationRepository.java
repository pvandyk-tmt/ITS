package za.co.kapsch.console.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.console.Enums.ApplicationType;
import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.App;
import za.co.kapsch.console.General.Constants;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.console.Models.MobileDeviceApplicationModel;

/**
 * Created by CSenekal on 2017/07/20.
 */
public class MobileDeviceApplicationRepository {

    public static final String NAME_COLUMN = "Name";
    public static final String APPLICATION_TYPE_COLUMN = "ApplicationType";

    public static List<MobileDeviceApplicationModel> getAll() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<MobileDeviceApplicationModel, Integer> mobileDeviceApplicationDao = ormDbHelper.getMobileDeviceApplicationDao();

            return mobileDeviceApplicationDao.queryForAll();
        }finally {
            ormDbHelper.close();
        }
    }

    public static List<MobileDeviceApplicationModel> getApplications(ApplicationType applicationType) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<MobileDeviceApplicationModel, Integer> mobileDeviceApplicationDao = ormDbHelper.getMobileDeviceApplicationDao();

            return mobileDeviceApplicationDao.query(mobileDeviceApplicationDao.queryBuilder()
                    .where()
                    .eq(APPLICATION_TYPE_COLUMN, applicationType).prepare());
        }finally {
            ormDbHelper.close();
        }
    }

    public static MobileDeviceApplicationModel getMobileDeviceApplication(String name) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<MobileDeviceApplicationModel, Integer> mobileDeviceApplicationDao = ormDbHelper.getMobileDeviceApplicationDao();

            final MobileDeviceApplicationModel mobileDeviceApplication = mobileDeviceApplicationDao.queryForFirst(mobileDeviceApplicationDao.queryBuilder()
                    .where()
                    .eq(NAME_COLUMN, name).prepare());

            return mobileDeviceApplication;
        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean update(MobileDeviceApplicationModel mobileDeviceApplication) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<MobileDeviceApplicationModel, Integer> mobileDeviceApplicationDao = ormDbHelper.getMobileDeviceApplicationDao();
            return (mobileDeviceApplicationDao.update(mobileDeviceApplication) > 0);
        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
            return false;
        } finally {
            ormDbHelper.close();
        }
    }

    public static void cleanInsert(List<MobileDeviceApplicationModel> mobileDeviceApplicationList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<MobileDeviceApplicationModel, Integer> mobileDeviceApplicationDao = ormDbHelper.getMobileDeviceApplicationDao();

            ormDbHelper.deleteAll(mobileDeviceApplicationDao);

            for (MobileDeviceApplicationModel mobileDeviceApplication : mobileDeviceApplicationList) {
                mobileDeviceApplicationDao.create(mobileDeviceApplication);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static void insert(MobileDeviceApplicationModel mobileDeviceApplication) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<MobileDeviceApplicationModel, Integer> mobileDeviceApplicationDao = ormDbHelper.getMobileDeviceApplicationDao();

            mobileDeviceApplicationDao.create(mobileDeviceApplication);

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static long getMaxID()throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<MobileDeviceApplicationModel, Integer> mobileDeviceApplicationDao = ormDbHelper.getMobileDeviceApplicationDao();
            return mobileDeviceApplicationDao.queryRawValue("select MAX(ID) from MobileDeviceApplication");
        } finally {
            ormDbHelper.close();
        }
    }
}
