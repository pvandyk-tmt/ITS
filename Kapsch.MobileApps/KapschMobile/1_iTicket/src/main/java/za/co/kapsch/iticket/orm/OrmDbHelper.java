package za.co.kapsch.iticket.orm;


import android.content.Context;

import com.j256.ormlite.android.AndroidConnectionSource;
import com.j256.ormlite.android.DatabaseTableConfigUtil;
import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.dao.DaoManager;
import com.j256.ormlite.table.DatabaseTableConfig;
import com.readystatesoftware.sqliteasset.SQLiteAssetHelper;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.Models.CancellationReasonModel;
import za.co.kapsch.iticket.Models.ChargeInfoModel;
import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.iticket.Models.CountryModel;
import za.co.kapsch.iticket.Models.CourtDateModel;
import za.co.kapsch.iticket.Models.CourtModel;
import za.co.kapsch.iticket.Models.CourtRoomModel;
import za.co.kapsch.iticket.Models.DatabaseScriptModel;
import za.co.kapsch.iticket.Models.DeviceItemModel;
import za.co.kapsch.iticket.Models.EvidenceModel;
import za.co.kapsch.iticket.Models.CourtDetailModel;
import za.co.kapsch.iticket.Models.IdentificationTypeModel;
import za.co.kapsch.iticket.Models.InfringementLocationModel;
import za.co.kapsch.iticket.Models.VosiActionCaptureModel;
import za.co.kapsch.iticket.Models.VosiActionModel;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.iticket.Models.HandWrittenModel;
import za.co.kapsch.iticket.Models.PublicHolidayModel;
//import za.co.kapsch.iticket.Models.TestTableModel;
import za.co.kapsch.iticket.Models.TicketNumberModel;
import za.co.kapsch.shared.Models.UserModel;


/**
 * Created by zarah.dominguez on 20/01/15.
 */
public class OrmDbHelper extends SQLiteAssetHelper {

    protected AndroidConnectionSource mConnectionSource = new AndroidConnectionSource(this);

    // name of the database file for your application
    public static final String DATABASE_NAME = "iTicket.db";

    // any time you make changes to your database objects, increase the database version
    public static final int DATABASE_VERSION = 2;

    // the DAO object we use to access the tables
    private Dao<UserModel, Integer> mUserDao = null;
    private Dao<CourtModel, Integer> mCourtDao = null;
    private Dao<CourtDetailModel, Integer> mCourtDetailDao = null;
    private Dao<ChargeInfoModel, Integer> mChargeBookDao = null;
    private Dao<DeviceItemModel, Integer> mDeviceItemDao = null;
    private Dao<DatabaseScriptModel, Integer> mDatabaseScriptDao = null;
    private Dao<PublicHolidayModel, Integer> mPublicHolidayDao = null;
    private Dao<TicketNumberModel, Integer> mTicketNumberDao = null;
    private Dao<CancellationReasonModel, Integer> mCancellationReasonDao = null;
    private Dao<EvidenceModel, Integer> mEvidenceDao = null;
    private Dao<CourtDateModel, Integer> mCourtDateDao = null;
    private Dao<CourtRoomModel, Integer> mCourtRoomDao = null;
    private Dao<ConfigItemModel, Integer> mConfigurationDao = null;
    private Dao<HandWrittenModel, Integer> mHandWrittenDao = null;
    private Dao<VosiActionModel, Integer> mVosiActionDao = null;
    private Dao<VosiActionCaptureModel, Integer> mVosiActionCaptureDao = null;
    private Dao<InfringementLocationModel, Integer> mInfringementLocationDao = null;
    private Dao<IdentificationTypeModel, Integer> mIdentificationTypeDao = null;
    private Dao<CountryModel, Integer> mCountryDao = null;

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
        mUserDao = null;
        mCourtDao = null;
        mCourtDetailDao = null;
        mChargeBookDao = null;
        mDeviceItemDao = null;
        mDatabaseScriptDao = null;
        mPublicHolidayDao = null;
        mTicketNumberDao = null;
        mCancellationReasonDao = null;
        mEvidenceDao = null;
        mCourtDateDao = null;
        mCourtRoomDao = null;
        mConfigurationDao = null;
        mHandWrittenDao = null;
        mVosiActionDao = null;
        mVosiActionCaptureDao = null;
        mInfringementLocationDao = null;
        mIdentificationTypeDao = null;
        mCountryDao = null;
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

