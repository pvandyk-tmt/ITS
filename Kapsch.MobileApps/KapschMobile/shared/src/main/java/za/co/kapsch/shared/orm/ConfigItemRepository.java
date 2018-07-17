package za.co.kapsch.shared.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.shared.Models.ConfigItemModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.LibApp;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/02/23.
 */
public class ConfigItemRepository {

    public static final String NAME_COLUMN = "Name";

    public static List<ConfigItemModel> getAll() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(LibApp.getContext());

        try {
            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigItemDao();

            return configurationDao.queryForAll();
        }finally {
            ormDbHelper.close();
        }
    }

    public static ConfigItemModel getConfigItem(String name) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(LibApp.getContext());

        try {
            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigItemDao();

            final ConfigItemModel configuration = configurationDao.queryForFirst(configurationDao.queryBuilder()
                    .where()
                    .eq(NAME_COLUMN, name).prepare());

            return configuration;
        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean update(ConfigItemModel configItem) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(LibApp.getContext());

        try {
            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigItemDao();
            return (configurationDao.update(configItem) > 0);
        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
            return false;
        } finally {
            ormDbHelper.close();
        }
    }

    public static void cleanInsert(List<ConfigItemModel> configurationList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(LibApp.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigItemDao();

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

        final OrmDbHelper ormDbHelper = new OrmDbHelper(LibApp.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigItemDao();

            configurationDao.create(configItem);

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static long getMaxID()throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(LibApp.getContext());

        try {
            final Dao<ConfigItemModel, Integer> configurationDao = ormDbHelper.getConfigItemDao();
            return configurationDao.queryRawValue("select MAX(ID) from ConfigItem");
        } finally {
            ormDbHelper.close();
        }
    }
}
