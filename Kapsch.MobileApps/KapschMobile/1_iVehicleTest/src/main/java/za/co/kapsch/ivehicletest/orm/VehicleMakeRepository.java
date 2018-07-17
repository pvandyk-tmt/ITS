package za.co.kapsch.ivehicletest.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.ivehicletest.App;
import za.co.kapsch.ivehicletest.Models.VehicleMakeModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2017/11/28.
 */

public class VehicleMakeRepository {

    public static final String ID_COLUMN = "ID";

    public static VehicleMakeModel getVehicleMake(long id) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleMakeModel, Integer> vehicleMakeDao = ormDbHelper.getVehicleMakeDao();

            final VehicleMakeModel vehicleMake = vehicleMakeDao.queryForFirst(vehicleMakeDao.queryBuilder()
                    .where()
                    .eq(ID_COLUMN, id).prepare());

            return vehicleMake;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static List<VehicleMakeModel> getVehicleMakeList() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<VehicleMakeModel, Integer> vehicleMakeDao = ormDbHelper.getVehicleMakeDao();

            final List<VehicleMakeModel> vehicleMakeList = vehicleMakeDao.queryForAll();

            return vehicleMakeList;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static void cleanInsert(List<VehicleMakeModel> vehicleMakeModelList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<VehicleMakeModel, Integer> vehicleMakeDao = ormDbHelper.getVehicleMakeDao();

             ormDbHelper.deleteAll(vehicleMakeDao);

            for (VehicleMakeModel vehicleMake : vehicleMakeModelList) {
                vehicleMakeDao.create(vehicleMake);
            }

            db.setTransactionSuccessful();
        }
        catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleMakeRepository::cleanInsert"), ErrorSeverity.High);
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }
}
