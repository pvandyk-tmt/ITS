package za.co.kapsch.ivehicletest.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.ivehicletest.App;
import za.co.kapsch.ivehicletest.Models.VehicleMakeModel;
import za.co.kapsch.ivehicletest.Models.VehicleMakeModelModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2017/11/28.
 */

public class VehicleMakeModelRepository {

    public static final String ID_COLUMN = "ID";

    public static VehicleMakeModelModel getVehicleMakeModel(long id) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleMakeModelModel, Integer> vehicleMakeModelDao = ormDbHelper.getVehicleMakeModelDao();

            final VehicleMakeModelModel vehicleMakeModel = vehicleMakeModelDao.queryForFirst(vehicleMakeModelDao.queryBuilder()
                    .where()
                    .eq(ID_COLUMN, id).prepare());

            return vehicleMakeModel;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static List<VehicleMakeModelModel> getVehicleMakeModelList() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleMakeModelModel, Integer> vehicleMakeModelDao = ormDbHelper.getVehicleMakeModelDao();

            final List<VehicleMakeModelModel> vehicleMakeModelList = vehicleMakeModelDao.queryForAll();

            return vehicleMakeModelList;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static void cleanInsert(List<VehicleMakeModelModel> vehicleMakeModelList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<VehicleMakeModelModel, Integer> vehicleMakeModelDao = ormDbHelper.getVehicleMakeModelDao();

            ormDbHelper.deleteAll(vehicleMakeModelDao);

            for (VehicleMakeModelModel vehicleMakeModel : vehicleMakeModelList) {
                vehicleMakeModelDao.create(vehicleMakeModel);
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
