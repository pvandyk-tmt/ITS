package za.co.kapsch.console.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.App;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.Models.SystemFunctionModel;

/**
 * Created by CSenekal on 2017/07/05.
 */

public class SystemFunctionRepository {

    public static final String NAME_COLUMN = "Name";
    public static final String DESCRITPTION_COLUMN = "Description";
    public static final String ID_COLUMN = "ID";

    public static List<SystemFunctionModel> getSystemFunctions(String IDs) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<SystemFunctionModel, Integer> systemFunctionDao = ormDbHelper.getSystemFunctionDao();

            final List<SystemFunctionModel> systemFunctions = systemFunctionDao.query(systemFunctionDao.queryBuilder()
                    .where()
                    .in(ID_COLUMN, IDs).prepare());

            return systemFunctions;
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "SystemFunctionRepository::getSystemFunctions()"), ErrorSeverity.High);
            return null;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static void deleteAll() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<SystemFunctionModel, Integer> systemFunctionDao = ormDbHelper.getSystemFunctionDao();

            ormDbHelper.deleteAll(systemFunctionDao);

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static void cleanInsert(List<SystemFunctionModel> systemFunctions) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<SystemFunctionModel, Integer> systemFunctionDao = ormDbHelper.getSystemFunctionDao();

            ormDbHelper.deleteAll(systemFunctionDao);

            for (SystemFunctionModel systemFunction : systemFunctions) {
                systemFunctionDao.create(systemFunction);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static void insert(List<SystemFunctionModel> systemFunctions) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<SystemFunctionModel, Integer> systemFunctionDao = ormDbHelper.getSystemFunctionDao();

            for (SystemFunctionModel systemFunction: systemFunctions){
                systemFunctionDao.create(systemFunction);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }
}
