package za.co.kapsch.iticket.orm;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Constants;
import za.co.kapsch.iticket.Models.DeviceItemModel;

/**
 * Created by csenekal on 2016-09-12.
 */
public class DeviceItemRepository {

    public static DeviceItemModel getDeviceItem(String description) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<DeviceItemModel, Integer> deviceItemDao = ormDbHelper.getDeviceItemDao();

            return deviceItemDao.queryForFirst(deviceItemDao.queryBuilder()
                    .where().eq(Constants.TABLE_DEVICE_ITEM_DESC_COLUMN, description).prepare());
        } finally {
            ormDbHelper.close();
        }
    }

    public static List<DeviceItemModel> getAll() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<DeviceItemModel, Integer> deviceItemDao = ormDbHelper.getDeviceItemDao();

            return deviceItemDao.queryForAll();
        }finally {
            ormDbHelper.close();
        }
    }

    public static boolean create(DeviceItemModel deviceItem) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<DeviceItemModel, Integer> deviceItemDao = ormDbHelper.getDeviceItemDao();

            return (deviceItemDao.create(deviceItem) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean update(DeviceItemModel deviceItem) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<DeviceItemModel, Integer> deviceItemDao = ormDbHelper.getDeviceItemDao();

            return (deviceItemDao.update(deviceItem) > 0);
        } finally {
            ormDbHelper.close();
        }
    }
}
