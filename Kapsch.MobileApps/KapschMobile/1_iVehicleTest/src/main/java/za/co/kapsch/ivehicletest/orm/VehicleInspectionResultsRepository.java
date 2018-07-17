package za.co.kapsch.ivehicletest.orm;

import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;
import java.util.logging.ErrorManager;

import za.co.kapsch.ivehicletest.App;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultsModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2018/01/22.
 */

public class VehicleInspectionResultsRepository {

    public static final String UPLOADED_COLUMN = "Uploaded";
    public static final String BOOKING_ID_COLUMN = "BookingID";

    public static void createOrUpdate(VehicleInspectionResultsModel vehicleInspectionResults) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<VehicleInspectionResultsModel, Integer> vehicleInspectionResultsDao = ormDbHelper.getVehicleInspectionResultsDao();
            final Dao<VehicleInspectionResultModel, Integer> vehicleInspectionResultDao = ormDbHelper.getVehicleInspectionResultDao();

            Dao.CreateOrUpdateStatus status = vehicleInspectionResultsDao.createOrUpdate(vehicleInspectionResults);

            for (VehicleInspectionResultModel vehicleInspectionResult : vehicleInspectionResults.getVehicleInspectionResultList()) {
                vehicleInspectionResult.setVehicleInspectionResultsID(vehicleInspectionResults.getID());
                vehicleInspectionResultDao.createOrUpdate(vehicleInspectionResult);
            }

            db.setTransactionSuccessful();
        }finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static List<VehicleInspectionResultsModel> getVehicleInspectionResultList() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleInspectionResultsModel, Integer> vehicleInspectionResultsDao = ormDbHelper.getVehicleInspectionResultsDao();

            final List<VehicleInspectionResultsModel> vehicleInspectionResultsList = vehicleInspectionResultsDao.queryForAll();

            return vehicleInspectionResultsList;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static List<VehicleInspectionResultsModel> getUnSyncedVehicleInspectionResults() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleInspectionResultsModel, Integer> vehicleInspectionResultsDao = ormDbHelper.getVehicleInspectionResultsDao();

            final List<VehicleInspectionResultsModel> vehicleInspectionResultsList = vehicleInspectionResultsDao.query(vehicleInspectionResultsDao.queryBuilder()
                    .where()
                    .eq(UPLOADED_COLUMN, false).prepare());

            return vehicleInspectionResultsList;
        }finally {
            ormDbHelper.close();
        }
    }

    public static boolean update(VehicleInspectionResultsModel vehicleInspectionResults) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleInspectionResultsModel, Integer> vehicleInspectionResultsDao = ormDbHelper.getVehicleInspectionResultsDao();

            return (vehicleInspectionResultsDao.update(vehicleInspectionResults) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

    public static List<VehicleInspectionResultsModel> getVehicleInspectionResultsList(long bookingID) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleInspectionResultsModel, Integer> vehicleInspectionResultsDao = ormDbHelper.getVehicleInspectionResultsDao();

            final List<VehicleInspectionResultsModel> vehicleInspectionResultsList = vehicleInspectionResultsDao.query(vehicleInspectionResultsDao.queryBuilder()
                    .where()
                    .eq(BOOKING_ID_COLUMN, bookingID).prepare());

            return vehicleInspectionResultsList;
        }finally {
            ormDbHelper.close();
        }
    }
}
