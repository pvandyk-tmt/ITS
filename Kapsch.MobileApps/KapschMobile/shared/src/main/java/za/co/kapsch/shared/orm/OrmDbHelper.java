package za.co.kapsch.shared.orm;

import android.content.Context;

import com.j256.ormlite.android.AndroidConnectionSource;
import com.j256.ormlite.android.DatabaseTableConfigUtil;
import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.dao.DaoManager;
import com.j256.ormlite.table.DatabaseTableConfig;
import com.readystatesoftware.sqliteasset.SQLiteAssetHelper;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.shared.Models.ConfigItemModel;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Models.UserModel;


/**
 * Created by zarah.dominguez on 20/01/15.
 */
public class OrmDbHelper extends SQLiteAssetHelper {

    protected AndroidConnectionSource mConnectionSource = new AndroidConnectionSource(this);

    // name of the database file for your application
    public static final String DATABASE_NAME = "Shared.db";

    // any time you make changes to your database objects, increase the database version
    public static final int DATABASE_VERSION = 1;

    // the DAO object we use to access the tables
    private Dao<UserActivityLogModel, Integer> mUserActivityLogDao = null;
    private Dao<ConfigItemModel, Integer> mConfigurationDao = null;

    public OrmDbHelper(Context context) {
        super(context, DATABASE_NAME, null, DATABASE_VERSION);
    }

    /**
     * Close the database connections and clear any cached DAOs.
     */
    @Override
    public void close() {

        super.close();
        mUserActivityLogDao = null;
        mConfigurationDao = null;
    }

    /**
     *
     * @return Dao we need to access the UserModel table
     *
     * @throws SQLException
     */
    public Dao<UserActivityLogModel, Integer> getUserActivityLogDao() throws SQLException {
        if (mUserActivityLogDao == null) {
            mUserActivityLogDao = getDao(UserActivityLogModel.class);
        }
        return mUserActivityLogDao;
    }

    public Dao<ConfigItemModel, Integer> getConfigItemDao() throws SQLException {
        if (mConfigurationDao == null) {
            mConfigurationDao = getDao(ConfigItemModel.class);
        }
        return mConfigurationDao;
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
