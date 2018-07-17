package za.co.kapsch.ivehicletest.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.ivehicletest.App;
import za.co.kapsch.ivehicletest.Models.VehicleModelNumberModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2017/11/28.
 */

public class VehicleModelNumberRepository {

    public static final String ID_COLUMN = "ID";

    public static VehicleModelNumberModel getVehicleModelNumber(long id) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleModelNumberModel, Integer> vehicleModelNumberDao = ormDbHelper.getVehicleModelNumberDao();

            final VehicleModelNumberModel vehicleModelNumber = vehicleModelNumberDao.queryForFirst(vehicleModelNumberDao.queryBuilder()
                    .where()
                    .eq(ID_COLUMN, id).prepare());

            return vehicleModelNumber;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static List<VehicleModelNumberModel> getVehicleMakeModelList() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleModelNumberModel, Integer> vehicleModelNumberDao = ormDbHelper.getVehicleModelNumberDao();

            final List<VehicleModelNumberModel> vehicleModelNumberList = vehicleModelNumberDao.queryForAll();

            return vehicleModelNumberList;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static void cleanInsert(List<VehicleModelNumberModel> vehicleModelNumberList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<VehicleModelNumberModel, Integer> vehicleModelNumberDao = ormDbHelper.getVehicleModelNumberDao();

            ormDbHelper.deleteAll(vehicleModelNumberDao);

            for (VehicleModelNumberModel vehicleModelNumber : vehicleModelNumberList) {
                vehicleModelNumberDao.create(vehicleModelNumber);
            }

            db.setTransactionSuccessful();
        }
        catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleMakeModelRepository::cleanInsert"), ErrorSeverity.High);
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }
}
