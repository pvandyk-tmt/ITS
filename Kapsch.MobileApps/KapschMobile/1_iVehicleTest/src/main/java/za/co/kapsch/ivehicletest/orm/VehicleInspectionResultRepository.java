package za.co.kapsch.ivehicletest.orm;

import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.ivehicletest.App;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultModel;
import za.co.kapsch.ivehicletest.Models.VehicleMakeModel;
import za.co.kapsch.ivehicletest.Models.VehicleMakeModelModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.LibApp;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2017/12/06.
 */

public class VehicleInspectionResultRepository {

    public static final String ID_COLUMN = "ID";
    public static final String UPLOADED_COLUMN = "Uploaded";
    public static final String BOOKING_ID_COLUMN = "BookingID";
    public static final String VEHICLE_INSPECTION_RESULTS_ID = "vehicleInspectionResultsID";

    public static void create(VehicleInspectionResultModel vehicleInspectionResult) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<VehicleInspectionResultModel, Integer> vehicleInspectionResultDao = ormDbHelper.getVehicleInspectionResultDao();

            vehicleInspectionResultDao.create(vehicleInspectionResult);

            db.setTransactionSuccessful();
        }
        catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionResultRepository::create"), ErrorSeverity.High);
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static List<VehicleInspectionResultModel> getVehicleInspectionResultList() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleInspectionResultModel, Integer> vehicleInspectionResultDao = ormDbHelper.getVehicleInspectionResultDao();

            final List<VehicleInspectionResultModel> vehicleInspectionResultList = vehicleInspectionResultDao.queryForAll();

            return vehicleInspectionResultList;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static List<VehicleInspectionResultModel> getUnSyncedVehicleInspectionResults(long vehicleInspectionResultsID) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleInspectionResultModel, Integer> vehicleInspectionResultDao = ormDbHelper.getVehicleInspectionResultDao();

            final List<VehicleInspectionResultModel> vehicleInspectionResultList = vehicleInspectionResultDao.query(vehicleInspectionResultDao.queryBuilder()
                    .where()
                    .eq(UPLOADED_COLUMN, false)
                    .and()
                    .eq(VEHICLE_INSPECTION_RESULTS_ID, vehicleInspectionResultsID).prepare());

            return vehicleInspectionResultList;
        }finally {
            ormDbHelper.close();
        }
    }

    public static boolean update(VehicleInspectionResultModel vehicleInspectionResult) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleInspectionResultModel, Integer> vehicleInspectionResultDao = ormDbHelper.getVehicleInspectionResultDao();

            return (vehicleInspectionResultDao.update(vehicleInspectionResult) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

//    public static List<VehicleInspectionResultModel> getBookingIdList() throws SQLException {
//
//        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());
//
//        try {
//            final Dao<VehicleInspectionResultModel, Integer> vehicleInspectionResultDao = ormDbHelper.getVehicleInspectionResultDao();
//
//            final List<VehicleInspectionResultModel> vehicleInspectionResultList =
//                    vehicleInspectionResultDao
//                            .queryBuilder()
//                            .distinct()
//                            .selectColumns(BOOKING_ID_COLUMN).query();
//
//           return vehicleInspectionResultList;
//        }
//        finally {
//            ormDbHelper.close();
//        }
//    }

    public static List<Long> getBookingIdList() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            SQLiteDatabase db = ormDbHelper.getWritableDatabase();
            String query = "SELECT distinct BookingID FROM VehicleInspectionResult WHERE Uploaded = 0";
            Cursor cursor = db.rawQuery(query, null);

            List<Long> bookingIdList = new ArrayList<>();

            cursor.moveToFirst();

            while ( !cursor.isAfterLast()) {
                bookingIdList.add(cursor.getLong(cursor.getColumnIndex(BOOKING_ID_COLUMN)));
                cursor.moveToNext();
            }

            return bookingIdList;

        }finally {
            ormDbHelper.close();
        }
    }

    public static List<VehicleInspectionResultModel> getVehicleInspectionResultList(long bookingID) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleInspectionResultModel, Integer> vehicleInspectionResultDao = ormDbHelper.getVehicleInspectionResultDao();

            final List<VehicleInspectionResultModel> vehicleInspectionResultList = vehicleInspectionResultDao.query(vehicleInspectionResultDao.queryBuilder()
                    .where()
                    .eq(BOOKING_ID_COLUMN, bookingID).prepare());

            return vehicleInspectionResultList;
        }finally {
            ormDbHelper.close();
        }
    }
}
