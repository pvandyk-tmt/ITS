package za.co.kapsch.ivehicletest.orm;


import android.content.Context;

import com.j256.ormlite.android.AndroidConnectionSource;
import com.j256.ormlite.android.DatabaseTableConfigUtil;
import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.dao.DaoManager;
import com.j256.ormlite.table.DatabaseTableConfig;
import com.readystatesoftware.sqliteasset.SQLiteAssetHelper;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.ivehicletest.Models.CancellationReasonModel;
import za.co.kapsch.ivehicletest.Models.ConfigItemModel;
import za.co.kapsch.ivehicletest.Models.EvidenceModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultsModel;
import za.co.kapsch.ivehicletest.Models.VehicleMakeModel;
import za.co.kapsch.ivehicletest.Models.VehicleMakeModelModel;
import za.co.kapsch.ivehicletest.Models.VehicleModelNumberModel;

//import za.co.kapsch.iticket.Models.TestTableModel;


/**
 * Created by zarah.dominguez on 20/01/15.
 */
public class OrmDbHelper extends SQLiteAssetHelper {

    protected AndroidConnectionSource mConnectionSource = new AndroidConnectionSource(this);

    // name of the database file for your application
    public static final String DATABASE_NAME = "iVehicleTest.db";

    // any time you make changes to your database objects, increase the database version
    public static final int DATABASE_VERSION = 3;

    // the DAO object we use to access the tables
    private Dao<ConfigItemModel, Integer> mConfigurationDao = null;
    private Dao<VehicleMakeModel, Integer> mVehicleMakeDao = null;
    private Dao<VehicleMakeModelModel, Integer> mVehicleMakeModelDao = null;
    private Dao<VehicleModelNumberModel, Integer> mVehicleModelNumberDao = null;
    private Dao<VehicleInspectionResultModel, Integer> mVehicleInspectionResultDao = null;
    private Dao<VehicleInspectionResultsModel, Integer> mVehicleInspectionResultsDao = null;
    private Dao<CancellationReasonModel, Integer> mCancellationReasonDao = null;
    private Dao<EvidenceModel, Integer> mEvidenceDao = null;

    //private Dao<TestTableModel, Integer> mTestTableDao = null;

    public OrmDbHelper(Context context) {
        super(context, DATABASE_NAME, null, DATABASE_VERSION);
    }

//BEGIN TODO: update method when database sturcture changes
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
        mConfigurationDao = null;
        mVehicleMakeDao = null;
        mVehicleMakeModelDao = null;
        mVehicleModelNumberDao = null;
        mVehicleInspectionResultDao = null;
        mVehicleInspectionResultsDao = null;
        mCancellationReasonDao = null;
        mEvidenceDao = null;
    }

    /**
     *
     * @return Dao we need to access the UserModel table
     *
     * @throws SQLException
     */

    public Dao<ConfigItemModel, Integer> getConfigurationDao() throws SQLException {
        if (mConfigurationDao == null) {
            mConfigurationDao = getDao(ConfigItemModel.class);
        }
        return mConfigurationDao;
    }

    public Dao<VehicleMakeModel, Integer> getVehicleMakeDao() throws SQLException {
        if (mVehicleMakeDao == null) {
            mVehicleMakeDao = getDao(VehicleMakeModel.class);
        }
        return mVehicleMakeDao;
    }

    public Dao<VehicleMakeModelModel, Integer> getVehicleMakeModelDao() throws SQLException {
        if (mVehicleMakeModelDao == null) {
            mVehicleMakeModelDao = getDao(VehicleMakeModelModel.class);
        }
        return mVehicleMakeModelDao;
    }

    public Dao<VehicleModelNumberModel, Integer> getVehicleModelNumberDao() throws SQLException {
        if (mVehicleModelNumberDao == null) {
            mVehicleModelNumberDao = getDao(VehicleModelNumberModel.class);
        }
        return mVehicleModelNumberDao;
    }

    public Dao<VehicleInspectionResultModel, Integer> getVehicleInspectionResultDao() throws SQLException {
        if (mVehicleInspectionResultDao == null) {
            mVehicleInspectionResultDao = getDao(VehicleInspectionResultModel.class);
        }
        return mVehicleInspectionResultDao;
    }

    public Dao<VehicleInspectionResultsModel, Integer> getVehicleInspectionResultsDao() throws SQLException {
        if (mVehicleInspectionResultsDao == null) {
            mVehicleInspectionResultsDao = getDao(VehicleInspectionResultsModel.class);
        }
        return mVehicleInspectionResultsDao;
    }

    public Dao<CancellationReasonModel, Integer> getCancellationReasonDao() throws SQLException {
        if (mCancellationReasonDao == null) {
            mCancellationReasonDao = getDao(CancellationReasonModel.class);
        }
        return mCancellationReasonDao;
    }

    public Dao<EvidenceModel, Integer> getEvidenceDao() throws SQLException {
        if (mEvidenceDao == null) {
            mEvidenceDao = getDao(EvidenceModel.class);
        }
        return mEvidenceDao;
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
     * @throws SQLException
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
