package za.co.kapsch.console.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.App;
import za.co.kapsch.console.General.Constants;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.console.Models.ConfigItemModel;

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

    public static List<ConfigItemModel> getKapschPackages() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigurationDao();

            return configurationDao.query(configurationDao.queryBuilder()
                    .where()
                    .like(NAME_COLUMN, String.format("%s", Constants.KAPSCH_ANDROID_PACKAGE_NAME + "%")).prepare());
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
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
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
