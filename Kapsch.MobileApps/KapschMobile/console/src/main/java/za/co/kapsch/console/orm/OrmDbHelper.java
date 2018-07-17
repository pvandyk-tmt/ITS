package za.co.kapsch.console.orm;


import android.content.Context;

import com.j256.ormlite.android.AndroidConnectionSource;
import com.j256.ormlite.android.DatabaseTableConfigUtil;
import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.dao.DaoManager;
import com.j256.ormlite.table.DatabaseTableConfig;
import com.readystatesoftware.sqliteasset.SQLiteAssetHelper;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.console.Models.ConfigItemModel;
import za.co.kapsch.shared.Models.MobileDeviceModel;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.console.Models.MobileDeviceApplicationModel;
import za.co.kapsch.console.Models.MobileDeviceLocationModel;
import za.co.kapsch.shared.Models.SystemFunctionModel;
import za.co.kapsch.shared.Models.UserModel;


/**
 * Created by zarah.dominguez on 20/01/15.
 */
public class OrmDbHelper extends SQLiteAssetHelper {

    protected AndroidConnectionSource mConnectionSource = new AndroidConnectionSource(this);

    // name of the database file for your application
    public static final String DATABASE_NAME = "Console.db";

    // any time you make changes to your database objects, increase the database version
    public static final int DATABASE_VERSION = 2;

    // the DAO object we use to access the tables
    private Dao<UserModel, Integer> mUserDao = null;
    private Dao<SystemFunctionModel, Integer> mSystemFunctionDao = null;
    private Dao<DistrictModel, Integer> mDistrictDao = null;
    private Dao<MobileDeviceLocationModel, Integer> mMobileDeviceLocationDao = null;
    private Dao<ConfigItemModel, Integer> mConfigurationDao = null;
    private Dao<MobileDeviceApplicationModel, Integer> mMobileDeviceApplicationDao = null;
    private Dao<MobileDeviceModel, Integer> mMobileDeviceDao = null;

    public OrmDbHelper(Context context) {
        super(context, DATABASE_NAME, null, DATABASE_VERSION);
    }

//BEGIN TODO: update method when database structure changes
//for more info see: https://github.com/jgilfelt/android-sqlite-asset-helper
//    Database Upgrades
//
//    At a certain point in your application's lifecycle you will need to alter it's database structure to support additional features. You must ensure users who have installed your app prior to this can safely upgrade their local databases without the loss of any locally held data.
//
//    To facilitate a database upgrade, increment the version number that you pass to your SQLiteAssetHelper constructor:
//
//    private static final int DATABASE_VERSION = 2;
//    Update the initial SQLite database in the project's assets/databases directory with the changes and create a text file containing all required SQL commands to upgrade the database from its previous version to it's current version and place it in the same folder. The required naming convention for this upgrade file is as follows:
//
//    assets/databases/<database_name>_upgrade_<from_version>-<to_version>.sql
//    For example, northwind.db_upgrade_1-2.sql upgrades the database named "northwind.db" from version 1 to 2. You can include multiple upgrade files to upgrade between any two given versions.
//
//    If there are no files to form an upgrade path from a previously installed version to the current one, the class will throw a SQLiteAssetHelperException.
//
//    The samples:database-v2-upgrade project demonstrates a simple upgrade to the Northwind database which adds a FullName column to the Employee table.

//    OR
    //The onUpgrade method seems to work as well, but scripts must then be embedded in the code.
//    @Override
//    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
//        // Code to upgrade your db here
//
//        while (oldVersion != newVersion) {
//            switch (oldVersion) {
//                case 1:
//                    updatesVersion1(db);
//                    break;
//            }
//
//            oldVersion++;
//        }
//    }

//    private void updatesVersion1(SQLiteDatabase db){
        //db.execSQL("CREATE TABLE IF NOT EXISTS TutorialsPoint(Username VARCHAR,Password VARCHAR);");
        //db.execSQL("INSERT INTO TutorialsPoint VALUES('admin','admin');");
//    }

