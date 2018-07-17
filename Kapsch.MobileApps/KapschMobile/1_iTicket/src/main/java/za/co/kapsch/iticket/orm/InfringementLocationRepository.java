package za.co.kapsch.iticket.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Models.InfringementLocationModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/10/12.
 */

public class InfringementLocationRepository {

    public static final String CODE_COLUMN = "Code";

    public static InfringementLocationModel getLocation(String code) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<InfringementLocationModel, Integer> infringementLocationDao = ormDbHelper.getInfringementLocationDao();

            final InfringementLocationModel infringementLocationModel = infringementLocationDao.queryForFirst(infringementLocationDao.queryBuilder()
                    .where()
                    .eq(CODE_COLUMN, code).prepare());


            return infringementLocationModel;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static void cleanInsert(List<InfringementLocationModel> infringementLocationList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<InfringementLocationModel, Integer> infringementLocationDao = ormDbHelper.getInfringementLocationDao();

            ormDbHelper.deleteAll(infringementLocationDao);

            for (InfringementLocationModel infringementLocation : infringementLocationList) {
                infringementLocationDao.create(infringementLocation);
            }

            db.setTransactionSuccessful();
        }
        catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "InfringementLocationRepository::cleanInsert"), ErrorSeverity.High);
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }
}
