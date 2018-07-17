package za.co.kapsch.ipayment.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.ipayment.General.App;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.ipayment.Models.ConfigItemModel;

/**
 * Created by CSenekal on 2017/02/23.
 */
public class ConfigItemRepository {

    public static final String NAME_COLUMN = "Name";

    public static List<ConfigItemModel> getAll() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigurationDao();

            return configurationDao.queryForAll();
        }finally {
            ormDbHelper.close();
        }
    }

    public static ConfigItemModel getConfigItem(String name) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigurationDao();

            final ConfigItemModel configuration = configurationDao.queryForFirst(configurationDao.queryBuilder()
                    .where()
                    .eq(NAME_COLUMN, name).prepare());

            return configuration;
        }catch (SQLException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "getConfigItem"), ErrorSeverity.High);
            return null;
        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean update(ConfigItemModel configuration) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigurationDao();
            return (configurationDao.update(configuration) > 0);
        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "update"), ErrorSeverity.High);
            return false;
        } finally {
            ormDbHelper.close();
        }
    }

    public static void cleanInsert(List<ConfigItemModel> configurationList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigurationDao();

            ormDbHelper.deleteAll(configurationDao);

            for (ConfigItemModel configuration : configurationList) {
                configurationDao.create(configuration);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static void insert(ConfigItemModel configItem) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigurationDao();

            configurationDao.create(configItem);

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static long getMaxID()throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigurationDao();
            return configurationDao.queryRawValue("select MAX(ID) from ConfigItem");
        } finally {
            ormDbHelper.close();
        }
    }
}