    //END TODO

    /**
     * Close the database connections and clear any cached DAOs.
     */
    @Override
    public void close() {

        super.close();
        mUserDao = null;
        mSystemFunctionDao = null;
        mDistrictDao = null;
        mMobileDeviceLocationDao = null;
        mConfigurationDao = null;
        mMobileDeviceApplicationDao = null;
        mMobileDeviceDao = null;
    }

    /**
     *
     * @return Dao we need to access the UserModel table
     *
     * @throws java.sql.SQLException
     */
    public Dao<UserModel, Integer> getUserDao() throws SQLException {
        if (mUserDao == null) {
            mUserDao = getDao(UserModel.class);
        }
        return mUserDao;
    }

    public Dao<SystemFunctionModel, Integer> getSystemFunctionDao() throws SQLException {
        if (mSystemFunctionDao == null) {
            mSystemFunctionDao = getDao(SystemFunctionModel.class);
        }
        return mSystemFunctionDao;
    }

    public Dao<DistrictModel, Integer> getDistrictDao() throws SQLException {
        if (mDistrictDao == null) {
            mDistrictDao = getDao(DistrictModel.class);
        }
        return mDistrictDao;
    }

    public Dao<MobileDeviceLocationModel, Integer> getMobileDeviceLocationDao() throws SQLException {
        if (mMobileDeviceLocationDao == null) {
            mMobileDeviceLocationDao = getDao(MobileDeviceLocationModel.class);
        }
        return mMobileDeviceLocationDao;
    }

    public Dao<ConfigItemModel, Integer> getConfigurationDao() throws SQLException {
        if (mConfigurationDao == null) {
            mConfigurationDao = getDao(ConfigItemModel.class);
        }
        return mConfigurationDao;
    }

    public Dao<MobileDeviceApplicationModel, Integer> getMobileDeviceApplicationDao() throws SQLException {
        if (mMobileDeviceApplicationDao == null) {
            mMobileDeviceApplicationDao = getDao(MobileDeviceApplicationModel.class);
        }
        return mMobileDeviceApplicationDao;
    }

    public Dao<MobileDeviceModel, Integer> getMobileDeviceDao() throws SQLException {
        if (mMobileDeviceDao == null) {
            mMobileDeviceDao = getDao(MobileDeviceModel.class);
        }
        return mMobileDeviceDao;
    }

    //use this method if all records must be deleted and there could be more than 999 records
    public static <T> void deleteAll (Dao<T, Integer> dao) throws SQLException {
        List<T> list;
        do {
            list = dao.queryBuilder().limit(999).query();
            if (list.size() > 0) {
                dao.delete(list);
            }
        }while (list.size() > 0);
    }

    /**
     * Lifted off of https://github.com/j256/ormlite-examples/blob/master/android/HelloAndroidNoHelper/src/com/example/hellonohelper/DatabaseHelper.java
     *
     *
     * @param clazz
     * @param <D>
     * @param <T>
     * @return
     * @throws java.sql.SQLException
     */
    private <D extends Dao<T, ?>, T> D getDao(Class<T> clazz) throws SQLException {
        // lookup the dao, possibly invoking the cached database config
        Dao<T, ?> dao = DaoManager.lookupDao(mConnectionSource, clazz);
        if (dao == null) {
            // try to use our new reflection magic
            DatabaseTableConfig<T> tableConfig = DatabaseTableConfigUtil.fromClass(mConnectionSource, clazz);
            if (tableConfig == null) {
                /**
                 * TODO: we have to do this to get to see if they are using the deprecated annotations like
                 * {@link DatabaseFieldSimple}.
                 */
                dao = (Dao<T, ?>) DaoManager.createDao(mConnectionSource, clazz);
            } else {
                dao = (Dao<T, ?>) DaoManager.createDao(mConnectionSource, tableConfig);
            }
        }

        @SuppressWarnings("unchecked")
        D castDao = (D) dao;
        return castDao;
    }
}