    public Dao<CourtDetailModel, Integer> getCourtDetailDao() throws SQLException {
        if (mCourtDetailDao == null) {
            mCourtDetailDao = getDao(CourtDetailModel.class);
        }
        return mCourtDetailDao;
    }

    public Dao<CourtModel, Integer> getCourtDao() throws SQLException {
        if (mCourtDao == null) {
            mCourtDao = getDao(CourtModel.class);
        }
        return mCourtDao;
    }

    public Dao<ChargeInfoModel, Integer> getChargeBookDao() throws SQLException {
        if (mChargeBookDao == null) {
            mChargeBookDao = getDao(ChargeInfoModel.class);
        }
        return mChargeBookDao;
    }

    public Dao<DeviceItemModel, Integer> getDeviceItemDao() throws SQLException {
        if (mDeviceItemDao == null) {
            mDeviceItemDao = getDao(DeviceItemModel.class);
        }
        return mDeviceItemDao;
    }

    public Dao<DatabaseScriptModel, Integer> getDatabaseScriptDao() throws SQLException {
        if (mDatabaseScriptDao == null) {
            mDatabaseScriptDao = getDao(DatabaseScriptModel.class);
        }
        return mDatabaseScriptDao;
    }

    public Dao<PublicHolidayModel, Integer> getPublicHolidayDao() throws SQLException {
        if (mPublicHolidayDao == null) {
            mPublicHolidayDao = getDao(PublicHolidayModel.class);
        }
        return mPublicHolidayDao;
    }

    public Dao<TicketNumberModel, Integer> getTicketNumberDao() throws SQLException {
        if (mTicketNumberDao == null) {
            mTicketNumberDao = getDao(TicketNumberModel.class);
        }
        return mTicketNumberDao;
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

    public Dao<CourtDateModel, Integer> getCourtDateDao() throws SQLException {
        if (mCourtDateDao == null) {
            mCourtDateDao = getDao(CourtDateModel.class);
        }
        return mCourtDateDao;
    }

    public Dao<CourtRoomModel, Integer> getCourtRoomDao() throws SQLException {
        if (mCourtRoomDao == null) {
            mCourtRoomDao = getDao(CourtRoomModel.class);
        }
        return mCourtRoomDao;
    }

    public Dao<ConfigItemModel, Integer> getConfigurationDao() throws SQLException {
        if (mConfigurationDao == null) {
            mConfigurationDao = getDao(ConfigItemModel.class);
        }
        return mConfigurationDao;
    }

    public Dao<HandWrittenModel, Integer> getHandWrittenDao() throws SQLException {
        if (mHandWrittenDao == null) {
            mHandWrittenDao = getDao(HandWrittenModel.class);
        }
        return mHandWrittenDao;
    }

    public Dao<VosiActionModel, Integer> getVosiActionDao() throws SQLException {
        if (mVosiActionDao == null) {
            mVosiActionDao = getDao(VosiActionModel.class);
        }
        return mVosiActionDao;
    }

    public Dao<VosiActionCaptureModel, Integer> getVosiActionCaptureDao() throws SQLException {
        if (mVosiActionCaptureDao == null) {
            mVosiActionCaptureDao = getDao(VosiActionCaptureModel.class);
        }
        return mVosiActionCaptureDao;
    }

    public Dao<InfringementLocationModel, Integer> getInfringementLocationDao() throws SQLException {
        if (mInfringementLocationDao == null) {
            mInfringementLocationDao = getDao(InfringementLocationModel.class);
        }
        return mInfringementLocationDao;
    }

    public Dao<IdentificationTypeModel, Integer> getIdentificationTypeDao() throws SQLException {
        if (mIdentificationTypeDao == null) {
            mIdentificationTypeDao = getDao(IdentificationTypeModel.class);
        }

        return mIdentificationTypeDao;
    }

    public Dao<CountryModel, Integer> getCountryDao() throws SQLException {
        if (mCountryDao == null) {
            mCountryDao = getDao(CountryModel.class);
        }

        return mCountryDao;
    }

//    public Dao<TestTableModel, Integer> getTestTableDao() throws SQLException {
//        if (mTestTableDao == null) {
//            mTestTableDao = getDao(TestTableModel.class);
//        }
//        return mTestTableDao;
//    }

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
