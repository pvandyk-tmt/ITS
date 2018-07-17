package za.co.kapsch.shared.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.LibApp;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/07/05.
 */

public class UserActivityLogRepository {

    public static final String ID_COLUMN = "ID";
    public static final String UPLOADED_COLUMN = "Uploaded";

    public static void create(UserActivityLogModel userActivityLog) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(LibApp.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<UserActivityLogModel, Integer> userActivityLogDao = ormDbHelper.getUserActivityLogDao();

            userActivityLogDao.create(userActivityLog);

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static boolean delete(UserActivityLogModel userActivityLog) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(LibApp.getContext());

        try {
            final Dao<UserActivityLogModel, Integer> userActivityLogDao = ormDbHelper.getUserActivityLogDao();

            return (userActivityLogDao.delete(userActivityLog) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

//    public static List<UserActivityLogModel> getUserActivityLogs() throws SQLException {
//
//        final OrmDbHelper ormDbHelper = new OrmDbHelper(LibApp.getContext());
//
//        try {
//            final Dao<UserActivityLogModel, Integer> userActivityLogDao = ormDbHelper.getUserActivityLogDao();
//
//            final List<UserActivityLogModel> userActivityLogs = userActivityLogDao.queryForAll();
//
//            return userActivityLogs;
//        }catch (Exception e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "UserActivityLogRepository::getUserActivityLogs()"), ErrorSeverity.High);
//            return null;
//        }
//        finally {
//            ormDbHelper.close();
//        }
//    }

    public static boolean update(UserActivityLogModel userActivityLog) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(LibApp.getContext());

        try {
            final Dao<UserActivityLogModel, Integer> userActivityLogDao = ormDbHelper.getUserActivityLogDao();

            return (userActivityLogDao.update(userActivityLog) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

    public static List<UserActivityLogModel> getUnSyncedUserActivityLogs() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(LibApp.getContext());

        try {
            final Dao<UserActivityLogModel, Integer> userActivityLogDao = ormDbHelper.getUserActivityLogDao();

            final List<UserActivityLogModel> userActivityLogList = userActivityLogDao.query(userActivityLogDao.queryBuilder()
                    .where()
                    .eq(UPLOADED_COLUMN, false).prepare());

            return userActivityLogList;
        }finally {
            ormDbHelper.close();
        }
    }
}
