package za.co.kapsch.console.orm;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.console.General.App;
import za.co.kapsch.console.General.Constants;
import za.co.kapsch.console.Models.InsertResultModel;
import za.co.kapsch.console.Models.MobileDeviceLocationModel;

/**
 * Created by CSenekal on 2017/07/13.
 */
public class MobileDeviceLocationRepository {

    private static final String IS_SYNCED_COLUMN = "IsSynced";
    private static final String ID_COLUMN = "ID";

    public static void Create(MobileDeviceLocationModel mobileDeviceLocation) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<MobileDeviceLocationModel, Integer> MobileDeviceLocationDao = ormDbHelper.getMobileDeviceLocationDao();
            MobileDeviceLocationDao.create(mobileDeviceLocation);
        } finally {
            ormDbHelper.close();
        }
    }

    public static List<MobileDeviceLocationModel> getGpsLogs() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try
        {
            final Dao<MobileDeviceLocationModel, Integer> MobileDeviceLocationDao = ormDbHelper.getMobileDeviceLocationDao();

            final List<MobileDeviceLocationModel> mobileDeviceLocations = MobileDeviceLocationDao.query(MobileDeviceLocationDao.queryBuilder()
                    .where()
                    .eq(IS_SYNCED_COLUMN, false).prepare());

            return mobileDeviceLocations;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static void update(MobileDeviceLocationModel mobileDeviceLocation) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<MobileDeviceLocationModel, Integer> MobileDeviceLocationDao = ormDbHelper.getMobileDeviceLocationDao();
            MobileDeviceLocationDao.update(mobileDeviceLocation);
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static boolean delete(MobileDeviceLocationModel mobileDeviceLocation) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<MobileDeviceLocationModel, Integer> MobileDeviceLocationDao = ormDbHelper.getMobileDeviceLocationDao();

            return (MobileDeviceLocationDao.delete(mobileDeviceLocation) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

//    public static void deleteLogs(List<InsertResultModel> insertResults) throws SQLException {
//
//        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());
//
//        try {
//            final Dao<MobileDeviceLocationModel, Integer> MobileDeviceLocationDao = ormDbHelper.getMobileDeviceLocationDao();
//
//            for (InsertResultModel insertResult : insertResults) {
//
//                if (insertResult.getMessage() == Constants.EMPTY_STRING) {
//
//                    final MobileDeviceLocationModel gpsLog = MobileDeviceLocationDao.queryForFirst(MobileDeviceLocationDao.queryBuilder()
//                            .where()
//                            .eq(ID_COLUMN, insertResult.getId()).prepare());
//
//                    if (gpsLog != null) {
//                        if ((MobileDeviceLocationDao.delete(gpsLog) > 0) == false) {
//                            throw new SQLException("GpsLog delete failed");
//                        }
//                    }
//                }
//            }
//        } finally {
//            ormDbHelper.close();
//        }
//    }

    public static void deleteLogs() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<MobileDeviceLocationModel, Integer> MobileDeviceLocationDao = ormDbHelper.getMobileDeviceLocationDao();
            ormDbHelper.deleteAll(MobileDeviceLocationDao);
        } finally {
            ormDbHelper.close();
        }
    }
}
